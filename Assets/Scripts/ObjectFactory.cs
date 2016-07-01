using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectFactory : MonoBehaviour {


    static ObjectFactory Instance;

    [SerializeField]
    Signals UISignals;
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

    public static GameHex GameHex()
    {
        GameHex gHex = Instantiate<GameHex>(Instance.GameHexPrefab);

        return gHex;
    }

    public static Piece Piece(Piece piecePrefab, Player owner, int startRotation, OffsetCoord? startPosition = null)
    {
        Piece piece = Instantiate<Piece>(piecePrefab);
        piece.name = piecePrefab.name + " " + owner.Name;
        piece.Init(startRotation, startPosition);
        owner.pieces.Add(piece);
        return piece;

    }

    public static Board Board(Board boardPrefab)
    {
        Board board = Instantiate<Board>(boardPrefab);
        board.Init(Instance.UISignals);

        return board;
    }
}
