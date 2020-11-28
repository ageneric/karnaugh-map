using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBit : MonoBehaviour
{
    public int index = 0;

    public void Toggle() {
        Main.Instance.gridState[index] = !Main.Instance.gridState[index];
    }
}
