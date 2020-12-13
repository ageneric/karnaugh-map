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

    public int MinCellVisits(bool[] cellsDefinitelyCovered, int[] cellsVisited) {
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
            PrimeLoop loopData = new PrimeLoop(possibleLoop, KMapSolve.UnpackLoopToCells(possibleLoop));
            foreach (int gridIndex in loopData.CellList)
                cellsLoopCount[gridIndex]++;

            currentLoops.Add(loopData);
        }
        Debug.Log("Cell loop counts: " + string.Join(", ", cellsLoopCount));

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
                if (currentLoops[i].MinCellVisits(cellsDefinitelyCovered, cellsLoopCount) == 1) {
                    foreach (int gridIndex in currentLoops[i].CellList)
                        cellsDefinitelyCovered[gridIndex] = true;

                    Main.Instance.loops.Add(currentLoops[i].Loop);
                    currentLoops.RemoveAt(i);
                }
            }

            // Eliminate loops that are fully contained by essential loops (have no unique cells).
            for (int i = currentLoops.Count - 1; i >= 0; i--) {
                if (currentLoops[i].CellList.All(gridIndex => cellsDefinitelyCovered[gridIndex])) {
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

                currentLoops.RemoveAt(0);
            }
            else {
                foreach (int gridIndex in currentLoops.Last().CellList)
                    cellsLoopCount[gridIndex]--;

                currentLoops.RemoveAt(currentLoops.Count - 1);
            }

            Debug.Log("Step. " + workingLoops.Count.ToString() + " loops remain.");
        }
    }
}