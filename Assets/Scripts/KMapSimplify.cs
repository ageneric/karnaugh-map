using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapSimplify
{
    // Simplify the k-map by finding "loops" in the binary grid.

    public static void Solve() {
        for (int index = 0; index < Main.Instance.GridSize; index++) {
            // For each bit that is set, form loops.
            if (Main.Instance.gridState[index]) {
                KMapLoop startLoop = SeedLoop(index);

                // Expand the loop -> Adds loops to Main.Instance.loops;
            }
        }
    }

    public static KMapLoop SeedLoop(int startIndex) {
        // Create a 1x1 loop containing the index (which should represent a true truth-table bit).
        KMapLoop loop = new KMapLoop();

        foreach (bool bit in BinaryHelper.BinaryValueToBoolean(startIndex, Main.Instance.inputLength)) {
            loop.Add(new List<bool>() { bit });
        }
        return loop;
    }

    public static void ExpandLoop(KMapLoop startLoop, int minimumIndex) {
        List<KMapLoop> currentLoops = new List<KMapLoop>();
        // For each flippable bit (neighbouring tile).
            // Create the neighbouring loop.
            // Unpack into tiles contained in the neighbouring loop.
            // If all those tiles are set to 1, can merge together into larger loop.

        // If any loops were created remove the original loop.
        // Append remaining loops to loop list.
        Main.Instance.loops.AddRange(currentLoops);
    }

    public static List<int> UnpackLoop(KMapLoop loop) {
        // Inefficient method to list all the tiles in any size loop.
        List<int> tileLists = new List<int>();
        
        for (int gridIndex = 0; gridIndex < Main.Instance.GridSize; gridIndex++) {
            // Determine if the grid tile's binary index is contained within the tiles
            // represented, which are found using the cross product of all combinations.
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

    // Taken from https://ericlippert.com/2010/06/28/computing-a-cartesian-product-with-linq/.
    // Will replace UnpackLoop() after testing.
    public static IEnumerable<IEnumerable<bool>> CrossProductCombinations(IEnumerable<IEnumerable<bool>> sequences) {
        // Base case {} - if no sequences are provided.
        IEnumerable<IEnumerable<bool>> defaultEmpty = new[] { Enumerable.Empty<bool>() };
        return sequences.Aggregate(
            defaultEmpty,
            (accumulator, sequence) =>
                from combination in accumulator
                from includeValue in sequence
                select combination.Concat(new[] { includeValue })
        );
    }
}
