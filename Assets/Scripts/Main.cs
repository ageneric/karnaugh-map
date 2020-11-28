using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }

    public bool[] gridState;
    public int inputLength = 2;  // Number of input variable bits (input dimension).

    [RuntimeInitializeOnLoadMethod]
    public void Awake() {
        if (Instance == null) {
            Instance = this;

            int grid_size = (int)Mathf.Pow(2, inputLength);
            gridState = new bool[grid_size];
            Debug.Log("Initialised main data.");
        }
        else {
            Debug.Log("Second call to create multiple [Main] instances.");
        }
    }
}
