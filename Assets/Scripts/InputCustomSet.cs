using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InputCustomSet : MonoBehaviour
{
    public InputField input;
    public Text infoText;
    public GridScene sceneManager;

    private void Start() {
        int bitsSet = 0;
        foreach (bool state in Main.Instance.gridState) {
            if (state) bitsSet++;
        }
        infoText.text = "total " + bitsSet.ToString();
    }

    public void UpdateBitsWithInput() {
        sceneManager.UpdateInput();
        Main.Instance.gridState = new bool[Main.Instance.GridSize];

        string inputString = input.text;
        string inputCSV = Regex.Replace(inputString, @"\s+", "");
        string[] cellIndexes = inputCSV.Split(';', ',');

        foreach (string cellIndex in cellIndexes) {
            try {
                int gridIndex = Int32.Parse(cellIndex);
                Main.Instance.gridState[gridIndex] = true;
            }
            catch (FormatException) {
                infoText.text = "invalid format";
                return;
            }
            catch (IndexOutOfRangeException) {
                infoText.text = "index out of range (use 0-31)";
                return;
            }
        }
        infoText.text = "total " + cellIndexes.Length.ToString();
    }
}
