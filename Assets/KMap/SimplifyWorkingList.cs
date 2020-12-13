using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

struct PrimeLoop
{
    // Holds the loop and the cells it contains.
    public KMapLoop Loop { get; set; }
    public List<int> CellList { get; set; }

    public PrimeLoop(KMapLoop loop, List<int> cellList) {
        Loop = loop;
        CellList = cellList;
    }

    public int UniqueCellCount(bool[] cellsDefinitelyCovered) {
        int count = 0;
        foreach (int gridIndex in CellList) {
            if (!cellsDefinitelyCovered[gridIndex])
                count++;
        }
        return count;
    }

    public int MinimumCellVisited(bool[] cellsDefinitelyCovered, int[] cellsVisited) {
        int minimum = int.MaxValue;
        foreach (int gridIndex in CellList) {
            if (!cellsDefinitelyCovered[gridIndex])
                minimum = Mathf.Min(minimum, cellsVisited[gridIndex]);
        }
        return minimum;
    }
}

public static class SimplifyWorkingList
{
    public static void Simplify(List<KMapLoop> workingLoops) {
        // Stores the number of times each cell is looped. If a cell is only covered by one loop
        // (has only been visited once), that loop is essential for an optimal solution.
        int[] cellsLoopCount = new int[Main.Instance.GridSize];

        // Cache the cell lists contained within each working loop.
        // Count the number of times each cell has been looped.
        List<PrimeLoop> currentLoops = new List<PrimeLoop>();
        foreach (KMapLoop possibleLoop in workingLoops) {
            Debug.Log(possibleLoop.ToReadableString());
            PrimeLoop loopData = new PrimeLoop(possibleLoop, KMapSolve.UnpackLoopToCells(possibleLoop));
            foreach (int gridIndex in loopData.CellList)
                cellsLoopCount[gridIndex]++;

            currentLoops.Add(loopData);
        }

        Debug.Log(string.Join(", ", cellsLoopCount));

        // Identify essential loops from the working list and add them to the finished list.
        for (int i = currentLoops.Count - 1; i >= 0; i--) {
            if (currentLoops[i].CellList.Any(gridIndex => cellsLoopCount[gridIndex] == 1)) {
                Debug.Log(currentLoops[i].Loop.ToReadableString() + " is Essential[I'1].");
                Main.Instance.loops.Add(currentLoops[i].Loop);
                currentLoops.RemoveAt(i);
            }
        }

        // Record all cells that are covered by essential loops (thus are not considered).
        bool[] cellsDefinitelyCovered = new bool[Main.Instance.GridSize];
        foreach (KMapLoop essentialLoop in Main.Instance.loops) {
            List<int> cellList = KMapSolve.UnpackLoopToCells(essentialLoop);
            foreach (int gridIndex in cellList)
                cellsDefinitelyCovered[gridIndex] = true;
        }

        while (currentLoops.Count > 0) {
            // Identify essential loops from the working list and add them to the finished list.
            for (int i = currentLoops.Count - 1; i >= 0; i--) {
                if (currentLoops[i].MinimumCellVisited(cellsDefinitelyCovered, cellsLoopCount) == 1) {
                    Main.Instance.loops.Add(currentLoops[i].Loop);
                    foreach (int gridIndex in currentLoops[i].CellList)
                        cellsDefinitelyCovered[gridIndex] = true;

                    Debug.Log(currentLoops[i].Loop.ToReadableString() + " is Essential[I'2].");
                    currentLoops.RemoveAt(i);
                }
            }
            if (currentLoops.Count == 0)
                continue;

            // Eliminate loops that are fully contained by essential loops (have no unique cells).
            for (int i = currentLoops.Count - 1; i >= 0; i--) {
                if (currentLoops[i].CellList.All(gridIndex => cellsDefinitelyCovered[gridIndex])) {
                    Debug.Log(currentLoops[i].Loop.ToReadableString() + " is not Essential[I'4].");
                    currentLoops.RemoveAt(i);
                }
            }
            if (currentLoops.Count == 0)
                continue;

            // Sort the loops so that the loop which covers the most unique cells is first.
            currentLoops.OrderByDescending(loopData => loopData.UniqueCellCount(cellsDefinitelyCovered));
            int max = currentLoops.FirstOrDefault().UniqueCellCount(cellsDefinitelyCovered);
            int min = currentLoops.LastOrDefault().UniqueCellCount(cellsDefinitelyCovered);

            // Choose a single loop that contains the most unique cells and add it to the loop.
            // If all loops contain the same number of unique cells, choose a single loop and remove it.
            if (max - min > 0) {
                Main.Instance.loops.Add(currentLoops.First().Loop);
                foreach (int gridIndex in currentLoops.First().CellList)
                    cellsDefinitelyCovered[gridIndex] = true;

                Debug.Log(currentLoops.First().Loop.ToReadableString() + " is Essential[I'3].");
                currentLoops.RemoveAt(0);
            }
            else {
                foreach (int gridIndex in currentLoops.Last().CellList)
                    cellsLoopCount[gridIndex]--;

                Debug.Log(currentLoops.Last().Loop.ToReadableString() + " is not Essential[I'5].");
                currentLoops.RemoveAt(currentLoops.Count - 1);
            }

            Debug.Log(workingLoops.Count.ToString());
        }
    }
}