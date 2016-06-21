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
    public int currentGameIndex;

    void Awake()
    {

        UIState = gameObject.GetComponent<UIStates>();
        UISignals = gameObject.GetComponent<UISignals>();

        GamePrefabs = new List<Game>(Resources.LoadAll<Game>("Prefabs/Games/Puzzle"));
        GamePrefabs.RemoveAll(game => game.type != Game.GameType.Puzzle);
        GamePrefabs.Sort(delegate(Game a, Game b)
        {
            return a.order.CompareTo(b.order);
        });
    }
    void Start()
    {

        UISignals.AddListeners(OnUISignal, new List<UISignal>() { 
            UISignal.SelectBoard,
            UISignal.ShowBoardSelect,
            UISignal.Quit,
            UISignal.Restart });

        Invoke("LevelSelect", 0.1f);
    }

    void LevelSelect()
    {
        if (GamePrefabs.Count > 1)
            UISignals.Click(UISignal.ShowBoardSelect);
        else
            StartGame(0);

    }

    private void OnUISignal(UISignal signal, object arg1)
    {
        switch (signal)
        {
            case UISignal.SelectBoard:
                currentGameIndex = (int)(arg1 ?? ++currentGameIndex);
                StartGame(currentGameIndex);
                break;
            case UISignal.ShowBoardSelect:
                UIState.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
                UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Hidden);
                UIState.SetGroupState(UIStates.Group.PuzzleSelection, UIStates.State.Active);
                break;
            case UISignal.Quit:
                SceneManager.LoadScene("TitleScreen");
                break;
            case UISignal.Restart:
                StartGame(currentGameIndex);
                break;
        }
    }

    private void StartGame(int index)
    {
        if (currentGame != null)
            currentGame.End();

        currentGame = ObjectFactory.Game(GamePrefabs[currentGameIndex]);
    }

}

 


