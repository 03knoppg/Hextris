public class StatePuzzleStart : HextrisStateMachineBehaviour
{
    protected override void OnEnter()
    {
        Progression.Init(Animator);

        Animator.SetTrigger("SelectBoard");
    }
}
