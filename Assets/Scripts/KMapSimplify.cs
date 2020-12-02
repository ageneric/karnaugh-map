using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapSimplify
{
    // Simplify the k-map by finding "loops" in the binary grid.

    public static void SeedLoop(int startIndex) {
        // Create a 1x1 loop containing the index (which should represent a true truth-table bit).
        KMapLoop loop = new KMapLoop();

        foreach (bool bit in BinaryHelper.BinaryValueToBoolean(startIndex, Main.Instance.inputLength)) {
            loop.Add(new List<bool>() { bit });
        }
        Main.Instance.loops.Add(loop);
    }

    public static List<int> UnpackLoop(KMapLoop loop) {
        // Inefficient method to list all the tiles in any size loop.
        List<int> tileLists = new List<int>();
        
        for (int gridIndex = 0; gridIndex < Main.Instance.GridSize; gridIndex++) {
            // Determine if the grid tile's binary index is contained within
            // the Cartesian product of the loop lists.
            bool tileIsContained = true;
            for (int bitChecked = 0; bitChecked < Main.Instance.inputLength; bitChecked++) {
                if (!loop[bitChecked].Contains(BinaryHelper.BitIsSet(gridIndex, bitChecked))) {
                    tileIsContained = false;
                }
            }
            if (tileIsContained) {
                tileLists.Add(gridIndex);
            }
        }
        return tileLists;
    }

    public static KMapLoop AdjacentLoop(KMapLoop startLoop, int flipBit) {
        KMapLoop adjacentLoop = new KMapLoop();

        for (int copyBit = 0; copyBit < Main.Instance.inputLength; copyBit++) {
            List<bool> adjacentCombination = new List<bool>(startLoop[copyBit]);
            if (flipBit == copyBit) {
                adjacentCombination[0] = !adjacentCombination[0];
            }
            adjacentLoop.Add(adjacentCombination);
        }
        return adjacentLoop;
    }
}
