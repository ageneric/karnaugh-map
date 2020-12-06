using System.Collections.Generic;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapExtensionMethods
{
    public static string ToPresentableString(this KMapLoop loop) {
        List<string> loopStrings = new List<string>();
        foreach (List<bool> logicIncludeList in loop) {

            string representation;
            if (logicIncludeList.Count == 1) {
                if (logicIncludeList[0] == true)
                    representation = "1";
                else
                    representation = "0";
            }
            else {
                representation = "X";
            }
            loopStrings.Add(string.Join("", representation));
        }
        return "{" + string.Join(", ", loopStrings) + "}";
    }
}
