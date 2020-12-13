using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParseExpression
{
    // Alphabet for input bit names (doesn't include I, Q).
    public static string logicVariableAlphabet = "ABCDEFGHJKLMNOPRSTUVWXYZ";

    public static bool GenerateTruthValue(string text) {
        Dictionary<char, bool> variableBits = new Dictionary<char, bool>();

        return BooleanExpressionEngine.Parser.Parse(text).Eval(variableBits);
    }
}
