using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButton : UIThing {

    public UISignals.UISignal signal;
    protected UISignals UISignals;

	// Use this for initialization
    protected new void Start()
    {
        base.Start();
        UISignals = FindObjectOfType<UISignals>();
        GetComponentInChildren<Button>().onClick.AddListener(Click);
	}

    protected virtual void Click()
    {
        UISignals.Click(signal);
    }
}
