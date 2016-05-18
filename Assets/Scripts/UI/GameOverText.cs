using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverText : UIThing {

    public override void OnStateChanged(UIStates.State newState)
    {
        GetComponentInChildren<Text>().text = "Player " + (UIState.winner + 1) + " Wins!";
        base.OnStateChanged(newState);
    }
}
