using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TruthTableScene : MonoBehaviour
{
    public InputField input;
    public Text outputText;
    public Text variablesText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClearExpression() {
        outputText.text = "";
        variablesText.text = "";
    }

    public void CallParser() {
        ClearExpression();
        try {
            bool[] truthTable = ParseExpression.GenerateTruthTable(input.text);

            outputText.text = string.Join("\n", truthTable);

            List<string> variableColumnText = new List<string>();
            for (int i = 0; i < truthTable.Length; i++) {
                variableColumnText.Add(i.ToString());
            }
            variablesText.text = string.Join("\n", variableColumnText);
        }
        catch (BooleanExpressionEngine.SyntaxException ex) {
            outputText.text = "Syntax invalid: " + ex.Message;
        }
        catch (InvalidDataException ex) {
            outputText.text = ex.Message;
        }
    }
}
