using System.Collections.Generic;
using System.Linq;
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

    public static string ToReadableString(this KMapLoop loop) {
        List<string> loopStrings = new List<string>();
        foreach (List<bool> logicIncludeList in loop) {

            string representation;
            if (logicIncludeList.Count == 1) {
                if (logicIncludeList[0] == true)
                    representation = "1";
                else
                    representation = "0";
            }
            else {
                representation = "X";
            }
            loopStrings.Add(string.Join("", representation));
        }
        return "{" + string.Join(", ", loopStrings) + "}";
    }
}
