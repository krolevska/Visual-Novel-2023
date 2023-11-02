using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Characters;

namespace Dialoque
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config => _config;
        public DialogueContainer dialogueContainer = new DialogueContainer();
        private ConversationManager conversationManager;
        private TextArchitect architect;
        public static DialogueSystem instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPromt_Next;
        public bool isRunningConveration => conversationManager.isRunning;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }
        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized)
                return;
            architect = new TextArchitect(dialogueContainer.diaglogueText);
            conversationManager = new ConversationManager(architect);
        }

        public void OnUserPrompt_Next()
        {
            onUserPromt_Next?.Invoke();
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config);
        }

        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
        }
        public void ShowSpeakerName(string speakerName = "")
        {
            
            if (speakerName != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);
            else
                dialogueContainer.nameContainer.Hide();
        }
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> conversation)
        {
            return conversationManager.StartConversation(conversation);
        }
    }
}