using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    public delegate void GamePhaseChange(GamePhase newPhase);
    public GamePhaseChange OnGamePhaseChange;

    public PieceMaker PieceMakerPrefab;

    public List<Hex> legalStartingHexes;

    public enum GameType
    {
        Classic
    }

    public enum GamePhase
    {
        Setup,
        Main
    }

    List<Player> players;
    int currentPlayerIndex = -1;

    Board currentBoard;
    GamePhase currentPhase;
    Piece currentPlacementPiece;
    int currentPlacementPieceIndex;


    [SerializeField]
    GameType type;

    [SerializeField]
    Board BoardPrefab;

    [SerializeField]
    UISignals UISignals;

    //move these to gameType structure
    int numPlayers = 2;
    List<PieceMaker.Shape> shapes = new List<PieceMaker.Shape>()
    {
        PieceMaker.Shape.I,
        PieceMaker.Shape.L
    };

    public void StartGame()
    {
        UISignals = FindObjectOfType<UISignals>();
        UISignals.OnEndTurn += NextPlayer;

        currentBoard = Instantiate<Board>(BoardPrefab);

        players = new List<Player>();
        for (int i = 0; i < numPlayers; i++)
        {
            Player p = new Player();
            players.Add(p);
            p.Name = "Player" + i;
        }

        currentPlacementPieceIndex = 0;
        SetPhase(GamePhase.Setup);
        MakeNextPlacementPiece();
    }

    void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        if (OnGamePhaseChange != null)
            OnGamePhaseChange(currentPhase);
    }

    void PiecePlaced()
    {
        currentPlacementPiece.mode = Piece.Mode.Inactive;
        MakeNextPlacementPiece();
    }

    void Update()
    {
        if (currentPhase == GamePhase.Setup)
        {
            if (Input.GetMouseButtonUp(0))
            {
                PiecePlaced();
            }
            Plane boardPlane = new Plane(Vector3.up, currentBoard.transform.position);
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            bool hit = boardPlane.Raycast(ray, out rayDistance);

            if (hit)
            {
                Vector3 point = ray.GetPoint(rayDistance);
                FractionalHex fHex = Layout.PixelToHex(Driver.layout, new Point(point.x, point.z));
                Point p = Layout.HexToPixel(Driver.layout, FractionalHex.HexRound(fHex));
                currentPlacementPiece.SetPosition(p);

            }
            
        }
    }

    void MakeNextPlacementPiece()
    {
        if (Mathf.FloorToInt(currentPlacementPieceIndex / 2) == shapes.Count)
        {
            currentPhase = GamePhase.Main;
            NextPlayer();
            return;
        }
            
        Piece piece = PieceMakerPrefab.Make(shapes[Mathf.FloorToInt(currentPlacementPieceIndex / 2)]);
        piece.mode = Piece.Mode.Placement;
        piece.GlobalLayout = Driver.layout;
        piece.legalStartingHexes = legalStartingHexes;

        //for two players go 01100110 etc.
        players[Mathf.FloorToInt((currentPlacementPieceIndex + 1) /2) % 2].pieces.Add(piece);


        currentPlacementPiece = piece;
        currentPlacementPieceIndex++;
    }

    void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetActivePlayer(currentPlayerIndex == i);
        }
    }
}
