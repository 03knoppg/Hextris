public class StateStart : HextwistStateMachineBehaviour
{
    protected override void OnEnter()
    {
        Animator.SetTrigger("SelectBoard");
    }
}
