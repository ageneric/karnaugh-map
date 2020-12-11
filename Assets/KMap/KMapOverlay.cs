using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class KMapOverlay : MonoBehaviour
{
    public GameObject baseLoopSprite;
    public List<GameObject> displayedLoops = new List<GameObject>();
    public GameObject overlayGroup;
    public Transform overlayStartOffset;
    private Coroutine drawLoopRoutine;

    public void Clear() {
        foreach (GameObject loop in displayedLoops)
            Destroy(loop);
        displayedLoops = new List<GameObject>();
    }

    public void QueueDraw() {
        if (!(drawLoopRoutine is null))
            StopCoroutine(drawLoopRoutine);
        drawLoopRoutine = StartCoroutine(RoutineDraw());
    }

    public IEnumerator RoutineDraw() {
        foreach (KMapLoop loop in Main.Instance.loops) {
            yield return new WaitForEndOfFrame();

            GameObject loopSprite = Instantiate(baseLoopSprite, Vector3.zero, Quaternion.identity,
                overlayGroup.transform);
            LoopAroundSprite loopSpritePlacement = loopSprite.GetComponent<LoopAroundSprite>();
            loopSpritePlacement.loop = loop;
            loopSpritePlacement.offset = overlayStartOffset;
            loopSpritePlacement.PositionInOverlay();
            displayedLoops.Add(loopSprite);
        }
    }

    public void LogLoops() {
        Debug.Log(Main.Instance.loops.Count.ToString() + " loop(s):");

        foreach (KMapLoop loop in Main.Instance.loops) {
            Debug.Log(loop.ToReadableString());
        }
    }
}