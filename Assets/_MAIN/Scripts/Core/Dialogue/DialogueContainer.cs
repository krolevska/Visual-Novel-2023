using UnityEngine;
using TMPro;
namespace Dialoque
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer = new NameContainer();
        public TextMeshProUGUI diaglogueText;
    }
}
