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
                ExpandLoop(startLoop, index);
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
        bool isMaximum = true;

        // For each flippable bit (neighbouring tile).
        for (int flipBit = 0; flipBit < Main.Instance.inputLength; flipBit++) {
            if (startLoop[flipBit].Count == 2) 
                continue;

            // Create the neighbouring loop.
            KMapLoop neighbour = AdjacentLoop(startLoop, flipBit);
            Debug.Log("Neighbour: " + neighbour.ToPresentableString());

            // Unpack into tiles contained in the neighbouring loop.
            List<int> neighbourTileList = UnpackLoop(neighbour);
            Debug.Log("Checking tiles: " + string.Join(", ", neighbourTileList));

            // If all those tiles are set to 1, can merge together into larger loop.
            if (neighbourTileList.All(tileIndex => Main.Instance.gridState[tileIndex])) {
                KMapLoop mergeLoop = MergeAdjacent(startLoop, neighbour);
                Debug.Log("Merged: " + mergeLoop.ToPresentableString());

                currentLoops.Add(mergeLoop);
                isMaximum = false;
            }
        }

        // Append remaining to loop list (add original only if it can't be expanded).
        if (isMaximum) {
            Main.Instance.loops.Add(startLoop);
        }
        else {
            Main.Instance.loops.AddRange(currentLoops);
        }
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

    public static List<int> UnpackLoop(KMapLoop loop) {
        // Inefficient method to list all the tiles in any size loop.
        List<int> tileList = new List<int>();
        
        for (int gridIndex = Main.Instance.GridSize - 1; gridIndex >= 0; gridIndex--) {
            // Determine if the grid tile's binary index is contained within the tiles
            // represented, which are found using the cross product of all combinations.
            bool tileIsContained = true;
            for (int bitChecked = 0; bitChecked < Main.Instance.inputLength; bitChecked++) {
                if (!loop[bitChecked].Contains(BinaryHelper.BitIsSet(gridIndex, bitChecked))) {
                    tileIsContained = false;
                }
            }
            if (tileIsContained) {
                tileList.Add(gridIndex);
            }
        }
        return tileList;
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

    public static KMapLoop MergeAdjacent(KMapLoop startLoop, KMapLoop neighbour) {
        // {{0}, {0}} merge {{0}, {1}} -> {{0}, {0, 1}}
        KMapLoop mergeLoop = new KMapLoop();

        // Merge each logic value from the loops in pairs.
        for (int i = startLoop.Count - 1; i >= 0; i--) {
            HashSet<bool> logicIncludeSet = new HashSet<bool>();

            for (int j = 0; j < startLoop[i].Count; j++) {
                logicIncludeSet.Add(startLoop[i][j]);
                logicIncludeSet.Add(neighbour[i][j]);
            }
            // HashSet used to eliminate duplicates if matching: {0} + {0} -> {0},
            // but to keep both logic values if different: {0} + {1} -> {0, 1}.
            mergeLoop.Add(logicIncludeSet.ToList());
        }
        return mergeLoop;
    }
}
