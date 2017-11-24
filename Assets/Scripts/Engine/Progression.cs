using System.Collections.Generic;
using UnityEngine;

public struct Puzzle
{
    public string name;
    public int stars;
    public bool unlocked;
    public Game prefab;
}

public class Progression
{
    static AnimatorIntWrapper<List<Puzzle>> _puzzles;
    public static AnimatorIntWrapper<List<Puzzle>> Puzzles {
        get { return _puzzles; }
        }

    public static void Init(Animator animator)
    {
        List<Game> GamePrefabs = new List<Game>(Resources.LoadAll<Game>("Prefabs/Games/Puzzle"));
        GamePrefabs.Sort(delegate(Game a, Game b)
        {
            return a.order.CompareTo(b.order);
        });
        
        _puzzles = new AnimatorIntWrapper<List<Puzzle>>(new List<Puzzle>(), animator, "PuzzleCount", x=>x.Count);
        foreach (Game game in GamePrefabs)
        {
            Puzzle puzzle = new Puzzle() {
                name = game.name,
                stars = PlayerPrefs.GetInt(game.name + "stars", 0),
                unlocked = Puzzles.Obj.Count == 0 || PlayerPrefs.GetInt(game.name + "unlocked", 0) != 0,
                prefab = game
           };

            Puzzles.Call(x=>x.Add(puzzle));
        }

        Signals.AddListeners(OnSignal, new List<ESignalType>() {
            ESignalType.PuzzleComplete });
    }


    static private void OnSignal(ESignalType arg0, object stars)
    {
        PuzzleComplete(Game.currentGameIndex, (int)stars);
    }

	// Use this for initialization
    static void PuzzleComplete(int puzzleIndex, int stars)
    {
        Puzzle puzzle;
        puzzle.name = Puzzles.Obj[puzzleIndex].name;
        puzzle.stars = Mathf.Max(stars, Puzzles.Obj[puzzleIndex].stars);
        puzzle.unlocked = true;
        puzzle.prefab = Puzzles.Obj[puzzleIndex].prefab;

        if (Puzzles.Obj[puzzleIndex].stars < stars)
        {
            PlayerPrefs.SetInt(Puzzles.Obj[puzzleIndex].name + "stars", puzzle.stars);
        }
        if(puzzleIndex < Puzzles.Obj.Count - 1)
            PlayerPrefs.SetInt(Puzzles.Obj[puzzleIndex + 1].name + "unlocked", 1);
	}


    internal static void UnlockAll()
    {
        for (int i = 0; i < Puzzles.Obj.Count; i++)
        {
            Game.currentGameIndex = i;
            Signals.Invoke(ESignalType.PuzzleComplete, 3);
            //Instance.PuzzleComplete(i, 3);
        }
    }
}
