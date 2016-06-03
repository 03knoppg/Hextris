using UnityEngine;
using System.Collections;

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

    public static Piece Piece(Layout layout, Piece.Shape shape, Player owner, int startRotation)
    {
        Piece piece = Instantiate<Piece>(Instance.PiecePrefab);
        piece.name = shape + " " + owner.Name;
        piece.Init(layout, shape, startRotation);
        owner.pieces.Add(piece);
        return piece;

    }

    public static Board Board(Board boardPrefab, Layout globalLayout)
    {
        Board board = Instantiate<Board>(boardPrefab);
        board.InitBoard(globalLayout, Instance.UISignals);

        return board;
    }
}
