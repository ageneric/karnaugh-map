using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KMapLoop = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

public class LoopAroundSprite : MonoBehaviour
{
    // Displays a single k-map loop which encircles the indexes it includes.

    [EditorReadOnly] public List<List<bool>> loop;
    [EditorReadOnly] public Transform offset;
    public float positionScale;
    public Image loopImage;
    private float colorSaturation;
    private float colorBrightness;

    private void Start() {
        Color.RGBToHSV(loopImage.color, out _, out colorSaturation, out colorBrightness);
        ColorSprite(Random.value);
    }

    public void PositionInOverlay() {
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

        // Horizontal positioning - with special case for "wrap-around" loops.
        if (scaleWidth == 2 && offsetWidth == 0 && maximumRight == 3) {
            offsetWidth = 3;
        }
        // Vertical positioning - with special case for "wrap-around" loops.
        if (scaleHeight == 2 && offsetHeight == 0 && maximumDown == 3) {
            offsetHeight = 3;
        }

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        // Column offset -ve: Increasing indexes appear top-to-bottom, i.e. decreasing y.
        rectTransform.position = new Vector3(offsetWidth, -offsetHeight) * positionScale + offset.position;
        rectTransform.sizeDelta = new Vector2(scaleWidth * positionScale, scaleHeight * positionScale);
    }

    public int PositionRangeInAxis(KMapLoop loop, int[] checkBits, out int maximum, out int numCellsInAxis) {
        // Get the leftmost and rightmost position, or upmost and downmost position.
        KMapLoop singleAxisLoop = new KMapLoop();
        foreach (int bit in checkBits) {
            singleAxisLoop.Add(loop[bit]);
        }
        IEnumerable<IEnumerable<bool>> combinations = KMapSimplify.CrossProductCombinations(singleAxisLoop);
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

    public void ColorSprite(float hue) {
        loopImage.color = Color.HSVToRGB(hue, colorSaturation, colorBrightness);
        loopImage.canvasRenderer.SetAlpha(0.3f);
    }
}
