using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverText : UIThing {

    new void Start()
    {
        base.Start();
        Signals.AddListeners(OnPlayerChange, new List<ESignalType>() { ESignalType.PlayerWin });
    }

    void OnPlayerChange(ESignalType Signal, object playerIndexObj)
    {
        if (Signal == global::ESignalType.PlayerWin)
            GetComponentInChildren<Text>().text = "Player " + (((int)playerIndexObj) + 1) + " Wins!";
    }

}
