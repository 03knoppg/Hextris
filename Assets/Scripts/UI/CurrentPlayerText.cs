using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CurrentPlayerText : UIThing {

    public void Update()
    {
        GetComponentInChildren<Text>().text = "Player " + (UIState.currentPlayer + 1) + "'s turn";
        
    }
}
