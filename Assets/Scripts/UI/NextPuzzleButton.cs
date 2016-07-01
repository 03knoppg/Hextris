using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextPuzzleButton : UIButton {

	// Use this for initialization
	new void Start () {
        base.Start();

        UISignals.AddListeners(OnBoardSelect, new List<Signal>() { Signal.SelectBoard });
	}

    private void OnBoardSelect(Signal signal, object arg1)
    {
        if (signal == Signal.SelectBoard)
        {
            if (Driver.currentGameIndex == (Progression.Puzzles.Count - 1))
                OnStateChanged(UIStates.State.Disabled);
            
            else
                OnStateChanged(UIStates.State.Active);
        }
    }
	
}
