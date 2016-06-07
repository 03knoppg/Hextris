using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Driver : MonoBehaviour
{

    public List<Game> GamePrefabs;
    UISignals UISignals;
    UIStates UIState;

    Game currentGame;

    void Awake()
    {
        UIState = gameObject.GetComponent<UIStates>();
        UISignals = gameObject.GetComponent<UISignals>();
    }
    void Start()
    {
        
        Tests.TestAll();

        UISignals.AddListeners(OnUISignal, new List<UISignal>() { 
            UISignal.SelectBoard,
            UISignal.ShowBoardSelect,
            UISignal.Quit });

        Invoke("LevelSelect", 0.1f);
    }

    void LevelSelect()
    {
        UISignals.Click(UISignal.ShowBoardSelect);
    }

    private void OnUISignal(UISignal signal, object arg1)
    {
        switch (signal)
        {
            case UISignal.SelectBoard:
                if (currentGame != null)
                    currentGame.End();

                int index = (int)arg1;
                currentGame = ObjectFactory.Game(GamePrefabs[index]);
                break;
            case UISignal.ShowBoardSelect:
                UIState.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
                UIState.SetGroupState(UIStates.Group.PuzzleSelection, UIStates.State.Active);
                break;
            case UISignal.Quit:
                SceneManager.LoadScene("TitleScreen");
                break;
        }
    }

}

 


