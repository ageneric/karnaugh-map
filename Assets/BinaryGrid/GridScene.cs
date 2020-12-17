using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GridScene : MonoBehaviour
{
    public BinaryGrid grid;
    public DisplayLoopSolution loopSpriteOverlay;
    public Text[] labelTextHorizontal;
    public Text[] labelTextVertical;
    public Text labelVariableHorizontal;
    public Text labelVariableVertical;
    public GameObject customLengthInput;

    void Start() {
        StartCoroutine(WaitForGridUpdateLabels());

        if (BinaryHelper.CountBitsSet(Main.Instance.gridState) > 0 && Main.Instance.loops.Count == 0)
            KMapSolve.Solve();

        if (Main.Instance.inputLength <= 4) {
            customLengthInput.SetActive(false);
            loopSpriteOverlay.Draw();
        }
        else {
            customLengthInput.SetActive(true);
            loopSpriteOverlay.TextDraw();
        }
    }

    public IEnumerator WaitForGridUpdateLabels() {
        yield return new WaitWhile(() => grid.gridWidth >= 2);
        UpdateLabels();
    }

    public void UpdateInputLength(int newInputLength) {
        Main.Instance.UpdateInputLength(newInputLength);
        loopSpriteOverlay.Clear();
        grid.ResetGrid();
        UpdateLabels();

        if (newInputLength <= 4)
            customLengthInput.SetActive(false);
        else
            customLengthInput.SetActive(true);
    }

    public void UpdateInput() {
        Main.Instance.ClearLoops();
        loopSpriteOverlay.Clear();
    }

    public void Solve() {
        Main.Instance.ClearLoops();
        KMapSolve.Solve();

        loopSpriteOverlay.Clear();
        if (Main.Instance.inputLength <= 4)
            loopSpriteOverlay.Draw();
        else
            loopSpriteOverlay.TextDraw();
    }

    void UpdateLabels() {
        int bitsHorizontal = Mathf.FloorToInt(Mathf.Log(grid.gridWidth, 2));
        int bitsVertical = Mathf.FloorToInt(Mathf.Log(grid.gridHeight, 2));
        bool labelGrid = Main.Instance.inputLength >= 2 && Main.Instance.inputLength <= 4;

        // Update the column / row labels (length 2: 0 1, length 4: 00 01 11 10).
        for (int i = 0; i < 4; i++) {
            if (labelGrid && i < grid.gridWidth) {
                string label = Convert.ToString(BinaryHelper.GrayCode(i), 2).PadLeft(bitsHorizontal, '0');
                labelTextHorizontal[i].gameObject.SetActive(true);
                labelTextHorizontal[i].text = label;
            }
            else {
                labelTextHorizontal[i].gameObject.SetActive(false);
            }

            if (labelGrid && i < grid.gridHeight) {
                string label = Convert.ToString(BinaryHelper.GrayCode(i), 2).PadLeft(bitsVertical, '0');
                labelTextVertical[i].gameObject.SetActive(true);
                labelTextVertical[i].text = label;
            }
            else {
                labelTextVertical[i].gameObject.SetActive(false);
            }
        }

        if (labelGrid) {
            labelVariableVertical.text = VariableExpression.logicVariableAlphabet.Substring(0, bitsVertical);
            labelVariableHorizontal.text = VariableExpression.logicVariableAlphabet.Substring(bitsVertical, bitsHorizontal);
        }
        else if (Main.Instance.inputLength >= 1 && Main.Instance.inputLength <= VariableExpression.logicVariableAlphabet.Length) {
            labelVariableHorizontal.text = VariableExpression.logicVariableAlphabet.Substring(0, Main.Instance.inputLength);
            labelVariableVertical.text = "";
        }
    }
}
