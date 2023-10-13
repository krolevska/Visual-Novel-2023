using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialoque;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

        string[] lines = new string[5]
        {
            "This is a random line of dialogue.",
            "I want to say something, come over here.",
            "The world is a crazy place sometimes.",
            "Don't lose hope, things will get better",
            "It's a bird? It's a plane? No! - It's a shakhed!"
        };
        // Start is called before the first frame update
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.diaglogueText);
            architect.buildMethod = TextArchitect.BuildMethod.fade;
        }

        // Update is called once per frame
        void Update()
        {
            if (bm != architect.buildMethod)
            {
                architect.buildMethod = bm;
                architect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.S))
                architect.Stop();

            string longLine = "This is a very long line that makes no sence. This is a very long line that makes no sence. This is a very long line that makes no sence. This is a very long line that makes no sence. This is a very long line that makes no sence. ";
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();
                }
                else
                {
                    architect.Build(longLine);
                    //architect.Build(lines[Random.Range(0, lines.Length)]);
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Build(longLine);

                //architect.Append(lines[Random.Range(0, lines.Length)]);
            }
        }
    }
}