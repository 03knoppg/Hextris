using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectFactory : MonoBehaviour {


    static ObjectFactory Instance;

    [SerializeField]
    UISignals UISignals;
    [SerializeField]
    Piece PiecePrefab;
    [SerializeField]
    GameHex GameHexPrefab;

	// Use this for initialization
	void Awake() {
        Instance = this;
	}

    public static Game Game(Game GamePrefab)
    {
        return Instantiate<Game>(GamePrefab);
    }

    public static GameHex GameHex(Layout GlobalLayout)
    {
        GameHex gHex = Instantiate<GameHex>(Instance.GameHexPrefab);
        gHex.Init(GlobalLayout);

        return gHex;
    }

    public static Piece Piece(Piece piecePrefab, Layout layout, Player owner, int startRotation)
    {
        Piece piece = Instantiate<Piece>(piecePrefab);
        piece.name = piecePrefab.name + " " + owner.Name;
        piece.Init(layout, startRotation);
        owner.pieces.Add(piece);
        return piece;

    }

    public static Board Board(Board boardPrefab, Layout globalLayout)
    {
        Board board = Instantiate<Board>(boardPrefab);
        board.InitBoard(globalLayout, Instance.UISignals);

        return board;
    }

    public static HexListWrapper HexListWrapper()
    {
        HexListWrapper HexListWrapper = ScriptableObject.CreateInstance<HexListWrapper>();
        HexListWrapper.GameHexes = new List<GameHex>();
        HexListWrapper.Hexes = new List<Hex>();

        return HexListWrapper;
    }
}
