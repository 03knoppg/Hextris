using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StateTitleScreen : HextwistStateMachineBehaviour { 

    protected override void OnEnter()
    {
        AddListeners(new List<ESignalType> { ESignalType.PuzzleMode, ESignalType.PvPMode, ESignalType.ClearProgression });

        HextwistStateMachine.GameType = HextwistStateMachine.EGameType.Unselected;
    }

    protected override void OnSignal(ESignalType signalType)
    {
        switch (signalType)
        {
            case ESignalType.PuzzleMode:
                HextwistStateMachine.GameType = HextwistStateMachine.EGameType.Puzzle;
                //SMTransition(signalType);
                SceneManager.LoadScene("Game");
                break;
            case ESignalType.PvPMode:
                HextwistStateMachine.GameType = HextwistStateMachine.EGameType.PvP;
                //SMTransition(signalType);
                SceneManager.LoadScene("Game");
                break;
            case ESignalType.ClearProgression:
                Progression.ClearProgression();
                break;
        }
    }
}
