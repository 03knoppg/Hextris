using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Driver : MonoBehaviour
{

    Signals Signals;
    UIStates UIState;

    Game currentGame;
    public static int currentGameIndex;

    void Awake()
    {
        UIState = gameObject.GetComponent<UIStates>();
        Signals = gameObject.GetComponent<Signals>();
    }
    void Start()
    {

        Signals.AddListeners(OnSignal, new List<Signal>() { 
            Signal.ShowBoardSelect,
            Signal.SelectBoard,
            Signal.Quit,
            Signal.Restart });

        Signals.AddListeners(OnSignalOne, new List<Signal>() { 
            Signal.SelectBoard });

        Invoke("LevelSelect", 0.1f);
    }

    void LevelSelect()
    {
        if (Progression.Puzzles.Count > 1)
            Signals.Invoke(Signal.ShowBoardSelect);
        else
            StartGame(0);

    }

    private void OnSignal(Signal signal)
    {
        switch (signal)
        {
            case Signal.ShowBoardSelect:
                UIState.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
                UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Hidden);
                UIState.SetGroupState(UIStates.Group.PuzzleSelection, UIStates.State.Active);
                break;
            case Signal.SelectBoard:
                currentGameIndex = ++currentGameIndex;
                StartGame(currentGameIndex);
                break;
            case Signal.Quit:
                SceneManager.LoadScene("TitleScreen");
                break;
            case Signal.Restart:
                StartGame(currentGameIndex);
                break;
        }
    }

    private void OnSignalOne(Signal signal, object arg1)
    {
        switch (signal)
        {
            case Signal.SelectBoard:
                currentGameIndex = (int)arg1;
                StartGame(currentGameIndex);
                break;
        }
    }

    private void StartGame(int index)
    {
        if (currentGame != null)
            currentGame.End();

        currentGame = ObjectFactory.Game(Progression.Puzzles[currentGameIndex].prefab);

        Signals.Invoke(Signal.GameStart, index);
    }

}

 


