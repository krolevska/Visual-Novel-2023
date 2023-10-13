using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Dialoque;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Character e = CharacterManager.instance.CreateCharacter("Ellen");
            // Character s = CharacterManager.instance.CreateCharacter("Stella");
            // Character s2 = CharacterManager.instance.CreateCharacter("Stella");
            // Character r = CharacterManager.instance.CreateCharacter("Roman");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character Ellen = CharacterManager.instance.CreateCharacter("Ellen");
            Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            Character Sara = CharacterManager.instance.CreateCharacter("Sara");

            List<string> lines = new List<string>()
            {
                "Hi, there!"
            };
            yield return Ellen.Say(lines);

            lines = new List<string>()
            {
                "My name is Adam.",
                "What's your name?"
            };
            yield return Adam.Say(lines);

            lines = new List<string>()
            {
                "Oh, {wa 1}that's very nice."
            };
            yield return Sara.Say(lines);

        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}