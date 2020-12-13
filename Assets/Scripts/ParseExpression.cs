using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParseExpression
{
    // Alphabet for input bit names (doesn't include I, Q).
    public static string logicVariableAlphabet = "ABCDEFGHJKLMNOPRSTUVWXYZ";

    public static int VariableCount(string expression) {
        // Set the input dimension based on the highest letter found in the expression.
        // Accept only the first 5 bits, ABCDE.
        Debug.Log(expression);
        for (int i = 4; i >= 0; i--) {
            Debug.Log(logicVariableAlphabet[i]);
            if (expression.Contains(logicVariableAlphabet[i].ToString()))
                return i + 1;
        }
        return 0;
    }

    public static bool[] GenerateTruthTable(string expression) {
        int variableCount = VariableCount(expression);
        bool[] truthTable = new bool[BinaryHelper.PowBaseTwo(variableCount)];
        Debug.Log(variableCount);
        if (variableCount >= 2)
            Main.Instance.UpdateInputLength(variableCount);

        // Iterate through each row / combination of the binary truth table.
        for (int i = 0; i < BinaryHelper.PowBaseTwo(variableCount); i++) {
            
            bool outcome = GenerateTruthValue(expression,
                BinaryHelper.BinaryValueToBoolean(i, variableCount));
            truthTable[i] = outcome;

            if (variableCount >= 2)
                Main.Instance.gridState[i] = outcome;
        }
        return truthTable;
    }

    public static bool GenerateTruthValue(string expression, bool[] truthTableBits) {
        // Create the dictionary of variable names to their truth table values.
        Debug.Log(string.Join(", ", truthTableBits));
        Dictionary<char, bool> variables = new Dictionary<char, bool>();
        for (int j = 0; j < truthTableBits.Length; j++) {
            variables.Add(logicVariableAlphabet[j], truthTableBits[j]);
        }
        Debug.Log(variables);
        Debug.Log(string.Join(", ", variables.Keys));
        Debug.Log(string.Join(", ", variables.Values));
        Debug.Log(variables['A']);
        // Parse the expression with the variable values.
        return BooleanExpressionEngine.Parser.Parse(expression).Eval(variables);
    }
}
