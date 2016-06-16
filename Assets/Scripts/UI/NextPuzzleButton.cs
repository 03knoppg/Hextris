using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextPuzzleButton : UIButton {

	// Use this for initialization
	new void Start () {
        base.Start();

        UISignals.AddListeners(OnBoardSelect, new List<UISignal>() { UISignal.SelectBoard });
	}

    private void OnBoardSelect(UISignal signal, object arg1)
    {
        if (signal == UISignal.SelectBoard)
        {
            if (FindObjectOfType<Driver>().currentGameIndex == (FindObjectOfType<Driver>().GamePrefabs.Count - 1))
                OnStateChanged(UIStates.State.Disabled);
            
            else
                OnStateChanged(UIStates.State.Active);
        }
    }
	
}
