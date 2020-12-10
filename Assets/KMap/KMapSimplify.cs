using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapSimplify
{
    // Simplify the k-map by finding "loops" in the binary grid.

    private static List<KMapLoop> workingLoops;

    public static void Solve() {
        workingLoops = new List<KMapLoop>(); // Holds any maximal loops found that may not be essential.
        int[] cellsVisited = new int[Main.Instance.GridSize];

        // Find and maximise essential cells first.
        // If a cell is only covered by one loop, it is essential for the optimal solution.
        for (int gridIndex = 0; gridIndex < Main.Instance.GridSize; gridIndex++) {
            if (Main.Instance.gridState[gridIndex] && cellsVisited[gridIndex] == 0) {
                // For each bit that is set, form a loop containing it, and expand to find maximal loops.
                if (SingleCellConnectionCount(gridIndex) <= 1) {
                    KMapLoop startLoop = SeedLoop(gridIndex);
                    cellsVisited[gridIndex]++;
                    ExpandLoop(startLoop, cellsVisited, true);
                }
            }
        }
        // Form loops around all remaining cells.
        for (int gridIndex = 0; gridIndex < Main.Instance.GridSize; gridIndex++) {
            if (Main.Instance.gridState[gridIndex] && cellsVisited[gridIndex] == 0) {
                // For each bit that is set, form a loop containing it, and expand to find maximal loops.
                KMapLoop startLoop = SeedLoop(gridIndex);
                cellsVisited[gridIndex]++;
                ExpandLoop(startLoop, cellsVisited, false);
            }
        }

        foreach (KMapLoop possibleLoop in workingLoops) {
            Debug.Log(possibleLoop.ToPresentableString());
        }
        Main.Instance.loops.AddRange(workingLoops);
    }

    public static KMapLoop SeedLoop(int startIndex) {
        // Create a size-1 loop containing the index (which should represent an enabled truth-table bit).
        KMapLoop loop = new KMapLoop();

        foreach (bool bit in BinaryHelper.BinaryValueToBoolean(startIndex, Main.Instance.inputLength)) {
            loop.Add(new List<bool>() { bit });
        }
        return loop;
    }

    public static int SingleCellConnectionCount(int startIndex) {
        int connectionCount = 0;

        // For each flippable bit (neighbouring cell).
        for (int flipBit = 0; flipBit < Main.Instance.inputLength; flipBit++) {
            // Check the neighbouring cell and record if it is true (connectable).
            if (Main.Instance.gridState[startIndex ^ BinaryHelper.PowBaseTwo(flipBit)])
                connectionCount++;
        }
        return connectionCount;
    }

    public static void ExpandLoop(KMapLoop startLoop, int[] cellsLoopCount, bool isEssential, List<int> cellsConnected=null) {
        if (cellsConnected is null)
            cellsConnected = new List<int>();
        bool isMaximum = true;
        Debug.Log("Start about: " + startLoop.ToPresentableString() + " Connected: " + string.Join(", ", cellsLoopCount));

        // For each flippable bit (neighbouring cell / loop).
        for (int flipBit = 0; flipBit < Main.Instance.inputLength; flipBit++) {
            if (startLoop[flipBit].Count == 2) {
                continue;
            }
            // Create the neighbouring loop.
            KMapLoop neighbour = AdjacentLoop(startLoop, flipBit);

            // Unpack into all the cell indexes contained in the neighbouring loop.
            List<int> neighbourCellList = UnpackForNewCells(neighbour);
            Debug.Log("Neighbour: " + neighbour.ToPresentableString() + " Checking cells: " + string.Join(", ", neighbourCellList));

            // If the neighbour is already connected within a previous larger loop, avoid the overlap.
            if (neighbourCellList.All(cellIndex => cellsConnected.Contains(cellIndex))) {
                continue;
            }
            // If all those cells are set to 1, can merge together into larger loop.
            if (neighbourCellList.All(cellIndex => Main.Instance.gridState[cellIndex])) {
                KMapLoop mergeLoop = MergeAdjacent(startLoop, neighbour);
                Debug.Log("Merged: " + mergeLoop.ToPresentableString());

                foreach (int cellIndex in neighbourCellList) {
                    cellsLoopCount[cellIndex]++;
                    cellsConnected.Add(cellIndex);
                }
                ExpandLoop(mergeLoop, cellsLoopCount, isEssential, cellsConnected);
                isMaximum = false;
            }
        }

        Debug.Log(startLoop.ToPresentableString() + " " + isMaximum);

        // Append remaining to loop list (add original only if it can't be expanded).
        
        if (isMaximum) {
            if (isEssential)
                Main.Instance.loops.Add(startLoop);
            else
                workingLoops.Add(startLoop);
        }
    }

    public static List<int> UnpackForNewCells(KMapLoop loop) {
        List<int> neighbourCellList = new List<int>();
        IEnumerable<IEnumerable<bool>> combinations = loop.CrossProductCombinations();

        foreach (IEnumerable<bool> cellBitList in combinations) {
            int gridIndex = BinaryHelper.BooleanToBinaryValue(cellBitList.ToArray());
            neighbourCellList.Add(gridIndex);
        }
        return neighbourCellList;
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
