using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryGrid : MonoBehaviour
{
    [EditorReadOnly] public int gridWidth;
    [EditorReadOnly] public int gridHeight;

    public GameObject[] poolToggleButtons;
    public GameObject toggleBitButton;
    public List<GameObject> gridToggleButtons;
    public GameObject gridGroup;
    public Transform gridStartOffset;

    // Start is called before the first frame update
    void Start()
    {
        StartInitialiseGrid();
        ResetGrid(Main.Instance.inputLength);
    }

    public void StartInitialiseGrid() {
        // Generate 16 buttons and add to the pool of available button objects.
        for (int i = 0; i<16; i++) {
            GameObject toggleButton = Instantiate(toggleBitButton, Vector3.zero, Quaternion.identity,
                gridGroup.transform);
            ToggleBit toggleBit = toggleButton.GetComponent<ToggleBit>();
            toggleBit.index = i;
            toggleBit.offset = gridStartOffset;
            gridToggleButtons.Add(toggleButton);
        }

        poolToggleButtons = gridToggleButtons.ToArray();
    }

    public void ResetGrid(int inputLength) {
        Main.Instance.UpdateInputLength(inputLength);

        switch (inputLength) {
            case 2:
                gridWidth = 2; gridHeight = 2; break;
            case 3:
                gridWidth = 4; gridHeight = 2; break;
            case 4:
                gridWidth = 4; gridHeight = 4; break;
            default:
                Debug.LogWarning("Invalid [ResetGrid(input_dim)] binary input size, out of range (2 - 4).");
                gridWidth = 2; gridHeight = 2; break;
        }

        // Enable and reset only the needed buttons. Deactivate unused buttons.
        gridToggleButtons = new List<GameObject>();
        foreach (GameObject toggleButton in poolToggleButtons) {
            ToggleBit toggleBit = toggleButton.GetComponent<ToggleBit>();

            if (toggleBit.index < Main.Instance.GridSize) {
                toggleButton.SetActive(true);
                toggleBit.Reset();
                toggleBit.PositionInGrid(gridWidth);
                gridToggleButtons.Add(toggleButton);
            }
            else {
                toggleButton.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
