using UnityEngine.SceneManagement;

public class StateQuit : HextwistStateMachineBehaviour {

    protected override void OnEnter()
    {
        Game.currentGame?.End();

        SceneManager.LoadScene("Title");
    }
}
