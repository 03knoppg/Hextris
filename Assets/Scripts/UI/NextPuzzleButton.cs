using System.Collections.Generic;

public class NextPuzzleButton : UIButton {

	// Use this for initialization
	new void Start () {
		base.Start();

		Signals.AddListeners(OnBoardSelect, new List<ESignalType>() { ESignalType.StartPuzzle });
	}

	private void OnBoardSelect(ESignalType signal, object arg1)
	{
		if (signal == ESignalType.StartPuzzle)
		{
			if (Game.currentGameIndex == (Progression.Puzzles.Count - 1))
				OnStateChanged(UIStates.State.Disabled);
			
			else
				OnStateChanged(UIStates.State.Active);
		}
	}
	
}
