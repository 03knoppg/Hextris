using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Puzzle
{
    public string name;
    public int stars;
    public bool unlocked;
    public Game prefab;
}

public class Progression : MonoBehaviour{

    

    static Progression instance;
    static Progression Instance
    { get { return instance; } }

    List<Puzzle> puzzles;
    public static List<Puzzle> Puzzles { get { return Instance.puzzles; } }

    Signals UISignals;

    void Awake()
    {
        instance = this;

        List<Game> GamePrefabs = new List<Game>(Resources.LoadAll<Game>("Prefabs/Games/Puzzle"));
        GamePrefabs.RemoveAll(game => game.type != Game.GameType.Puzzle);
        GamePrefabs.Sort(delegate(Game a, Game b)
        {
            return a.order.CompareTo(b.order);
        });

        puzzles = new List<Puzzle>();
        foreach (Game game in GamePrefabs)
        {
            Puzzle puzzle = new Puzzle();
            puzzle.name = game.name;
            puzzle.stars = PlayerPrefs.GetInt(puzzle.name + "stars", 0);
            if (puzzles.Count == 0) //first level is always unlocked
                puzzle.unlocked = true;
            else
                puzzle.unlocked = PlayerPrefs.GetInt(puzzle.name + "unlocked", 0) != 0;

            puzzle.prefab = game;
            puzzles.Add(puzzle);
        }
    }
    void Start()
    {
        UISignals = gameObject.GetComponent<Signals>();
        UISignals.AddListeners(OnSignal, new List<Signal>() { 
            Signal.PuzzleComplete });
    }

    private void OnSignal(Signal arg0, object stars)
    {
        PuzzleComplete(Driver.currentGameIndex, (int)stars);
    }

	// Use this for initialization
    void PuzzleComplete(int puzzleIndex, int stars)
    {
        Puzzle puzzle;
        puzzle.name = puzzles[puzzleIndex].name;
        puzzle.stars = Mathf.Max(stars, puzzles[puzzleIndex].stars);
        puzzle.unlocked = true;
        puzzle.prefab = puzzles[puzzleIndex].prefab;

        if (puzzles[puzzleIndex].stars < stars)
        {
            PlayerPrefs.SetInt(puzzles[puzzleIndex].name + "stars", puzzle.stars);
        }
        if(puzzleIndex < puzzles.Count - 1)
            PlayerPrefs.SetInt(puzzles[puzzleIndex + 1].name + "unlocked", 1);
	}


    internal static void UnlockAll()
    {
        for (int i = 0; i < Puzzles.Count; i++)
        {
            Driver.currentGameIndex = i;
            Instance.UISignals.Invoke(Signal.PuzzleComplete, 3);
            //Instance.PuzzleComplete(i, 3);
        }
    }
}
