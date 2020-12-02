using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BinaryHelper
{
    public static int PowBaseTwo(int exponent) {
        return 1 << exponent;
    }

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

    public static bool BitIsSet(int value, int index) {
        int maskedBinary = value & (1 << index);
        return maskedBinary != 0;
    }

    public static bool[] BinaryValueToBoolean(int value, int length) {
        List<bool> bits = new List<bool>();

        for (int i = 0; i < length; i++) {
            bits.Add(value % 2 != 0);
            value >>= 1;
        }
        if (value > 0) {
            Debug.LogWarning("Invalid [...Boolean(length)] binary length, too small to contain value.");
        }
        bits.Reverse();
        return bits.ToArray();
    }

    public static int BooleanToBinaryValue(bool[] bits) {
        int totalBinary = 0;
        Array.Reverse(bits, 0, bits.Length);
        
        for (int i = 0; i < bits.Length; i++) {
            if (bits[i])
                totalBinary += PowBaseTwo(i);
        }
        return totalBinary;
    }
}
