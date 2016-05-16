using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    public delegate void GamePhaseChange(GamePhase newPhase);
    public GamePhaseChange OnGamePhaseChange;

    public PieceMaker PieceMakerPrefab;
    

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
    int currentPlayerNum;

    [SerializeField]
    GameType type;

    [SerializeField]
    Board BoardPrefab;

    [SerializeField]
    UISignals UISignals;

    //move these to gameType structure
    int numPlayers = 1;
    List<PieceMaker.Shape> shapes = new List<PieceMaker.Shape>()
    {
        //PieceMaker.Shape.I,
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

    void PiecePlaced(Piece piece)
    {
        piece.mode = Piece.Mode.Inactive;
        MakeNextPlacementPiece();
    }

    void Update()
    {
        if (currentPhase == GamePhase.Setup)
        {
            Plane boardPlane = new Plane(Vector3.up, currentBoard.transform.position);
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            bool hit = boardPlane.Raycast(ray, out rayDistance);

            if (hit)
            {
                Vector3 point = ray.GetPoint(rayDistance);
                FractionalHex fHex = Layout.PixelToHex(Driver.layout, new Point(point.x, point.z));
                Point p = Layout.HexToPixel(Driver.layout, FractionalHex.HexRound(fHex));
                currentPlacementPiece.Point = p;
                if (IsValidPosition(currentPlacementPiece))
                    currentPlacementPiece.SetColor(Color.green);
                else
                    currentPlacementPiece.SetColor(Color.red);

            }
            
        }
    }

    void MakeNextPlacementPiece()
    {
        //have all the pieces been placed?
        if (Mathf.FloorToInt(currentPlacementPieceIndex / numPlayers) == shapes.Count)
        {
            currentPhase = GamePhase.Main;
            NextPlayer();
            return;
        }

        //for two players go 01100110 etc.
        int p = numPlayers - 1;
        currentPlayerNum = Mathf.Clamp(Mathf.FloorToInt(currentPlacementPieceIndex / (p + 1)) % 2 == 0 ?
            currentPlacementPieceIndex % (p + 1) :
            p - currentPlacementPieceIndex % (p + 1), 0, p);
        
        Piece piece = PieceMakerPrefab.Make(shapes[Mathf.FloorToInt(currentPlacementPieceIndex / 2)]);
        piece.mode = Piece.Mode.Placement;
        piece.OnPieceClicked += OnPieceClicked;

        players[currentPlayerNum].pieces.Add(piece);

        currentPlacementPiece = piece;
        currentPlacementPieceIndex++;
    }

    private void OnPieceClicked(Piece piece, GameHex hex)
    {
        if (currentPhase == GamePhase.Setup)
        {
            PiecePlaced(piece);
        }
        if (currentPhase == GamePhase.Main)
        {
            if (piece.mode != Piece.Mode.Selected || !hex.IsPivotHex)
            {
                piece.mode = Piece.Mode.Selected;
                piece.SetPivotHex(hex);
            }
            else
            {
                piece.Rotate();
            }
        }
    }

    void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetActivePlayer(currentPlayerIndex == i);
        }
    }

    public bool IsValidPosition(Piece piece)
    {
        if (currentPhase == GamePhase.Setup)
        {
            bool touchingStartArea = false;
            foreach (GameHex gHex in piece.hexes)
            {
                Point local2GlobalPoint = gHex.LocalPoint + piece.Point;
                Hex hex = FractionalHex.HexRound(Layout.PixelToHex(Driver.layout, local2GlobalPoint));

                foreach (Hex legalHex in currentPlayerNum == 0 ? currentBoard.legalStartingHexesP1 : currentBoard.legalStartingHexesP2)
                {
                    if (hex.Equals(legalHex))
                        touchingStartArea = true;
                    if (!currentBoard.InBounds(hex))
                        return false;
                }
            }
            return touchingStartArea;

        }
        return false;
    }
}
