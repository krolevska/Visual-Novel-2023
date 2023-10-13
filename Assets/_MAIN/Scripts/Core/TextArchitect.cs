using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TextArchitect is responsible for controlling the construction and animation of text on the screen.
/// </summary>
public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;     // Returns the active TextMeshPro component.

    public string currentText => tmpro.text; // Returns the current text in the active TextMeshPro component.

    public string targetText { get; private set; } = ""; // Target text to be displayed.

    public string preText { get; private set; } = ""; // Text to be prepended to the target text.

    private int preTextLength = 0;

    public string fullTargetText => preText + targetText; // Returns the final text.


    public enum BuildMethod { instant, typewriter, fade }    // Enumerates the methods to build the text.

    public BuildMethod buildMethod = BuildMethod.fade;

    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } } // Gets or sets the text color.

    // Gets or sets the speed at which text is built, modified by the speedMultiplier.
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }

    private const float baseSpeed = 1;
    private float speedMultiplier = 1; // Chosen by a user.

    // Gets the number of characters to be built per cycle, based on the speed.
    public int charactersPerCycle { get { return speed <= 2 ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }

    private int characterMultiplier = 1;
    public bool hurryUp = false;

   
    public TextArchitect(TextMeshProUGUI tmpro_ui) // Constructor for UI text.
    {
        this.tmpro_ui = tmpro_ui;
    }
    public TextArchitect(TextMeshPro tmpro_world) // Constructor for world space text.
    {
        this.tmpro_world = tmpro_world;
    }
    public Coroutine Build(string text) // Initiates the build process with the specified text.
    {
        preText = "";
        targetText = text;

        Stop();
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }
    public Coroutine Append(string text) // Appends the specified text to the current text. Used when the previous text didn't disappear yet.
    {
        preText = tmpro.text;
        targetText = text;

        Stop();
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }
    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null; // Indicates whether the build process is currently active.      
    public void Stop() // Stops the current build process, if active.
    {
        if (!isBuilding)
            return;
        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }
    IEnumerator Building() // Coroutine for building the text.
    {
        Prepare();

        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }

        OnCompete();
    }
    private void OnCompete()
    {
        buildProcess = null;
        hurryUp = false;
    }
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate();
                break;
        }
        Stop();
        OnCompete();
    }
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
    }
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }
    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;
        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }
        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }
    private void Prepare_Fade()
    {
        tmpro.text = preText;
        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else
            preTextLength = 0;
        tmpro.text += targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color colorVisible = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            if (i < preTextLength)
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;
            }
            else
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    private IEnumerator Build_Typewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle;
            
            yield return new WaitForSeconds(0.015f / speed);
        }

    }
    private IEnumerator Build_Fade()
    {
        int minRange = preTextLength;
        int maxRange = minRange + 1;

        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while (true)
        {
            float fadeSpeed = ((hurryUp ? charactersPerCycle * 5 : charactersPerCycle) * speed) * 4f;
            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];

                if (alphas[i] >= 255)
                    minRange++;
            }
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1] > alphaThreshold || lastCharacterIsInvisible)
            {
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                else if (alphas[maxRange - 1] >= 255 || lastCharacterIsInvisible)
                    break;
            }

            yield return new WaitForEndOfFrame();
        }

    }
}
