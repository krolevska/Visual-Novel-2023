using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialoque;

namespace TESTING
{
    public class TestParsing : MonoBehaviour
    {
        [SerializeField] private TextAsset file;
        // Start is called before the first frame update
        void Start()
        {
            SendFileToParse();
        }
        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset("testFile");
            foreach(string line in lines)
            {
                DialogueLine dl = DialogueParser.Parse(line);
            }
        }
    }
}