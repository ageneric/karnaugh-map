﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapSolve
{
    // Simplify the k-map by finding "loops" in the binary grid.

    private static List<KMapLoop> workingLoops;

    public static void Solve() {
        // Stores loops found during solving. Essential loops are moved to Main.Instance.loops.
        workingLoops = new List<KMapLoop>();
        Main.Instance.loops = new List<KMapLoop>();

        bool[] cellsVisited = new bool[Main.Instance.GridSize];
        // Iterate through the grid. A checkerboard pattern is used, so that redundant checks
        // between neighbouring cells are not made (which can lead to missing some loops).
        List<int> diagonalCheckingOrder = BinaryHelper.DiagonalWeave(Main.Instance.GridSize);
        for (int i = 0; i < Main.Instance.GridSize; i++) {
            int gridIndex = diagonalCheckingOrder[i];

            if (Main.Instance.gridState[gridIndex] && !cellsVisited[gridIndex]) {
                // For each bit that is set, form a loop containing it, and expand to find maximal loops.
                KMapLoop startLoop = SeedLoop(gridIndex);
                // For optimisation, essential single/double loops are detected as they are made.
                if (CountAdjacentCellsSet(gridIndex) <= 1) {
                    ExpandLoop(startLoop, true, gridIndex, cellsVisited);
                }
                else {
                    ExpandLoop(startLoop, false, gridIndex, cellsVisited);
                }

                cellsVisited[gridIndex] = true;
            }
        }
        // Log loop state for debugging.
        KMapLogging.LogLoopList("Essential loops", Main.Instance.loops);
        KMapLogging.LogLoopList("Other maximsed loops", workingLoops);

        SimplifyWorkingList.Simplify(workingLoops);
    }

    public static KMapLoop SeedLoop(int startIndex) {
        // Create a size-1 loop containing the index (which should represent an enabled truth-table bit).
        KMapLoop loop = new KMapLoop();

        foreach (bool bit in BinaryHelper.BinaryValueToBoolean(startIndex, Main.Instance.inputLength)) {
            loop.Add(new List<bool>() { bit });
        }
        return loop;
    }

    public static int CountAdjacentCellsSet(int startIndex) {
        int connectionCount = 0;

        // For each flippable bit (neighbouring cell).
        for (int flipBit = 0; flipBit < Main.Instance.inputLength; flipBit++) {
            // Check the neighbouring cell and record if it is true (connectable).
            if (Main.Instance.gridState[startIndex ^ BinaryHelper.PowBaseTwo(flipBit)])
                connectionCount++;
        }
        return connectionCount;
    }

    public static void ExpandLoop(KMapLoop startLoop, bool isEssential, int minimumIndex, bool[] cellsVisitedAllTime,
                                  List<int> cellsConnected=null) {
        // Recursively find all of the maximal loops connected to the given loop.
        // Returns the number of loops made around the given loop.

        if (cellsConnected is null)
            cellsConnected = new List<int>();
        bool isMaximum = true;
        // Debug.Log("Start about: " + startLoop.ToReadableString());

        // For each flippable bit (neighbouring cell / loop).
        for (int flipBit = 0; flipBit < Main.Instance.inputLength; flipBit++) {
            if (startLoop[flipBit].Count == 2)
                continue;
            
            // Create the neighbouring loop.
            KMapLoop neighbour = AdjacentLoop(startLoop, flipBit);

            // Unpack into all the cell indexes contained in the neighbouring loop.
            List<int> neighbourCellList = neighbour.CellsCovered();

            // If the neighbour is already connected within a previous larger loop, avoid the overlap.
            if (neighbourCellList.All(gridIndex => cellsConnected.Contains(gridIndex))) {
                continue;
            }
            // If all those cells are set to 1, can merge together into larger loop.
            else if (neighbourCellList.All(gridIndex => Main.Instance.gridState[gridIndex])) {
                KMapLoop mergeLoop = MergeAdjacent(startLoop, neighbour);
                // Debug.Log("Merged: " + mergeLoop.ToReadableString());

                foreach (int gridIndex in neighbourCellList) {
                    cellsVisitedAllTime[gridIndex] = true;
                    cellsConnected.Add(gridIndex);
                }
                ExpandLoop(mergeLoop, isEssential, minimumIndex, cellsVisitedAllTime, cellsConnected);
                isMaximum = false;
            }
        }

        // If no larger loops can be formed, the loop is maximally expanded, so it is saved to the list.
        if (isMaximum) {
            if (isEssential)
                Main.Instance.loops.Add(startLoop);
            else
                workingLoops.Add(startLoop);
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

/* Example cases
 * 
 * 0 1 0 0  -> 4 loops (2)
 * 0 1 1 1
 * 1 1 1 0
 * 0 0 1 0
 * 
 * 1 0 0 1  -> 4 loops (2)
 * 0 0 1 1
 * 0 1 1 0
 * 1 1 0 0
 * 
 * 1 1 1 1  -> 1 loop  (1)
 * 1 1 1 0  -> 3 loops (4)
 * 0 0 0 1
 * 0 0 0 0
 * 
 * 0 1 1 0  -> 1 loop  (2)
 * 0 0 1 1  -> 2 loops (4)
 * 1 1 1 1
 * 1 1 0 0
 */
