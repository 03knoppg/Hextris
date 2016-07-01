using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleSelectButton : UIButton {

    public int levelIndex;
    public int stars;

    protected new void Start()
    {
        base.Start();
        if (Progression.Puzzles[levelIndex].unlocked)
            State = UIStates.State.Active;
        else
            State = UIStates.State.Disabled;

        UISignals.AddListeners(OnSignalOne, new List<Signal>() { Signal.PuzzleComplete });
    }

    private void OnSignalOne(Signal signal, object arg1)
    {
        if (signal == Signal.PuzzleComplete)
        {
            if(Driver.currentGameIndex == levelIndex)
                stars = (int)arg1;
            else if(Driver.currentGameIndex == levelIndex - 1)
                State = UIStates.State.Active;
        }
    }

    protected override void Click()
    {
        UISignals.Invoke(signal, levelIndex);
    }

    public void SetText(string text)
    {
        GetComponentInChildren<Text>().text = text;
    }
}
