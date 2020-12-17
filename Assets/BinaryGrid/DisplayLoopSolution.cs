using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class DisplayLoopSolution : MonoBehaviour
{
    // Display the solution loops above the grid & update the sum-of-products text.

    public GameObject overlayGroup;
    public Transform overlayStartOffset;
    public GameObject baseLoopSprite;
    private List<GameObject> displayedLoops = new List<GameObject>();
    [Space]
    public Text loopTextSOP;
    public float textTintIntensity;
    private Coroutine drawLoopRoutine;

    public void Clear() {
        foreach (GameObject loop in displayedLoops)
            Destroy(loop);
        displayedLoops = new List<GameObject>();
        loopTextSOP.text = "Press Solve to draw loops and simplify the Karnaugh map.";
    }

    public void Draw() {
        if (!(drawLoopRoutine is null))
            StopCoroutine(drawLoopRoutine);
        drawLoopRoutine = StartCoroutine(DrawCoroutine());
    }

    public void TextDraw(float hue=0f, bool useHue=false) {
        // Build the "sum of products" string for the current loops.
        // If 2 - 4 inputs are used, each product is shaded the colour of the loop it represents.

        StringBuilder styledExpression = new StringBuilder();
        for (int i = 0; i < Main.Instance.loops.Count; i++) {
            if (i > 0) styledExpression.Append(" + ");

            if (useHue) {
                Color matchingColor = Color.HSVToRGB(hue, textTintIntensity, textTintIntensity);
                styledExpression.Append("<color=#" + ColorUtility.ToHtmlStringRGB(matchingColor) + ">");
            }
            styledExpression.Append(Main.Instance.loops[i].ToProductsString());

            if (useHue) {
                styledExpression.Append("</color>");
                hue += 1f / Main.Instance.loops.Count; // Cycle through hues.
                hue %= 1f;
            }
        }
        loopTextSOP.text = styledExpression.ToString();
    }

    public IEnumerator DrawCoroutine() {
        // Add sprites over the grid for the next few frames, to display the current loops.
        float uniqueSpriteHue = Random.value;
        TextDraw(uniqueSpriteHue, true);

        foreach (KMapLoop loop in Main.Instance.loops) {
            yield return new WaitForEndOfFrame();

            LoopSprite loopSpritePlacement = CreateLoopSprite(uniqueSpriteHue);
            List<int[]> wrapAround = loopSpritePlacement.PositionInOverlay(loop);

            foreach (int[] position in wrapAround) {
                // Deal with copies of the loop that are shown "wrapping" around the grid.
                LoopSprite cloneSpritePlacement = CreateLoopSprite(uniqueSpriteHue);
                cloneSpritePlacement.SetPosition(position[0], position[1], position[2], position[3]);
            }
            uniqueSpriteHue += 1f / Main.Instance.loops.Count; // Cycle through hues.
            uniqueSpriteHue %= 1f;
        }
    }

    public LoopSprite CreateLoopSprite(float uniqueSpriteHue) {
        GameObject newLoop = Instantiate(baseLoopSprite, Vector3.zero, Quaternion.identity,
            overlayGroup.transform);
        LoopSprite newLoopSprite = newLoop.GetComponent<LoopSprite>();
        newLoopSprite.offset = overlayStartOffset;
        newLoopSprite.ColorSprite(uniqueSpriteHue);

        displayedLoops.Add(newLoop);
        return newLoopSprite;
    }
}