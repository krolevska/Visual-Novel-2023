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

        public void SetDialogueColor(Color color) => diaglogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => diaglogueText.font = font;
    }
}
