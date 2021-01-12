using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableEntry : MonoBehaviour
{
    [EditorReadOnly] public RectTransform offset;
    public Text[] variableBitLabels;
    public Text resultText;
    public int entrySpacing;

    public void SetVariableLabels(bool[] variableBits, bool outcome) {
        for (int i = 0; i < variableBitLabels.Length; i++) {
            if (variableBits.Length > i) {
                if (variableBits[i])
                    variableBitLabels[i].text = "1";
                else
                    variableBitLabels[i].text = "0";
            }
            else {
                variableBitLabels[i].gameObject.SetActive(false);
            }
        }

        if (outcome)
            resultText.text = "1";
        else
            resultText.text = "0";
    }

    public void OverrideVariableLabels(string firstLabel, bool result) {
        for (int i = 0; i < variableBitLabels.Length; i++) {
            if (i == 0)
                variableBitLabels[i].text = firstLabel;
            else
                variableBitLabels[i].gameObject.SetActive(false);
        }

        if (result)
            resultText.text = "1";
        else
            resultText.text = "0";
    }

    public void PositionInTable(int row) {
        // Column offset -ve: Increasing indexes appear top-to-bottom, i.e. decreasing y.
        Vector2 tablePosition = new Vector3(0f, -row) * entrySpacing;
        gameObject.GetComponent<RectTransform>().anchoredPosition = tablePosition + offset.anchoredPosition;
    }
}
