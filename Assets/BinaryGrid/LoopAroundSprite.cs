using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class LoopAroundSprite : MonoBehaviour
{
    // Displays a single k-map loop which encircles the indexes it includes.

    [EditorReadOnly] public Transform offset;
    public float positionScale;
    public Image loopImage;
    private float colorSaturation;
    private float colorBrightness;

    private void Awake() {
        Color.RGBToHSV(loopImage.color, out _, out colorSaturation, out colorBrightness);
    }

    public List<int[]> PositionInOverlay(List<List<bool>> loop) {
        int offsetWidth = 0;
        int offsetHeight = 0;
        int scaleWidth = 1;
        int scaleHeight = 1;
        int maximumRight;
        int maximumDown;

        switch (Main.Instance.inputLength) {
            case 2:
                offsetWidth = PositionRangeInAxis(loop, new int[] { 1 }, out maximumRight, out scaleWidth);
                offsetHeight = PositionRangeInAxis(loop, new int[] { 0 }, out maximumDown, out scaleHeight);
                break;
            case 3:
                offsetWidth = PositionRangeInAxis(loop, new int[] { 1, 2 }, out maximumRight, out scaleWidth);
                offsetHeight = PositionRangeInAxis(loop, new int[] { 0 }, out maximumDown, out scaleHeight);
                break;
            case 4:
                offsetWidth = PositionRangeInAxis(loop, new int[] { 2, 3 }, out maximumRight, out scaleWidth);
                offsetHeight = PositionRangeInAxis(loop, new int[] { 0, 1 }, out maximumDown, out scaleHeight);
                break;
            default:
                maximumRight = maximumDown = 0;
                break;
        }
        bool wrapAroundWidth = false;
        bool wrapAroundHeight = false;

        // Horizontal positioning - with special case for "wrap-around" loops.
        if (scaleWidth == 2 && offsetWidth == 0 && maximumRight == 3) {
            offsetWidth = maximumRight;
            wrapAroundWidth = true;
        }
        // Vertical positioning - with special case for "wrap-around" loops.
        if (scaleHeight == 2 && offsetHeight == 0 && maximumDown == 3) {
            offsetHeight = maximumDown;
            wrapAroundHeight = true;
        }
        SetPosition(offsetWidth, offsetHeight, scaleWidth, scaleHeight);
        List<int[]> wrapAround = new List<int[]>();

        // Display ghost copies of this sprite to show it wrapping around the grid.
        if (wrapAroundHeight)
            wrapAround.Add(new int[] { offsetWidth, offsetHeight - 4, scaleWidth, scaleHeight });
        if (wrapAroundWidth)
            wrapAround.Add(new int[] { offsetWidth - 4, offsetHeight, scaleWidth, scaleHeight });
        if (wrapAroundHeight && wrapAroundWidth)
            wrapAround.Add(new int[] { offsetWidth - 4, offsetHeight - 4, scaleWidth, scaleHeight });

        return wrapAround;
    }

    public int PositionRangeInAxis(KMapLoop loop, int[] checkBits, out int maximum, out int numCellsInAxis) {
        // Get the leftmost and rightmost position, or upmost and downmost position.
        KMapLoop singleAxisLoop = new KMapLoop();
        foreach (int bit in checkBits) {
            singleAxisLoop.Add(loop[bit]);
        }
        IEnumerable<IEnumerable<bool>> combinations = singleAxisLoop.CrossProductCombinations();
        int minimum = BinaryHelper.PowBaseTwo(checkBits.Length);
        maximum = 0;

        foreach (IEnumerable<bool> cellBitList in combinations) {
            int lineIndex = BinaryHelper.GrayCode(BinaryHelper.BooleanToBinaryValue(cellBitList.ToArray()));

            if (lineIndex < minimum)
                minimum = lineIndex;
            if (lineIndex > maximum)
                maximum = lineIndex;
        }
        numCellsInAxis = combinations.Count();
        return minimum;
    }

    public void SetPosition(int offsetWidth, int offsetHeight, int scaleWidth, int scaleHeight) {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        // Column offset -ve: Increasing indexes appear top-to-bottom, i.e. decreasing y.
        rectTransform.position = new Vector3(offsetWidth, -offsetHeight) * positionScale + offset.position;
        rectTransform.sizeDelta = new Vector2(scaleWidth, scaleHeight) * positionScale;

        float shrinkWidth = 0f;
        float shrinkHeight = 0f;
        if (scaleWidth == 2)
            shrinkWidth = 0.2f;
        if (scaleHeight == 2)
            shrinkHeight = 0.2f;
        rectTransform.position -= new Vector3(shrinkWidth, -shrinkHeight);
        rectTransform.sizeDelta -= new Vector2(shrinkWidth, -shrinkHeight);
    }

    public void ColorSprite(float hue) {
        loopImage.color = Color.HSVToRGB(hue, colorSaturation, colorBrightness);
        loopImage.canvasRenderer.SetAlpha(0.375f);
    }
}
