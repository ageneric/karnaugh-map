using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class KMapDisplaySolution : MonoBehaviour
{
    public GameObject baseLoopSprite;
    public List<GameObject> displayedLoops = new List<GameObject>();
    public GameObject overlayGroup;
    public Transform overlayStartOffset;
    public Text loopTextSOP;
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
        drawLoopRoutine = StartCoroutine(RoutineDraw());
        TextDraw();
    }

    public void TextDraw() {
        List<string> logicSOP = new List<string>();
        foreach (KMapLoop loop in Main.Instance.loops)
            logicSOP.Add(loop.ToProductsString());
        loopTextSOP.text = string.Join(" + ", logicSOP);
    }

    public IEnumerator RoutineDraw() {
        float uniqueSpriteHue = Random.value;

        foreach (KMapLoop loop in Main.Instance.loops) {
            yield return new WaitForEndOfFrame();
            GameObject loopSprite = Instantiate(baseLoopSprite, Vector3.zero, Quaternion.identity,
                overlayGroup.transform);
            LoopAroundSprite loopSpritePlacement = loopSprite.GetComponent<LoopAroundSprite>();
            loopSpritePlacement.loop = loop;
            loopSpritePlacement.offset = overlayStartOffset;
            List<int[]> wrapAround = loopSpritePlacement.PositionInOverlay();
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