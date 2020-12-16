using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TruthTableScene : MonoBehaviour
{
    public InputField input;
    public Text outputText;
    public Text labelVariableNames;
    public GameObject outputTable;
    public int itemsPerColumn;
    public Transform[] tableColumnOffsets;
    public GameObject baseTableEntry;
    private List<GameObject> displayedEntries = new List<GameObject>();

    void Start()
    {
        ClearExpression();
        UpdateTruthTable();
    }

    public void UpdateTruthTable() {
        labelVariableNames.text = "Table for " + Main.Instance.inputLength + " variable(s) "
            + VariableExpression.logicVariableAlphabet.Substring(0, Main.Instance.inputLength);

        // Make a truth table to represent the current grid state.
        for (int i = 0; i < Main.Instance.GridSize; i++) {
            bool[] variableBits = BinaryHelper.BinaryValueToBoolean(i, Main.Instance.inputLength);
            bool outcome = Main.Instance.gridState[i];

            TableEntry tableEntry = CreateTableEntry(i / itemsPerColumn);
            tableEntry.SetVariableLabels(variableBits, outcome);
            tableEntry.PositionInTable(i % itemsPerColumn);
        }
        int bitsSet = BinaryHelper.CountBitsSet(Main.Instance.gridState);
        outputText.text = $"{bitsSet} / {Main.Instance.GridSize} truthy bits";
    }

    public void ClearExpression() {
        foreach (GameObject entry in displayedEntries)
            Destroy(entry);
        displayedEntries = new List<GameObject>();
        outputText.text = "";
        labelVariableNames.text = "";
    }

    public void CallParser() {
        ClearExpression();

        try {
            GenerateTruthTable(input.text);
        }
        catch (BooleanExpressionEngine.SyntaxException ex) {
            outputText.text = "Syntax invalid: " + ex.Message;
        }
        catch (InvalidDataException ex) {
            outputText.text = ex.Message;
        }
    }

    public void GenerateTruthTable(string expression) {
        int variableCount = VariableExpression.VariableCount(expression);
        Debug.Log("Generated an expression with " + variableCount.ToString() + " variables.");

        if (variableCount == 0) {
            bool outcome = VariableExpression.GenerateTruthValue(expression, new bool[0]);

            TableEntry displayResult = CreateTableEntry(0);
            displayResult.OverrideVariableLabels("Result (identity): ", outcome);
            displayResult.PositionInTable(0);
        }
        else {
            if (variableCount >= 2)
                Main.Instance.UpdateInputLength(variableCount);

            labelVariableNames.text = "Table for " + variableCount + " variable(s) "
                + VariableExpression.logicVariableAlphabet.Substring(0, variableCount);
            int bitsSet = 0;

            // Iterate through each row / combination of the binary truth table.
            for (int i = 0; i < BinaryHelper.PowBaseTwo(variableCount); i++) {
                bool[] variableBits = BinaryHelper.BinaryValueToBoolean(i, variableCount);
                bool outcome = VariableExpression.GenerateTruthValue(expression, variableBits);

                TableEntry tableEntry = CreateTableEntry(i / itemsPerColumn);
                tableEntry.SetVariableLabels(variableBits, outcome);
                tableEntry.PositionInTable(i % itemsPerColumn);

                if (outcome)
                    bitsSet++;
                if (variableCount >= 2)
                    Main.Instance.gridState[i] = outcome;
            }
            outputText.text = $"{bitsSet} / {BinaryHelper.PowBaseTwo(variableCount)} truthy bits";
        }
    }

    public TableEntry CreateTableEntry(int column) {
        GameObject newEntry = Instantiate(baseTableEntry, Vector3.zero, Quaternion.identity,
            outputTable.transform);
        TableEntry tableEntry = newEntry.GetComponent<TableEntry>();
        tableEntry.offset = tableColumnOffsets[column];

        displayedEntries.Add(newEntry);
        return tableEntry;
    }
}
