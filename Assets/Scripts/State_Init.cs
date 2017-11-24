using DTAnimatorStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Init : DTStateMachineBehaviour<Game>
{
    protected override void OnStateEntered()
    {


        //Signals.AddListeners((signal, args) => OnSignal(signal, args), new List<ESignalType>() {
        //    ESignalType.ShowBoardSelect,
        //    ESignalType.SelectBoard,
        //    ESignalType.Quit,
        //    ESignalType.Restart });
    }

}
