using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryGrid : MonoBehaviour
{
    [EditorReadOnly] public int gridWidth;
    [EditorReadOnly] public int gridHeight;

    private GameObject[] poolToggleButtons;
    public GameObject gridGroup;
    public RectTransform gridStartOffset;
    public GameObject toggleBitButton;
    private List<GameObject> displayedToggleButtons;
    [Space]
    public GridScene sceneManager;

    void Start()
    {
        StartInitialiseGrid();
        ResetGrid();
    }

    public void StartInitialiseGrid() {
        displayedToggleButtons = new List<GameObject>();

        // Generate 16 buttons and add to the pool of available button objects.
        for (int i = 0; i<16; i++) {
            GameObject toggleButton = Instantiate(toggleBitButton, Vector3.zero, Quaternion.identity,
                gridGroup.transform);
            BinaryToggle toggleBit = toggleButton.GetComponent<BinaryToggle>();
            toggleBit.index = i;
            toggleBit.offset = gridStartOffset;
            toggleBit.sceneManager = sceneManager;
            displayedToggleButtons.Add(toggleButton);
        }

        poolToggleButtons = displayedToggleButtons.ToArray();
    }

    public void ResetGrid() {
        switch (Main.Instance.inputLength) {
            case 2:
                gridWidth = 2; gridHeight = 2; break;
            case 3:
                gridWidth = 4; gridHeight = 2; break;
            case 4:
                gridWidth = 4; gridHeight = 4; break;
            default:
                Debug.LogWarning("Invalid [ResetGrid(input_dim)] binary input size, out of range (2 - 4). Hidden grid.");
                gridWidth = 2; gridHeight = 2;
                foreach (GameObject toggleButton in poolToggleButtons) {
                    toggleButton.SetActive(false);
                }
                return;
        }

        // Enable and reset only the needed buttons. Deactivate unused buttons.
        displayedToggleButtons = new List<GameObject>();
        foreach (GameObject toggleButton in poolToggleButtons) {
            BinaryToggle toggleBit = toggleButton.GetComponent<BinaryToggle>();

            if (toggleBit.index < Main.Instance.GridSize) {
                toggleBit.Reset();
                toggleBit.PositionInGrid(gridWidth);
                toggleButton.SetActive(true);
                displayedToggleButtons.Add(toggleButton);
            }
            else {
                toggleButton.SetActive(false);
            }
        }
    }
}
