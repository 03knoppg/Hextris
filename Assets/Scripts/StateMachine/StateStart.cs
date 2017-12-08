public class StateStart : HextrisStateMachineBehaviour
{
    protected override void OnEnter()
    {
        Animator.SetTrigger("SelectBoard");
    }
}
