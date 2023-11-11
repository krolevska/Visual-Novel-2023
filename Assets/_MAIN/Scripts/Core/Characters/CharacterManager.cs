using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialoque;
using System.Linq;

namespace Characters
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;
        private const string characterCasting_ID = " as ";

        private const string characterName_ID = "<charname>";
        public string characterRootPathFormat => $"Characters/{characterName_ID}";
        public string characterPrefabNameFormat => $"Character - [{characterName_ID}]";
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/{characterPrefabNameFormat}";

        [SerializeField] private RectTransform _characterPanel = null;
        [SerializeField] private RectTransform _characterPanel_live2D = null;
        [SerializeField] private Transform _characterPanel_model3D = null;
        public RectTransform characterPanel => _characterPanel;
        public RectTransform characterPanelLive2D => _characterPanel_live2D;
        public Transform characterPanelModel3D => _characterPanel_model3D;

        void Awake()
        {
            instance = this;
        }
        public CharacterConfigData GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }
        public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
                return characters[characterName.ToLower()];
            else if (createIfDoesNotExist)
                return CreateCharacter(characterName);

            return null;
        }
        public Character CreateCharacter(string characterName)
        {
            Debug.Log($"Create {characterName}.");

            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.Log($"A character {characterName} already exists!");
                return null;
            }
            Character_Info info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(info.name.ToLower(), character);

            return character;
        }

        private Character_Info GetCharacterInfo(string characterName)
        {
            Character_Info result = new Character_Info();

            string[] nameData = characterName.Split(characterCasting_ID, System.StringSplitOptions.RemoveEmptyEntries);
            result.name = nameData[0];
            result.castingName = nameData.Length > 1? nameData[1] : result.name;

            result.config = config.GetConfig(result.castingName);

            result.prefab = GetPrefabForCharacter(result.castingName);

            result.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);

            return result;
        }
        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }
        public string FormatCharacterPath (string path, string characterName) => path.Replace(characterName_ID, characterName);
        private Character CreateCharacterFromInfo(Character_Info info)
        {
            CharacterConfigData config = info.config;

            switch (config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, info.config);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, info.config, info.prefab, info.rootCharacterFolder);
                case Character.CharacterType.Live2D:
                    return new Character_Live2D(info.name, info.config, info.prefab, info.rootCharacterFolder);
                case Character.CharacterType.Model3D:
                    return new Character_Model3D(info.name, info.config, info.prefab, info.rootCharacterFolder);
                default:
                    return null;
            }
        }

        public void SortCharacters()
        {
            List<Character> activeCharacters = characters.Values.Where(c => c.root.gameObject.activeInHierarchy && c.isVisible).ToList();
            List<Character> inactiveCharacters = characters.Values.Except(activeCharacters).ToList();

            activeCharacters.Sort((a,b) => a.priority.CompareTo(b.priority));
            activeCharacters.Concat(inactiveCharacters);

            SortCharacters(activeCharacters);
        }

        public void SortCharacters(string[] characterNames)
        {
            List<Character> sortedCharacter = new List<Character> ();

            sortedCharacter = characterNames
                .Select(name => GetCharacter(name))
                .Where(character => character != null)
                .ToList();

            List<Character> remainingCharacters = characters.Values
                .Except(sortedCharacter)
                .OrderBy(character => character.priority)
                .ToList();

            sortedCharacter.Reverse();

            int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(c =>  c.priority) : 0;
            for (int i = 0; i < sortedCharacter.Count; i++)
            {
                Character character = sortedCharacter[i];
                character.SetPriority(startingPriority + i + 1, autoSortCharsOnUI: false);
            }

            List<Character> allCharacters = remainingCharacters.Concat(sortedCharacter).ToList();
            SortCharacters (allCharacters);
        }

        private void SortCharacters(List<Character> charactersSortingOrder)
        {
            int i = 0;
            foreach (Character character in charactersSortingOrder)
            {
                character.root.SetSiblingIndex(i++);
                character.OnSort(i);
            }
        }

        public int GetCharacterCountFromCharacterType(Character.CharacterType characterType)
        {
            int count = 0;
            foreach (var c in characters.Values)
            {
                if (c.config.characterType == characterType)
                    count++;
            }
            return count;
        }
        private class Character_Info
        {
            public string name = "";
            public string castingName = "";

            public string rootCharacterFolder = "";
            public CharacterConfigData config = null;

            public GameObject prefab = null;
        }
    }
}