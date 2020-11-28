using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryGrid : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;

    public GameObject[] gridToggleButton;
    public GameObject toggleBitButton;
    public GameObject gridGroup;

    // Start is called before the first frame update
    void Start()
    {
        StartInitialiseGrid();
        ResetGrid(Main.Instance.inputLength);
    }
    
    public void StartInitialiseGrid() {
        Vector3 zeroPosition = new Vector3(0, 0, 0);

        // Generate 16 buttons and add to grid. Call once on Start().
        for (int i = 0; i<16; i++) {
            GameObject toggleButton = Instantiate(toggleBitButton, zeroPosition, Quaternion.identity,
                gridGroup.transform);
            toggleButton.GetComponent<ToggleBit>().index = i;
        }
    }

    public int GridLength() {
        return gridWidth * gridHeight;
    }


    public void ResetGrid(int inputLength) {
        switch (inputLength) {
            case 2:
                gridWidth = 2;
                gridHeight = 2;
                break;
            case 3:
                gridWidth = 4;
                gridHeight = 2;
                break;
            case 4:
                gridWidth = 4;
                gridHeight = 4;
                break;
            default:
                gridWidth = 2;
                gridHeight = 2;
                Debug.LogWarning("Invalid [ResetGrid(input_dim)] binary input size, out of range (2 - 4).");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(BinaryHelper.GrayCode(2));
    }
}
