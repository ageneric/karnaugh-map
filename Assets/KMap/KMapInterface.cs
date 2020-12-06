using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class KMapInterface : MonoBehaviour
{
    public void SeedLoopPositionZero() {
        Main.Instance.loops.Add(KMapSimplify.SeedLoop(0));
    }

    public void SolveKMap() {
        KMapSimplify.Solve();
    }

    public void LogLoops() {
        Debug.Log(Main.Instance.loops.Count.ToString() + " loop(s):");

        foreach (KMapLoop loop in Main.Instance.loops) {
            Debug.Log(loop.ToPresentableString());
        }
    }
}
