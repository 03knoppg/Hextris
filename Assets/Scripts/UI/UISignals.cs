using UnityEngine;
using System.Collections;

public class UISignals : MonoBehaviour {

    public delegate void EndTurnDelegate();
    public EndTurnDelegate OnEndTurn;

    public void EndTurnClicked()
    {
        if(OnEndTurn != null)
            OnEndTurn.Invoke();
    }
}
