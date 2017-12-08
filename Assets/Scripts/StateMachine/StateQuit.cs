using UnityEngine.SceneManagement;

public class StateQuit : HextrisStateMachineBehaviour {

    protected override void OnEnter()
    {
        Game.currentGame?.End();

        SceneManager.LoadScene("Title");
    }
}
