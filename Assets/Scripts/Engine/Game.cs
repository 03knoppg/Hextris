using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    public enum GameType
    {
        Classic
    }

    enum GamePhase
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
        UISignals = GameObject.FindObjectOfType<UISignals>();
        UISignals.OnEndTurn += NextPlayer;

        currentBoard = GameObject.Instantiate<Board>(BoardPrefab);

        players = new List<Player>();
        for (int i = 0; i < numPlayers; i++)
        {
            Player p = new Player();
            players.Add(p);
            p.Name = "Player" + i;
        }

        currentPlacementPieceIndex = 0;
        currentPhase = GamePhase.Setup;
        MakeNextPlacementPiece();
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
            //Debug.logger.Log(rayDistance + " " + hit);
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
            
        Piece piece = PieceMaker.Make(shapes[Mathf.FloorToInt(currentPlacementPieceIndex / 2)]);
        piece.mode = Piece.Mode.Placement;

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
