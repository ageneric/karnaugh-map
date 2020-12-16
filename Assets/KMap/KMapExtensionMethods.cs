using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public static class KMapExtensionMethods
{
    // Taken from https://ericlippert.com/2010/06/28/computing-a-cartesian-product-with-linq/.
    // Computes the Cartesian / "cross" product of all logic values included within a loop.
    // {{0}, {0, 1}} -> {{0, 0}, {0, 1}}
    public static IEnumerable<IEnumerable<bool>> CrossProductCombinations(this IEnumerable<IEnumerable<bool>> sequences) {
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

    public static List<int> CellsCovered(this KMapLoop loop) {
        List<int> neighbourCellList = new List<int>();

        foreach (IEnumerable<bool> cellBitList in loop.CrossProductCombinations()) {
            int gridIndex = BinaryHelper.BooleanToBinaryValue(cellBitList.ToArray());
            neighbourCellList.Add(gridIndex);
        }
        return neighbourCellList;
    }

    public static string ToReadableString(this KMapLoop loop) {
        StringBuilder representation = new StringBuilder();
        representation.Append("{" + string.Join(" ", loop.CellsCovered()) + "} ");

        foreach (List<bool> logicIncludeList in loop) {
            if (logicIncludeList.Count == 2)
                representation.Append('X');
            else if (logicIncludeList[0])
                representation.Append('1');
            else
                representation.Append('0');
        }
        return representation.ToString();
    }

    public static string ToProductsString(this KMapLoop loop) {
        List<string> loopStrings = new List<string>();

        // Create the strings from each logic value (used for "sum-of-products" notation).
        for (int i = 0; i < loop.Count; i++) {
            if (loop[i].Count == 1) {
                if (loop[i][0])
                    loopStrings.Add(VariableExpression.logicVariableAlphabet[i].ToString());
                else
                    loopStrings.Add("¬" + VariableExpression.logicVariableAlphabet[i]);
            }
        }

        if (loopStrings.Count > 0)
            return string.Join(".", loopStrings);
        else
            return "1";
    }
}
