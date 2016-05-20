using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PuzzleSelectButton : UIButton {

    public int levelIndex;

    protected override void Click()
    {
        UISignals.Click(signal, levelIndex);
    }
}
