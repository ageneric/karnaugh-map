using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class KMapInterface : MonoBehaviour
{
    public GameObject KMapLoopSprite;
    public List<GameObject> drawnLoops = new List<GameObject>();
    public GameObject overlayGroup;
    public Transform overlayStartOffset;

    public GameObject grid;
    BinaryGrid binaryGrid;

    private void Start() {
        binaryGrid = grid.GetComponent<BinaryGrid>();
    }

    public void SeedLoopPositionZero() {
        Main.Instance.loops.Add(KMapSimplify.SeedLoop(0));
    }

    public void SolveKMap() {
        Main.Instance.OnChangeInput();
        KMapSimplify.Solve();

        ResetLoopSprites();
        StartCoroutine(DrawLoopSprites());
    }

    public void ResetLoopSprites() {
        foreach (GameObject loop in drawnLoops) {
            Destroy(loop);
        }
    }

    public IEnumerator DrawLoopSprites() {
        foreach (KMapLoop loop in Main.Instance.loops) {
            yield return new WaitForEndOfFrame();
            GameObject loopSprite = Instantiate(KMapLoopSprite, Vector3.zero, Quaternion.identity,
                overlayGroup.transform);
            LoopAroundSprite loopSpritePlacement = loopSprite.GetComponent<LoopAroundSprite>();
            loopSpritePlacement.loop = loop;
            loopSpritePlacement.offset = overlayStartOffset;
            loopSpritePlacement.PositionInOverlay();
            drawnLoops.Add(loopSprite);
        }
    }

    public void LogLoops() {
        Debug.Log(Main.Instance.loops.Count.ToString() + " loop(s):");

        foreach (KMapLoop loop in Main.Instance.loops) {
            Debug.Log(loop.ToPresentableString());
        }
    }
}