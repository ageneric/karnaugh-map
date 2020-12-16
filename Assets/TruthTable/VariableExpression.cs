using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VariableExpression
{
    // Alphabet for input bit names (doesn't include I, Q).
    public static string logicVariableAlphabet = "ABCDEFGHJKLMNOPRSTUVWXYZ";

    public static int VariableCount(string expression) {
        // Set the input dimension based on the highest letter found in the expression.
        // Accept only the first 5 bits, ABCDE.
        for (int i = 4; i >= 0; i--) {
            if (expression.Contains(logicVariableAlphabet[i].ToString()))
                return i + 1;
        }
        return 0;
    }

    public static bool GenerateTruthValue(string expression, bool[] truthTableBits) {
        // Create the dictionary of variable names to their truth table values.
        Dictionary<char, bool> variables = new Dictionary<char, bool>();
        for (int j = 0; j < truthTableBits.Length; j++) {
            variables.Add(logicVariableAlphabet[j], truthTableBits[j]);
        }
        // Parse the expression with the variable values.
        return BooleanExpressionEngine.Parser.Parse(expression).Eval(variables);
    }
}
