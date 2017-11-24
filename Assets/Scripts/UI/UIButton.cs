using UnityEngine.UI;

public class UIButton : UIThing {

    public ESignalType signal;

    protected Button button;

	// Use this for initialization
    protected new void Start()
    {
        base.Start();

        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(Click);
	}

    protected virtual void Click()
    {
        Signals.Invoke(signal);
    }
}
