using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dialoque

{
    public class TextFiles : MonoBehaviour
    {
        private string fileName = "testFile";
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Run());
        }
        IEnumerator Run()
        {
            List<string> lines = FileManager.ReadTextAsset(fileName, true);

            foreach (string line in lines)
                Debug.Log(line);

            yield return null;
        }
    }
}