using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StateTitleScreen : HextrisStateMachineBehaviour { 

    protected override void OnEnter()
    {
        AddListeners(new List<ESignalType> { ESignalType.PuzzleMode, ESignalType.PvPMode, ESignalType.ClearProgression });

        HextrisStateMachine.GameType = HextrisStateMachine.EGameType.Unselected;
    }

    protected override void OnSignal(ESignalType signalType)
    {
        switch (signalType)
        {
            case ESignalType.PuzzleMode:
                HextrisStateMachine.GameType = HextrisStateMachine.EGameType.Puzzle;
                //SMTransition(signalType);
                SceneManager.LoadScene("Game");
                break;
            case ESignalType.PvPMode:
                HextrisStateMachine.GameType = HextrisStateMachine.EGameType.PvP;
                //SMTransition(signalType);
                SceneManager.LoadScene("Game");
                break;
            case ESignalType.ClearProgression:
                Progression.ClearProgression();
                break;
        }
    }
}
