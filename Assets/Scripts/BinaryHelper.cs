using System;
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

    public static bool BitIsSet(int binary, int index) {
        int maskedBinary = binary & (1 << index);
        return maskedBinary != 0;
    }

    public static int PowBaseTwo(int exponent) {
        return 1 << exponent;
    }

    public static bool[] ValueToBitArray(int binary) {
        List<bool> bits = new List<bool>();

        while (binary > 0) {
            bits.Add(binary % 2 != 0);
            binary >>= 1;
        }
        bits.Reverse();
        return bits.ToArray();
    }

    public static int BitArrayToValue(bool[] bits) {
        int totalBinary = 0;
        Array.Reverse(bits, 0, bits.Length);
        
        for (int i = 0; i<bits.Length; i++) {
            if (bits[i])
                totalBinary += BinaryHelper.PowBaseTwo(i);
        }
        return totalBinary;
    }
}
