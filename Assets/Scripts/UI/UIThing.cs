using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(CanvasGroup))]
public class UIThing : MonoBehaviour {
    public UIStates.State state;
    public UIStates.Group group;
    protected UIStates UIState;

    Animator animator;
    

	// Use this for initialization
    protected void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        UIState = GetComponentInParent<UIStates>();
        if(group != UIStates.Group.None)
            UIState.GetEvent(group).AddListener(OnStateChanged);

        //force transition into startstate
        UIStates.State startState = state;
        state = UIStates.State.None;
        OnStateChanged(startState);


	}

    public virtual void OnStateChanged(UIStates.State newState)
    {
        //if (state == newState)
            //return;

        switch (newState)
        {
            case UIStates.State.Active:
                gameObject.SetActive(true);
                animator.SetTrigger("Active");
                break;
            case UIStates.State.Disabled:
                gameObject.SetActive(true);
                animator.SetTrigger("Disabled");
                break;
            case UIStates.State.Hidden:
                gameObject.SetActive(false);
                animator.SetTrigger("Hidden");
                break;
        }

        state = newState;
    }
}
