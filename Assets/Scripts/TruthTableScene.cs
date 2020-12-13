using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TruthTableScene : MonoBehaviour
{
    public InputField input;
    public Text outputText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClearExpression() {
        outputText.text = "";
    }

    public void CallParser() {
        try {
            bool final = ParseExpression.GenerateTruthValue(input.text);
            outputText.text = final.ToString();
        }
        catch (BooleanExpressionEngine.SyntaxException) {
            outputText.text = "invalid syntax";
        }
        catch (InvalidDataException) {
            outputText.text = "invalid token";
        }
    }
}
