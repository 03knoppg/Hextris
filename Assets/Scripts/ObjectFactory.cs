using UnityEngine;

public class ObjectFactory : MonoBehaviour {


    static ObjectFactory Instance;
    
    [SerializeField]
    GameHex GameHexPrefab;

	// Use this for initialization
	void Awake() {
        Instance = this;
	}

    public static Game Game(Game GamePrefab)
    {
        Game game = Instantiate(GamePrefab);
        game.Init();
        return game;
    }

    public static GameHex GameHex()
    {
        GameHex gHex = Instantiate(Instance.GameHexPrefab);

        return gHex;
    }
    
    public static Piece Piece(Game.PieceData startStruct)
    {
        Piece piece = Instantiate(startStruct.piecePrefab);
        piece.Init(startStruct.startRotation, startStruct.startPosition);

        piece.lockSelected = startStruct.lockSelected;
        piece.lockPivotHex = startStruct.lockPivotHex;

        return piece;

    }

    public static Board Board(Board boardPrefab)
    {
        Board board = Instantiate(boardPrefab);
        board.Init();

        return board;
    }
}
