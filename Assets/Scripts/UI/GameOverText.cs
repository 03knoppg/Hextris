using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverText : UIThing {

    new void Start()
    {
        base.Start();
        UISignals.AddListeners(OnPlayerChange, new List<UISignal>() { UISignal.PlayerWin });
    }

    void OnPlayerChange(UISignal Signal, object playerIndexObj)
    {
        if (Signal == global::UISignal.PlayerWin)
            GetComponentInChildren<Text>().text = "Player " + (((int)playerIndexObj) + 1) + " Wins!";
    }

}
