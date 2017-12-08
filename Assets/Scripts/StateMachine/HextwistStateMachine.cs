using DTAnimatorStateMachine;
using System.Collections.Generic;
using UnityEngine;

public class HextwistStateMachine : MonoBehaviour {


    public enum EGameType
    {
        Unselected = 0,
        PvP = 1,
        Puzzle = 2
    }

    static HextwistStateMachine singleton;

    [SerializeField]
    Animator animator;

    static AnimatorIntWrapper<EGameType> gameType;
    public static EGameType GameType
    {
        get { return gameType.Obj; }
        set { gameType.Obj = value; }
    }


    static AnimatorIntWrapper<List<Game>> _puzzlesCount;
    public static List<Game> Puzzles
    {
        get { return _puzzlesCount.Obj; }
        set { _puzzlesCount.Obj = value; }
    }

    void Awake()
    {
        if(singleton)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;

        Debug.Log("SM AWAKE");

        gameType = new AnimatorIntWrapper<EGameType>(EGameType.Unselected, animator, "GameType", x => (int)x, x => (EGameType)x );
        _puzzlesCount = new AnimatorIntWrapper<List<Game>>(new List<Game>(), animator, "PuzzleCount", x => x.Count);

        DontDestroyOnLoad(gameObject);
        this.ConfigureAllStateBehaviours(animator);

        Progression.Init(animator);
    }

}
