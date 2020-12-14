using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class KMapDisplaySolution : MonoBehaviour
{
    public GameObject baseLoopSprite;
    private List<GameObject> displayedLoops = new List<GameObject>();
    public GameObject overlayGroup;
    public Transform overlayStartOffset;
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

    public void QueueDraw() {
        if (!(drawLoopRoutine is null))
            StopCoroutine(drawLoopRoutine);
        loopTextSOP.text = "";
        drawLoopRoutine = StartCoroutine(RoutineDraw());
    }

    public void TextDraw(float hue=0f, bool useHue=false) {
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
                hue += 1f / Main.Instance.loops.Count;
                hue %= 1f;
            }
        }
        loopTextSOP.text = styledExpression.ToString();
    }

    public IEnumerator RoutineDraw() {
        float uniqueSpriteHue = Random.value;
        TextDraw(uniqueSpriteHue, true);

        foreach (KMapLoop loop in Main.Instance.loops) {
            yield return new WaitForEndOfFrame();

            GameObject loopSprite = Instantiate(baseLoopSprite, Vector3.zero, Quaternion.identity,
                overlayGroup.transform);
            LoopAroundSprite loopSpritePlacement = loopSprite.GetComponent<LoopAroundSprite>();

            loopSpritePlacement.offset = overlayStartOffset;
            List<int[]> wrapAround = loopSpritePlacement.PositionInOverlay(loop);
            loopSpritePlacement.ColorSprite(uniqueSpriteHue);
            displayedLoops.Add(loopSprite);

            foreach (int[] position in wrapAround) {
                GameObject cloneSprite = Instantiate(baseLoopSprite, Vector3.zero, Quaternion.identity,
                    gameObject.transform);
                LoopAroundSprite cloneSpritePlacement = cloneSprite.GetComponent<LoopAroundSprite>();

                cloneSpritePlacement.offset = overlayStartOffset;
                cloneSpritePlacement.SetPosition(position[0], position[1], position[2], position[3]);
                cloneSpritePlacement.ColorSprite(uniqueSpriteHue);
                displayedLoops.Add(cloneSprite);
            }
            uniqueSpriteHue += 1f / Main.Instance.loops.Count;
            uniqueSpriteHue %= 1f;
        }
    }

    public void LogLoops() {
        Debug.Log(Main.Instance.loops.Count.ToString() + " loop(s):");

        foreach (KMapLoop loop in Main.Instance.loops) {
            Debug.Log(loop.ToReadableString());
        }
    }
}