using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class KMapDrawLoop : MonoBehaviour
{
    public void SeedLoopPositionZero() {
        KMapSimplify.SeedLoop(0);
    }

    public void LogLoops() {
        Debug.Log(Main.Instance.loops.Count.ToString() + " loop(s):");

        foreach (KMapLoop loop in Main.Instance.loops) {

            List<string> loopStrings = new List<string>();
            foreach (List<bool> combination in loop) {

                string representation;
                if (combination.Count == 1) {
                    if (combination[0] == true)
                        representation = "1";
                    else
                        representation = "0";
                }
                else {
                    representation = "X";
                }
                loopStrings.Add(string.Join("", representation));
            }
            Debug.Log("{" + string.Join(", ", loopStrings) + "}");
        }
    }
}
