using System.Collections.Generic;
using UnityEngine;

public class Progression
{
    public static Game PvPGamePrefab;

    public static List<Game> Puzzles {
        get { return HextwistStateMachine.Puzzles; }
        set { HextwistStateMachine.Puzzles = value; }
    }

    public static void Init(Animator animator)
    {
        PvPGamePrefab = Resources.Load<Game>("Prefabs/Games/PvP/PvPGame");

        List<Game> PuzzlePrefabs = new List<Game>(Resources.LoadAll<Game>("Prefabs/Games/Puzzle"));
        PuzzlePrefabs.Sort(delegate(Game a, Game b)
        {
            return a.order.CompareTo(b.order);
        });
        
        Puzzles = PuzzlePrefabs;
        Puzzles[0].Unlocked = true;

        Signals.AddListeners(OnSignal, new List<ESignalType>() {
            ESignalType.PuzzleComplete });
    }

    internal static void ClearProgression()
    {
        PlayerPrefs.DeleteAll();
        Puzzles[0].Unlocked = true;
    }

    static private void OnSignal(ESignalType arg0, object stars)
    {
        PuzzleComplete(Game.currentGameIndex, (int)stars);
    }
    
    static void PuzzleComplete(int puzzleIndex, int stars)
    {
        Puzzles[puzzleIndex].Stars = Mathf.Max(Puzzles[puzzleIndex].Stars, stars);

        if (puzzleIndex < Puzzles.Count - 1)
            Puzzles[puzzleIndex + 1].Unlocked = true;

    }


    internal static void UnlockAll()
    {
        for (int i = 0; i < Puzzles.Count; i++)
        {
            Game.currentGameIndex = i;
            Puzzles[i].Unlocked = true;
            Puzzles[i].Stars = 3;
        }
    }
}
