using System.Collections.Generic;
using System.Text;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapLogging
{
    public static void LogLoopList(string message, List<KMapLoop> loopList) {
        StringBuilder debugText = new StringBuilder();
        debugText.Append(message);
        debugText.Append(" (" + loopList.Count.ToString() + "): ");

        foreach (KMapLoop loop in loopList) {
            debugText.Append(loop.ToReadableString());
            debugText.Append("; ");
        }
        Debug.Log(debugText.ToString());
    }
}
