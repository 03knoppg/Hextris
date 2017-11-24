using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurrentPlayerText : UIThing {

    new void Start()
    {
        base.Start();
        Signals.AddListeners(OnPlayerChange, new List<ESignalType>(){ESignalType.PlayerTurn}); 
    }

    void OnPlayerChange(ESignalType Signal, object playerIndexObj)
    {
        if(Signal == global::ESignalType.PlayerTurn)
            GetComponentInChildren<Text>().text = "Player " + (((int)playerIndexObj) + 1) + "'s turn";
    }
}
