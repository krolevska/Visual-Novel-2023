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
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character_Model3D Ama = CreateCharacter("Amarya") as Character_Model3D;

            Ama.SetPosition(Vector2.one);

            yield return new WaitForSeconds(1);

            yield return null;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}