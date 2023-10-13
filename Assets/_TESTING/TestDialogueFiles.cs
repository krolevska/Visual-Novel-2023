using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialoque;

public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField] private TextAsset fileToRead = null;
    // Start is called before the first frame update
    void Start()
    {
        StartConversation();
    }
    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset("testFile");
        /* test segments
         * 
        foreach (string line in lines)
        {
            Debug.Log($"Segmenting line '{line}'");
            DialogueLine dlLine = DialogueParser.Parse(line);

            int i = 0;
            foreach(DL_DialogueData.Dialogue_Segment segment in dlLine.dialogue.segments)
            {
                Debug.Log($"Segment [{i++}] = '{segment.dialogue}' [signal={segment.startSignal.ToString()}{(segment.signalDelay > 0? $" {segment.signalDelay}": $"")}]");
            }
        } */   
        
        /* test commands
         
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            DialogueLine dl = DialogueParser.Parse(line);

            
            for (int i = 0; i < dl.commandsData.commands.Count; i++)
            {
                DL_CommandData.Command command = dl.commandsData.commands[i];
                Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
            }
        }*/

        /* test speaker signals 
        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            DialogueLine dl = DialogueParser.Parse(line);

            Debug.Log($"{dl.speaker.name} as [{(dl.speaker.castName != string.Empty ? dl.speaker.castName : dl.speaker.name)}] at {dl.speaker.castPosition}");

            List<(int l, string ex)> expr = dl.speaker.CastExpressions;
            for (int c = 0; c < expr.Count; c++)
            {
                Debug.Log($"[Layer[{expr[c].l}] = '{expr[c].ex}']");
            }
        }*/

        DialogueSystem.instance.Say(lines);
    }
}
