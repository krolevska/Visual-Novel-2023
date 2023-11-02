using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Dialoque;
using TMPro;
using System;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset font;
        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);
        // Start is called before the first frame update
        void Start()
        {
            // Character Stella = CharacterManager.instance.CreateCharacter("Female Student 2");
            // Character s = CharacterManager.instance.CreateCharacter("Stella");
            // Character s2 = CharacterManager.instance.CreateCharacter("Stella");
            // Character r = CharacterManager.instance.CreateCharacter("Roman");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {

            Character_Sprite Stella = CreateCharacter("Stella") as Character_Sprite;
            Character_Sprite EllaRed = CreateCharacter("Ella as Stella") as Character_Sprite;
            Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;
            Character_Sprite GuardRed = CreateCharacter("GuardRed as Generic") as Character_Sprite;
            
            GuardRed.SetColor(Color.red);
            EllaRed.SetColor(Color.red);

            Stella.SetPosition(new Vector2(0.3f, 0));
            Guard.SetPosition(new Vector2(0.45f, 0));
            GuardRed.SetPosition(new Vector2(0.6f, 0));
            EllaRed.SetPosition(new Vector2(0.75f, 0));

            GuardRed.SetPriority(1000);
            Stella.SetPriority(15);
            Guard.SetPriority(8);
            EllaRed.SetPriority(30);
            
            yield return new WaitForSeconds(1);

            CharacterManager.instance.SortCharacters(new string[] { "Stella", "Guard as Generic" });
            yield return new WaitForSeconds(1);

            Stella.Animate("Shiver", true);
            Guard.Animate("Hop");

            yield return null;

        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}