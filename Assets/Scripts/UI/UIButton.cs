using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButton : UIThing {

    public UISignals.UISignal signal;
    UISignals UISignals;

	// Use this for initialization
    protected void Start()
    {
        base.Start();
        UISignals = GetComponentInParent<UISignals>();
        GetComponent<Button>().onClick.AddListener(Click);
	}

    void Click()
    {
        UISignals.Click(signal);
    }
    /*public override void OnStateChanged(UIStates.State newState)
    {
        switch (newState)
        {
            case UIStates.State.Active:
                gameObject.SetActive(true);
                GetComponent<Button>().interactable = true;
                break;
            case UIStates.State.Disabled:
                gameObject.SetActive(true);
                GetComponent<Button>().interactable = false;
                break;
            case UIStates.State.Hidden:
                gameObject.SetActive(false);
                GetComponent<Button>().interactable = false;
                break;

        }
    }*/
   
}
