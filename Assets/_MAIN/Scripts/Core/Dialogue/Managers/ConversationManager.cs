using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;

namespace Dialoque
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;
        public bool isRunning => process != null;
        private TextArchitect architect = null;
        private bool userPrompt = false;
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPromt_Next += OnUserPrompt_Next;
        }
        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }
        public Coroutine StartConversation(List<string> conversation)
        {
            StopConversation();

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));

            return process;
        }

        public void StopConversation()
        {
            if (!isRunning)
                return;
            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation(List<string> conversation)
        {
            for (int i = 0; i < conversation.Count; i++)
            {
                // don't show empty lines
                if (string.IsNullOrWhiteSpace(conversation[i]))
                    continue;
                DialogueLine line = DialogueParser.Parse(conversation[i]);
                //has dialogue
                if (line.hasDialogue)
                {
                    yield return Line_RunDialogue(line);
                }
                //has command
                if (line.hasCommands)
                {
                    yield return Line_RunCommands(line);
                }
                if (line.hasDialogue)               
                    yield return WaitForUserInput();
            }
        }

        IEnumerator Line_RunDialogue(DialogueLine line)
        {
            //has speaker
            if (line.hasSpeaker)
                dialogueSystem.ShowSpeakerName(line.speakerData.displayName);

            yield return BuildLineSegments(line.dialogueData);
        }

        IEnumerator Line_RunCommands(DialogueLine line)
        {
            List<DL_CommandData.Command> commands = line.commandsData.commands;

            foreach (DL_CommandData.Command command in commands)
            {
                if (command.waitForCompletion)
                    yield return CommandManager.instance.Execute(command.name, command.arguments);
                CommandManager.instance.Execute(command.name, command.arguments);
            }
            yield return null;
        }
        IEnumerator BuildLineSegments(DL_DialogueData line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DialogueData.Dialogue_Segment segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DialogueData.Dialogue_Segment segment)
        {
            switch(segment.startSignal)
            {
                case DL_DialogueData.Dialogue_Segment.StartSignal.C:
                case DL_DialogueData.Dialogue_Segment.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DialogueData.Dialogue_Segment.StartSignal.WC:
                case DL_DialogueData.Dialogue_Segment.StartSignal.WA:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
                    default:
                    break;
            }
        }
        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            if (!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);

            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();

                    userPrompt = false;
                }
                yield return null;
            }
        }

        IEnumerator WaitForUserInput()
        {
            while (!userPrompt)
                yield return null;

            userPrompt = false;
        }
    }
}