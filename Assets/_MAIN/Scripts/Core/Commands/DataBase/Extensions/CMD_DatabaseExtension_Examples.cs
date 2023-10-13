using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialoque;
using Commands;

namespace TESTING
{
    public class CMD_DatabaseExtension_Examples : CMD_DatabaseExtention
    {
        new public static void Extend(CommandDataBase dataBase)
        {
            //Add Action with no params
            dataBase.AddCommand("print", new Action(PrintDefaultMessage));
            dataBase.AddCommand("print_1p", new Action<string>(PrintUserMessage));
            dataBase.AddCommand("print_mp", new Action<string[]>(PrintLines));

            // Add lambda wit no params
            dataBase.AddCommand("lambda", new Action(() => { Debug.Log("Printing a default message to console from lambda command."); }));
            dataBase.AddCommand("lambda_1p", new Action<string>((arg) => { Debug.Log($"Log user lambda message: '{arg}'."); }));
            dataBase.AddCommand("lambda_mp", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));
            //Add coroutine with no params
            dataBase.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
            dataBase.AddCommand("process", new Func<string, IEnumerator>(LineProcess));
            dataBase.AddCommand("process", new Func<string[], IEnumerator>(MultiLineProcess));

            //Special example
            dataBase.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));

        }

        private static void PrintDefaultMessage()
        {
            Debug.Log("Printing a defaut message to console");
        }

        private static void PrintUserMessage(string message)
        {
            Debug.Log($"User Message: '{message}'");
        }

        private static void PrintLines(string[] lines)
        {
            int i = 1;
            foreach (string line in lines)
            {
                Debug.Log($"{i++}: {line}");
            }
        }

        private static IEnumerator SimpleProcess()
        {
            for (int i = 1; i <= 5; i++)
            {
                Debug.Log($"Process running... [{i}]");
                yield return new WaitForSeconds(1);
            }
        }
        private static IEnumerator LineProcess(string data)
        {
            if (int.TryParse(data, out int num))
            {
                for (int i = 0; i < num; i++)
                {
                    Debug.Log($"Process running... [{i}]");
                    yield return new WaitForSeconds(1);
                }
            }
        }
        private static IEnumerator MultiLineProcess(string[] data)
        {

            foreach (string line in data)
            {
                Debug.Log($"Process message... [{line}]");
                yield return new WaitForSeconds(0.5f);
            }
        }

        private static IEnumerator MoveCharacter(string direction)
        {
            bool left = direction.ToLower() == "left";

            //Get vars defined sw else
            Transform character = GameObject.Find("Image").transform;
            float moveSpeed = 15;

            //Calculate the target pos
            float targetX = left ? -8 : 8;

            //Calc current pos
            float currentX = character.position.x;

            // Move image gradually tow target
            while (Mathf.Abs(targetX - currentX) > 0.1f)
            {
                currentX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
                character.position = new Vector3(currentX, character.position.y, character.position.z);
                yield return null;
            }

        }
    }
}