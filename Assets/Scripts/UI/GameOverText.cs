using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverText : UIThing {

    new void Start()
    {
        base.Start();
        UISignals.AddListeners(OnPlayerChange, new List<Signal>() { Signal.PlayerWin });
    }

    void OnPlayerChange(Signal Signal, object playerIndexObj)
    {
        if (Signal == global::Signal.PlayerWin)
            GetComponentInChildren<Text>().text = "Player " + (((int)playerIndexObj) + 1) + " Wins!";
    }

}
