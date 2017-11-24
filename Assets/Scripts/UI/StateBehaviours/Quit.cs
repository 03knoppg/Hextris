using UnityEngine.SceneManagement;

public class Quit : HextrisStateMachineBehaviour {

    protected override void OnEnter()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
