using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GridCustomInput : MonoBehaviour
{
    public InputField input;
    public Text infoText;
    public GridScene sceneManager;

    void Start() {
        DisplayBitCount();
    }

    public void DisplayBitCount() {
        infoText.text = BinaryHelper.CountBitsSet(Main.Instance.gridState).ToString() + " bits set";
    }

    public void UpdateBitsWithInput() {
        sceneManager.UpdateInput();
        Main.Instance.gridState = new bool[Main.Instance.GridSize];

        string inputCSV = Regex.Replace(input.text, @"\s+", "");
        string[] cellIndexes = inputCSV.Split(';', ',');

        foreach (string cellIndex in cellIndexes) {
            try {
                int gridIndex = Int32.Parse(cellIndex);

                if (gridIndex >= 0 && gridIndex < Main.Instance.GridSize) {
                    Main.Instance.gridState[gridIndex] = true;
                }
                else {
                    infoText.text = "index out of range (use 0-31)";
                    return;
                }
            }
            catch (FormatException) {
                infoText.text = "invalid format";
                return;
            }
        }
        infoText.text = cellIndexes.Length.ToString() + " bits set";
    }
}
