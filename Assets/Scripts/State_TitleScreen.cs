using DTAnimatorStateMachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class State_TitleScreen : DTStateMachineBehaviour<Game> { 

    protected override void OnStateEntered()
    {
        Signals.AddListeners(OnSignal, new List<ESignalType> { ESignalType.PuzzleMode, ESignalType.PvPMode });
    }

    private void OnSignal(ESignalType signalType)
    {
        switch (signalType)
        {
            case ESignalType.PuzzleMode:
                SceneManager.LoadScene("Puzzle");
                break;
            case ESignalType.PvPMode:
                SceneManager.LoadScene("PvP");
                break;
            default:
                break;
        }
    }

    protected override void OnStateExited()
    {
        Signals.RemoveListeners(OnSignal, new List<ESignalType> { ESignalType.PuzzleMode, ESignalType.PvPMode });
    }
}
