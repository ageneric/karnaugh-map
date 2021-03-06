﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class BinaryToggle : MonoBehaviour
{
    // Represents a single binary bit in the k-map's grid state.

    [EditorReadOnly] public int index;
    [EditorReadOnly] public RectTransform offset;
    public float positionScale;
    public Text label;
    public Toggle toggle;
    [EditorReadOnly] public GridScene sceneManager;

    public void Toggle() {
        Main.Instance.gridState[index] = !Main.Instance.gridState[index];
        UpdateDisplay();
        sceneManager.UpdateInput();
    }

    public void Reset() {
        toggle.SetIsOnWithoutNotify(Main.Instance.gridState[index]);
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        if (Main.Instance.gridState[index]) {
            label.text = index.ToString() + ":1";
        }
        else {
            label.text = index.ToString() + ":0";
        }
    }

    public void PositionInGrid(int wrapAtColumn) {
        int trueRow;
        int trueColumn = Math.DivRem(index, wrapAtColumn, out trueRow);
        int row = BinaryHelper.GrayCode(trueRow);
        int column = BinaryHelper.GrayCode(trueColumn);

        // Column offset -ve: Increasing indexes appear top-to-bottom, i.e. decreasing y.
        Vector2 gridPosition = new Vector2(row, -column) * positionScale;
        gameObject.GetComponent<RectTransform>().anchoredPosition = gridPosition + offset.anchoredPosition;
    }
}
