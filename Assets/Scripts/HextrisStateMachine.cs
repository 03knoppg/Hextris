using DTAnimatorStateMachine;
using UnityEngine;

public class HextrisStateMachine : MonoBehaviour {


    [SerializeField]
    Animator animator;

    void Awake()
    {
        this.ConfigureAllStateBehaviours(animator);
    }
}
