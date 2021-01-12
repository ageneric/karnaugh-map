using System.Collections.Generic;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }

    // Number of input variable bits (input dimension); supported range 2-4.
    public int inputLength;
    public int GridSize {
        // Get the expected length of the grid array.
        get => BinaryHelper.PowBaseTwo(inputLength);
    }
    public bool[] gridState;

    // Holds all loops. Each loop is a list of length inputLength, where
    // each item represents all of the states for that single bit
    // of the index in binary, as found for all cells within the loop.
    public List<KMapLoop> loops;
    // For example; [{0}, {0, 1}, {1}] is the loop that contains both {001} and {011}.

    // The original expression entered by the user (using the input field).
    public string inputExpression;

    [RuntimeInitializeOnLoadMethod]
    public void Awake() {
        if (Instance is null) {
            Instance = this;
            UpdateInputLength(2);
        }
        else {
            Debug.LogWarning("Attempted to create multiple Main instances.");
        }
    }

    public void UpdateInputLength(int newInputLength) {
        inputLength = newInputLength;
        gridState = new bool[GridSize];
        loops = new List<KMapLoop>();
        inputExpression = "";
    }

    public void ClearLoops() {
        if (loops.Count > 0)
            loops = new List<KMapLoop>();
    }
}
