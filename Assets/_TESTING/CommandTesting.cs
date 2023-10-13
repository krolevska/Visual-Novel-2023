using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;

public class CommandTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            CommandManager.instance.Execute("moveCharDemo", "left");
        if (Input.GetKeyUp(KeyCode.RightArrow))
            CommandManager.instance.Execute("moveCharDemo", "right");
    }

    IEnumerator Running()
    {
        yield return CommandManager.instance.Execute("print");
        yield return CommandManager.instance.Execute("print_1p", "Hello");
        yield return CommandManager.instance.Execute("print_mp", "line1", "line2");

        yield return CommandManager.instance.Execute("lambda");
        yield return CommandManager.instance.Execute("lambda_1p", "Hello");
        yield return CommandManager.instance.Execute("lambda_mp", "lambda1", "lambda2");

        yield return CommandManager.instance.Execute("process");
        yield return CommandManager.instance.Execute("process_1p", "Hello");
        yield return CommandManager.instance.Execute("process_mp", "process1", "process2");
    }    
}
