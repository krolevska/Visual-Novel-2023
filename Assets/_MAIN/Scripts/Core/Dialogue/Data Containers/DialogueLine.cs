using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dialoque
{
    public class DialogueLine
    {
        public DL_SpeakerData speakerData;
        public DL_DialogueData dialogueData;
        public DL_CommandData commandsData;

        public bool hasDialogue => dialogueData != null;
        public bool hasCommands => commandsData != null;
        public bool hasSpeaker => speakerData != null;


        public DialogueLine(string speaker, string dialogue, string commands)
        {
            this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new DL_SpeakerData(speaker));
            this.dialogueData = (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DialogueData(dialogue));
            this.commandsData = (string.IsNullOrWhiteSpace(commands) ? null : new DL_CommandData(commands));
        }
    }
}