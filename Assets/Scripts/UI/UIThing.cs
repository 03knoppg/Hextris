using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(CanvasGroup))]
public class UIThing : MonoBehaviour {
    [SerializeField]
    UIStates.State state;

    public UIStates.State State { get { return state; } set { OnStateChanged(value); } }
    public UIStates.Group group;
    protected UIStates UIState;
    protected UISignals UISignals;

    Animator animator;
    

	// Use this for initialization
    protected void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        UIState = FindObjectOfType<UIStates>();
        UISignals = FindObjectOfType<UISignals>();
        if(group != UIStates.Group.None)
            UIState.GetEvent(group).AddListener(OnStateChanged);

        //force animator into startstate
        UIStates.State startState = state;
        state = UIStates.State.None;
        OnStateChanged(startState);


	}

    protected virtual void OnStateChanged(UIStates.State newState)
    {
        if (state == newState)
            return;

        switch (newState)
        {
            case UIStates.State.Active:
                animator.SetTrigger("Active");
                break;
            case UIStates.State.Disabled:
                animator.SetTrigger("Disabled");
                break;
            case UIStates.State.Hidden:
                animator.SetTrigger("Hidden");
                break;
        }

        state = newState;
    }
}
