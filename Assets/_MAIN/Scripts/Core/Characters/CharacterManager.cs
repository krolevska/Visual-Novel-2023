using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialoque;

namespace Characters
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;

        void Awake()
        {
            instance = this;
        }

        public Character CreateCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.Log($"A character {characterName} already exists!");
                return null;
            }
            Character_Info info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(characterName.ToLower(), character);

            return character;
        }

        private Character_Info GetCharacterInfo(string characterName)
        {
            Character_Info result = new Character_Info();
            result.name = characterName;

            result.config = config.GetConfig(characterName);

            return result;
        }
        private Character CreateCharacterFromInfo(Character_Info info)
        {
            CharacterConfigData config = info.config;

            if (config.characterType == Character.CharacterType.Text)
            {
                return new Character_Text(info.name);
            }
            if (config.characterType == Character.CharacterType.Sprite || config.characterType == Character.CharacterType.SpriteSheet)
            {
                return new Character_Sprite(info.name);
            }
            if (config.characterType == Character.CharacterType.Live2D)
            {
                return new Character_Live2D(info.name);
            }
            if (config.characterType == Character.CharacterType.Model3D)
            {
                return new Character_Model3D(info.name);
            }
            return null;
        }

        private class Character_Info
        {
            public string name = "";

            public CharacterConfigData config = null;
        }
    }
}