using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapSimplify
{
    // Simplify the k-map by finding "loops" in the binary grid.

    public static void Solve() {
        // Each grid index that has been added to a loop is considered a connected tile.
        // When ExpandLoop() returns, each connected tile is optimally looped, so is not checked again.
        bool[] tilesConnected = new bool[Main.Instance.GridSize];

        for (int index = 0; index < Main.Instance.GridSize; index++) {
            // For each bit that is set, form loops.
            if (Main.Instance.gridState[index]) {
                KMapLoop startLoop = SeedLoop(index);
                // Expand the loop around each tile (maximal loops saved to Main.Instance.loops).
                if (!tilesConnected[index]) {
                    tilesConnected[index] = true;
                    ExpandLoop(startLoop, tilesConnected, true);
                }
                else {
                    ExpandLoop(startLoop, tilesConnected, false);
                }
            }
        }
    }

    public static KMapLoop SeedLoop(int startIndex) {
        // Create a size-1 loop containing the index (which should represent an enabled truth-table bit).
        KMapLoop loop = new KMapLoop();

        foreach (bool bit in BinaryHelper.BinaryValueToBoolean(startIndex, Main.Instance.inputLength)) {
            loop.Add(new List<bool>() { bit });
        }
        return loop;
    }

    public static void ExpandLoop(KMapLoop startLoop, bool[] tilesConnected, bool isMaximum, int depth = 0) {
        if (depth > 32) {
            Debug.LogWarning("Exceeded recursion limit.");
            return;
        }

        Debug.Log("Start about: " + startLoop.ToPresentableString() + " Connected: " + string.Join(", ", tilesConnected));

        // For each flippable bit (neighbouring tile).
        for (int flipBit = 0; flipBit < Main.Instance.inputLength; flipBit++) {
            if (startLoop[flipBit].Count == 2) {
                continue;
            }
            // Create the neighbouring loop.
            KMapLoop neighbour = AdjacentLoop(startLoop, flipBit);

            // Unpack into all the tile indexes contained in the neighbouring loop.
            List<int> neighbourTileList = UnpackForNewTiles(neighbour);
            Debug.Log("Neighbour: " + neighbour.ToPresentableString() + " Checking tiles: " + string.Join(", ", neighbourTileList));

            // If all those tiles are contained in other loops, avoid the overlap.
            if (neighbourTileList.All(tileIndex => tilesConnected[tileIndex])) {
                continue;
            }
            // If all those tiles are set to 1, can merge together into larger loop.
            if (neighbourTileList.All(tileIndex => Main.Instance.gridState[tileIndex])) {
                KMapLoop mergeLoop = MergeAdjacent(startLoop, neighbour);
                Debug.Log("Merged: " + mergeLoop.ToPresentableString());

                foreach (int tileIndex in neighbourTileList) {
                    tilesConnected[tileIndex] = true;
                }
                ExpandLoop(mergeLoop, tilesConnected, true, depth + 1);
                foreach (int tileIndex in neighbourTileList) {
                    ExpandLoop(SeedLoop(tileIndex), tilesConnected, false, depth + 1);
                }
                isMaximum = false;
            }
        }

        Debug.Log(startLoop.ToPresentableString() + " " + isMaximum);

        // Append remaining to loop list (add original only if it can't be expanded).
        if (isMaximum)
            Main.Instance.loops.Add(startLoop);
    }

    public static List<int> UnpackForNewTiles(KMapLoop loop) {
        List<int> neighbourTileList = new List<int>();
        IEnumerable<IEnumerable<bool>> combinations = CrossProductCombinations(loop);

        foreach (IEnumerable<bool> tileBitList in combinations) {
            int gridIndex = BinaryHelper.BooleanToBinaryValue(tileBitList.ToArray());
            neighbourTileList.Add(gridIndex);
        }
        return neighbourTileList;
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

    // Taken from https://ericlippert.com/2010/06/28/computing-a-cartesian-product-with-linq/.
    // Computes the Cartesian / "cross" product of all logic values included within a loop.
    // {{0}, {0, 1}} -> {{0, 0}, {0, 1}}
    public static IEnumerable<IEnumerable<bool>> CrossProductCombinations(IEnumerable<IEnumerable<bool>> sequences) {
        // Base case is {} - if no sequences are provided.
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
        for (int i = 0; i < startLoop.Count; i++) {
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
