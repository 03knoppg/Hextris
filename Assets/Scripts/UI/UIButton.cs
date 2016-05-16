using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButton : MonoBehaviour {

    public UISignals.UISignal signal;
    public UIStates.Group group;

    UISignals UISignals;
    UIStates UIState;

	// Use this for initialization
    void Start()
    {
        UISignals = GetComponentInParent<UISignals>();
        UIState = GetComponentInParent<UIStates>();

        UIState.GetEvent(group).AddListener(OnStateChanged);
        GetComponent<Button>().onClick.AddListener(Click);
	}

    void Click()
    {
        UISignals.Click(signal);
    }

    void OnStateChanged(UIStates.State newState) 
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
    }
}
