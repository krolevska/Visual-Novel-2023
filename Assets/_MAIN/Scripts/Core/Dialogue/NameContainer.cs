using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Dialoque
{
    /// <summary>
    /// Bpx that holds Name on screen. Part of the dialogue container
    /// </summary>
    [System.Serializable]
    public class NameContainer
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI nameText;

        // Start is called before the first frame update
        public void Show(string nameToShow = "")
        {
            root.SetActive(true);
            if (nameToShow != string.Empty)
                nameText.text = nameToShow;
        }

        // Update is called once per frame
        public void Hide()
        {
            root.SetActive(false);

        }
    }
}
