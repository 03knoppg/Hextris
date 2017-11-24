using System.Collections.Generic;
using UnityEngine.UI;

public class PuzzleSelectButton : UIButton {

    public int levelIndex;
    public int stars;

    protected new void Start()
    {
        base.Start();
        if (Progression.Puzzles.Obj[levelIndex].unlocked)
            State = UIStates.State.Active;
        else
            State = UIStates.State.Disabled;

        Signals.AddListeners(OnSignalOne, new List<ESignalType>() { ESignalType.PuzzleComplete });
    }

    private void OnSignalOne(ESignalType signal, object arg1)
    {
        if (signal == ESignalType.PuzzleComplete)
        {
            if(Game.currentGameIndex == levelIndex)
                stars = (int)arg1;
            else if(Game.currentGameIndex == levelIndex - 1)
                State = UIStates.State.Active;
        }
    }

    protected override void Click()
    {
        Signals.Invoke(signal, levelIndex);
    }

    public void SetText(string text)
    {
        GetComponentInChildren<Text>().text = text;
    }
}
