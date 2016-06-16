using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial1 : UIThing {

    public float startX;
    public float stopX;

    float t;

    new void Start()
    {
        base.Start();
        UISignals.AddListeners(OnBoardSelect, new List<UISignal>() { UISignal.SelectBoard, UISignal.ShowBoardSelect, UISignal.PlayerWin });
    }

    private void OnBoardSelect(UISignal signal, object arg1)
    {
        if (signal == UISignal.SelectBoard)
        {
            if (arg1 != null && ((int)arg1) == 0)
                OnStateChanged(UIStates.State.Active);

            else
                OnStateChanged(UIStates.State.Hidden);
        }
        else
            OnStateChanged(UIStates.State.Hidden);
    }

	// Update is called once per frame
	void Update () 
    {
        if (state == UIStates.State.Active)
        {
            t = (t + Time.deltaTime / 2) % 1;
            transform.position = new Vector3(Mathf.SmoothStep(Screen.width * startX / 100, Screen.width * stopX / 100, t), transform.position.y, transform.position.z);
        }
        else t = 0;
	}
}
