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
                offsetWidth = PositionRangeInAxis(loop, new int[] { 1 }, out maximumRight);
                offsetHeight = PositionRangeInAxis(loop, new int[] { 0 }, out maximumDown);
                break;
            case 3:
                offsetWidth = PositionRangeInAxis(loop, new int[] { 1, 2 }, out maximumRight);
                offsetHeight = PositionRangeInAxis(loop, new int[] { 0 }, out maximumDown);
                break;
            case 4:
                offsetWidth = PositionRangeInAxis(loop, new int[] { 2, 3 }, out maximumRight);
                offsetHeight = PositionRangeInAxis(loop, new int[] { 0, 1 }, out maximumDown);
                break;
            default:
                maximumRight = maximumDown = 0;
                break;
        }

        // Horizontal positioning - with special case for "wrap-around" loops.
        if (offsetWidth == 0 && maximumRight == 3) {
            offsetWidth = 3;
            scaleWidth = 2;
        }
        else {
            scaleWidth += maximumRight - offsetWidth;
        }
        // Vertical positioning - with special case for "wrap-around" loops.
        if (offsetHeight == 0 && maximumDown == 3) {
            offsetHeight = 3;
            scaleHeight = 2;
        }
        else {
            scaleHeight += maximumDown - offsetHeight;
        }

        // Column offset -ve: Increasing indexes appear top-to-bottom, i.e. decreasing y.
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(offsetWidth, -offsetHeight) * positionScale + offset.position;
        rectTransform.sizeDelta = new Vector2(scaleWidth * positionScale, scaleHeight * positionScale);
    }

    public int PositionRangeInAxis(KMapLoop loop, int[] checkBits, out int maximum) {
        // Get the leftmost and rightmost position, or upmost and downmost position.
        // 
        KMapLoop singleAxisLoop = new KMapLoop();
        foreach (int bit in checkBits) {
            singleAxisLoop.Add(loop[bit]);
        }
        IEnumerable<IEnumerable<bool>> combinations = KMapSimplify.CrossProductCombinations(singleAxisLoop);
        int minimum = BinaryHelper.PowBaseTwo(checkBits.Length);
        maximum = 0;

        foreach (IEnumerable<bool> tileBitList in combinations) {
            int lineIndex = BinaryHelper.GrayCode(BinaryHelper.BooleanToBinaryValue(tileBitList.ToArray()));

            if (lineIndex < minimum)
                minimum = lineIndex;
            if (lineIndex > maximum)
                maximum = lineIndex;
        }
        return minimum;
    }

    public void ColorSprite(float hue) {
        loopImage.color = Color.HSVToRGB(hue, colorSaturation, colorBrightness);
        loopImage.canvasRenderer.SetAlpha(0.3f);
    }
}
