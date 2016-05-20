using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

    public void LoadPvP()
    {
        SceneManager.LoadScene("PvP");
    }
    public void LoadPuzzle()
    {
        SceneManager.LoadScene("Puzzle");
    }
}
