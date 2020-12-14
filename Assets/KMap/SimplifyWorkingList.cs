using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class SimplifyWorkingList
{
    public static void Simplify(List<KMapLoop> workingLoops) {
        // Stores the number of times each cell is looped. If a cell is only covered by one loop
        // (has only been visited once), that loop is essential for an optimal solution.
        int[] cellsLoopCount = new int[Main.Instance.GridSize];

        // Cache the cell lists contained within each working loop.
        // Count the number of times each cell has been looped.
        foreach (KMapLoop possibleLoop in workingLoops) {
            foreach (int gridIndex in possibleLoop.CellsCovered())
                cellsLoopCount[gridIndex]++;
        }
        Debug.Log("Cell loop counts: " + string.Join(", ", cellsLoopCount));

        // Record all cells that are covered by essential loops (thus are not considered).
        bool[] cellsDefinitelyCovered = new bool[Main.Instance.GridSize];
        foreach (KMapLoop essentialLoop in Main.Instance.loops) {
            List<int> cellList = essentialLoop.CellsCovered();
            foreach (int gridIndex in cellList)
                cellsDefinitelyCovered[gridIndex] = true;
        }

        while (workingLoops.Count > 0) {
            // Identify essential loops from the working list and add them to the finished list.
            for (int i = workingLoops.Count - 1; i >= 0; i--) {
                List<int> cellList = workingLoops[i].CellsCovered();

                // Look for loops which contain a cell that isn't contained by any other loop.
                if (cellList.Any(gridIndex => !cellsDefinitelyCovered[gridIndex]
                        && cellsLoopCount[gridIndex] == 1)) {
                    Debug.Log(workingLoops[i].ToReadableString() + " is essential: unique cell.");
                    foreach (int gridIndex in cellList)
                        cellsDefinitelyCovered[gridIndex] = true;

                    Main.Instance.loops.Add(workingLoops[i]);
                    workingLoops.RemoveAt(i);
                }
            }

            // Eliminate loops that are fully contained by essential loops (have no unique cells).
            for (int i = workingLoops.Count - 1; i >= 0; i--) {
                if (workingLoops[i].CellsCovered().All(gridIndex => cellsDefinitelyCovered[gridIndex])) {
                    Debug.Log(workingLoops[i].ToReadableString() + " is not essential: covered.");
                    workingLoops.RemoveAt(i);
                }
            }

            // When loops exist that partially contain each other, or a complete chain of loops is formed:
            if (workingLoops.Count != 0) {
                // Choose a single loop that contains the fewest unique cells and remove it.
                int minimumUniqueCellCount = Main.Instance.GridSize + 1;
                int indexWorstLoop = 0;
                for (int i = workingLoops.Count - 1; i >= 0; i--) {
                    int currentCount = UniqueCellCount(workingLoops[i], cellsDefinitelyCovered);
                    if (currentCount < minimumUniqueCellCount) {
                        minimumUniqueCellCount = currentCount;
                        indexWorstLoop = i;
                    }
                }
                foreach (int gridIndex in workingLoops[indexWorstLoop].CellsCovered())
                    cellsLoopCount[gridIndex]--;
                workingLoops.RemoveAt(indexWorstLoop);
            }
        }
    }

    public static int UniqueCellCount(KMapLoop loop, bool[] blacklist) {
        int count = 0;
        foreach (int gridIndex in loop.CellsCovered()) {
            if (!blacklist[gridIndex])
                count++;
        }
        return count;
    }
}