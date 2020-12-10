using UnityEngine;

public class GridScene : MonoBehaviour
{
    public BinaryGrid grid;
    public KMapOverlay loopSpriteOverlay;

    void Start() {

    }

    public void UpdateInputLength(int newInputLength) {
        Main.Instance.UpdateInputLength(newInputLength);
        loopSpriteOverlay.Clear();
        grid.ResetGrid();
    }

    public void UpdateInput() {
        Main.Instance.ClearLoops();
        loopSpriteOverlay.Clear();
    }

    public void Solve() {
        Main.Instance.ClearLoops();
        KMapSimplify.Solve();

        loopSpriteOverlay.Clear();
        loopSpriteOverlay.QueueDraw();
    }
}
