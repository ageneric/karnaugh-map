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

    public static int ExtendedGrayCode(int index) {
        int grayValue = 0;
        int exponent = 0;

        while (index > 0) {
            index = Math.DivRem(index, 4, out int remainder);
            grayValue += GrayCode(remainder) * PowBaseTwo(exponent);
            exponent += 2;
        }
        return grayValue;
    }

    public static List<int> DiagonalWeave(int gridSize) {
        // 0, 3, 5, 6, 12, 15...
        // Lists grid indexes in a checkerboard pattern as viewed on a k-map.
        // Every index in each checkerboard is separated diagonally from the other indexes.
        List<int> primaryDiagonalList = new List<int>();
        List<int> secondaryDiagonalList = new List<int>();

        for (int i = 0; i < gridSize; i++) {
            // Alternate adding cells to the first / second checkerboards.
            if ((i % 2 == 0) ^ BitIsSet(i, 2)) {
                primaryDiagonalList.Add(ExtendedGrayCode(i));
            }
            else {
                secondaryDiagonalList.Add(ExtendedGrayCode(i));
            }
        }

        primaryDiagonalList.AddRange(secondaryDiagonalList);
        return primaryDiagonalList;
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
        bits.Reverse();

        if (value > 0)
            Debug.LogWarning("Invalid [...Boolean(length)] binary length, too small to contain value.");

        return bits.ToArray();
    }

    public static int BooleanToBinaryValue(bool[] bits) {
        int totalBinary = 0;
        Array.Reverse(bits);
        
        for (int i = 0; i < bits.Length; i++) {
            if (bits[i])
                totalBinary += PowBaseTwo(i);
        }
        return totalBinary;
    }
}
