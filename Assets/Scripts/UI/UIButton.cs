using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButton : MonoBehaviour {

    public UISignals.UISignal button;

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(Click);
	}

    void Click()
    {
        GetComponentInParent<UISignals>().Click(button);
    }
	
}
