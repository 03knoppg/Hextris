using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

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
    Piece currentSelectedPiece;
    int currentPlacementPieceIndex;
    int currentPlayerNum;

    [SerializeField]
    GameType type;

    [SerializeField]
    Board BoardPrefab;

    [SerializeField]
    UISignals UISignals;
    UIStates UIState;

    //move these to gameType structure
    int numPlayers = 1;
    List<PieceMaker.Shape> shapes = new List<PieceMaker.Shape>()
    {
        //PieceMaker.Shape.I,
        PieceMaker.Shape.L
    };

    public void StartGame()
    {
        UIState = FindObjectOfType<UIStates>();
        UISignals = FindObjectOfType<UISignals>();

        UISignals.AddListeners(UIClick, new List<UISignals.UISignal>() { UISignals.UISignal.RotateCCW, UISignals.UISignal.RotateCW, UISignals.UISignal.EndTurn });

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

    public void UIClick(UISignals.UISignal signal)
    {
        switch (signal)
        {
            case UISignals.UISignal.EndTurn:
                NextPlayer();
                break;
            case UISignals.UISignal.RotateCCW:
                currentSelectedPiece.RotateCCW();
                break;
            case UISignals.UISignal.RotateCW:
                currentSelectedPiece.RotateCW();
                break;
        }

    }

    void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        if (OnGamePhaseChange != null)
            OnGamePhaseChange(currentPhase);
    }

    void PiecePlaced(Piece piece)
    {
        piece.Mode = Piece.EMode.Inactive;
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

                currentSelectedPiece.Point = p;
                if (IsValidPosition(currentSelectedPiece))
                    currentSelectedPiece.SetColor(Color.green);
                else
                    currentSelectedPiece.SetColor(Color.red);

            }
        }

        UpdateUIState();
    }

    void UpdateUIState()
    {
        if (currentPhase == GamePhase.Setup)
        {
            UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Hidden);
        }
        else if (currentPhase == GamePhase.Main)
        {
            bool anyTurning = false;
            bool allLegal = true;
            foreach (Piece piece in players[currentPlayerIndex].pieces)
            {
                anyTurning |= piece.rotationRate != 0;
                allLegal &= IsValidPosition(piece);
            }

            if (anyTurning || currentSelectedPiece == null)
                UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Disabled);
            else
                UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Active);


            if (allLegal && !anyTurning)
                UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Active);
            else
                UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Disabled);
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
        piece.Mode = Piece.EMode.Placement;
        piece.OnPieceClicked += OnPieceClicked;

        players[currentPlayerNum].pieces.Add(piece);

        currentSelectedPiece = piece;
        currentPlacementPieceIndex++;
    }

    private void OnPieceClicked(Piece piece, GameHex hex)
    {
        if (currentPhase == GamePhase.Setup)
        {
            if (piece == currentSelectedPiece && IsValidPosition(piece))
                PiecePlaced(piece);
        }
        if (currentPhase == GamePhase.Main)
        {

            if (!hex.IsPivotHex)
                piece.SetPivotHex(hex);
            

            if (piece != currentSelectedPiece && piece.Mode == Piece.EMode.Active)
            {
                if (currentSelectedPiece != null)
                {
                    currentSelectedPiece.Mode = Piece.EMode.Active;
                    currentSelectedPiece.ResetRotation();
                }

                piece.Mode = Piece.EMode.Selected;
                currentSelectedPiece = piece;
            }
        }
    }

    void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
        currentSelectedPiece.LockRotation();
        currentSelectedPiece = null;

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
                if (!currentBoard.InBounds(hex))
                    return false;

                foreach (Hex legalHex in currentPlayerNum == 0 ? currentBoard.legalStartingHexesP1 : currentBoard.legalStartingHexesP2)
                {
                    if (hex.Equals(legalHex))
                        touchingStartArea = true;
                }
            }
            return touchingStartArea;

        }
        else if (currentPhase == GamePhase.Main)
        {
            foreach (GameHex gHex in piece.hexes)
            {
                Hex hex = FractionalHex.HexRound(Layout.PixelToHex(Driver.layout,  gHex.GlobalPoint));

                if (!currentBoard.InBounds(hex))
                    return false;
                
            }
            return true;
        }
        return false;
    }
}
