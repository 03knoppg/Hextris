using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButton : UIThing {

    public Signal signal;

	// Use this for initialization
    protected new void Start()
    {
        base.Start();

        GetComponentInChildren<Button>().onClick.AddListener(Click);
	}

    protected virtual void Click()
    {
        UISignals.Invoke(signal);
    }
}
