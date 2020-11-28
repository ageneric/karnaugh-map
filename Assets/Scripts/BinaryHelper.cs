using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BinaryHelper
{
    public static int GrayCode(int index) {
        switch (index) {
            case 0:
                return 0;
            case 1:
                return 1;
            case 2:
                return 3;
            case 3:
                return 2;
            default:
                Debug.LogWarning("Invalid [GrayCode(index)] index, out of range (0 - 3).");
                return 0;
        }
    }
}
