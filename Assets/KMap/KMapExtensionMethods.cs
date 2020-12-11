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

    // Alphabet for input bit names (doesn't include I, Q).
    public static string logicVariableAlphabet = "ABCDEFGHJKLMNOPRSTUVWXYZ";

    public static string ToReadableString(this KMapLoop loop) {
        List<string> loopStrings = new List<string>();
        foreach (List<bool> logicIncludeList in loop) {

            string representation;
            if (logicIncludeList.Count == 1) {
                if (logicIncludeList[0])
                    representation = "1";
                else
                    representation = "0";
            }
            else {
                representation = "X";
            }
            loopStrings.Add(representation);
        }
        return "{" + string.Join(", ", loopStrings) + "}";
    }

    public static string ToProductsString(this KMapLoop loop) {
        List<string> loopStrings = new List<string>();

        // Create the strings from each logic value (used for "sum-of-products" notation).
        for (int i = 0; i < loop.Count; i++) {
            if (loop[i].Count == 1) {
                if (loop[i][0])
                    loopStrings.Add(logicVariableAlphabet[i].ToString());
                else
                    loopStrings.Add("¬" + logicVariableAlphabet[i]);
            }
        }
        return string.Join(".", loopStrings);
    }
}
