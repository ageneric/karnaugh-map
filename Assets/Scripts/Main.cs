using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }

    // Number of input variable bits (input dimension); supported range 2-4.
    public int inputLength = 2;
    public bool[] gridState = new bool[0];

    // Each loop is represented as an array of length inputLength.
    // Each item represents all the possible states for a single bit
    // of the index (in binary) for all tiles found within the loop.
    public List<List<bool>[]> loops;
    // For example; [{0}, {0, 1}, {1}] is the loop that contains both {001} and {011}.

    public int GridSize {
        // Get the expected length of the grid array.
        get => BinaryHelper.PowBaseTwo(inputLength);
    }

    [RuntimeInitializeOnLoadMethod]
    public void Awake() {
        if (Instance == null) {
            Instance = this;
            Debug.Log("Initialised main data.");
        }
        else {
            Debug.Log("Second call to create multiple [Main] instances.");
        }
    }

    public void UpdateInputLength(int newInputLength) {
        inputLength = newInputLength;
        gridState = new bool[GridSize];
        loops = new List<List<bool>[]>();
    }
}
