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
        Main,
        End
    }

    List<Player> players;
    int currentPlayerIndex = 0;

    Board currentBoard;
    GamePhase currentPhase;
    Piece currentSelectedPiece;

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
        PieceMaker.Shape.C,
        PieceMaker.Shape.S,
        //PieceMaker.Shape.Triangle
    };

    public void StartGame()
    {
        UIState = FindObjectOfType<UIStates>();
        UISignals = FindObjectOfType<UISignals>();

        UISignals.AddListeners(UIClick, new List<UISignals.UISignal>() { UISignals.UISignal.RotateCCW, UISignals.UISignal.RotateUndo, UISignals.UISignal.RotateCW, UISignals.UISignal.EndTurn });

        currentBoard = Instantiate<Board>(BoardPrefab);

        players = new List<Player>();
        for (int i = 0; i < numPlayers; i++)
        {
            Player p = new Player();
            players.Add(p);
            p.Name = "Player" + i;
        }

        SetPhase(GamePhase.Setup);
        MakeNextPlacementPiece();
    }

    public void UIClick(UISignals.UISignal signal)
    {
        switch (signal)
        {
            case UISignals.UISignal.EndTurn:
                if (IsPlayerWin())
                {
                    SetPhase(GamePhase.End);
                    return;
                }
                else
                    NextPlayer();
                break;
            case UISignals.UISignal.RotateCCW:
                currentSelectedPiece.RotateCCW();
                break;
            case UISignals.UISignal.RotateCW:
                currentSelectedPiece.RotateCW();
                break;
            case UISignals.UISignal.RotateUndo:
                currentSelectedPiece.ResetRotation();
                break;
        }

    }

    void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        if (OnGamePhaseChange != null)
            OnGamePhaseChange(currentPhase);

        if(newPhase == GamePhase.Setup)
            UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Hidden);

        if (newPhase == GamePhase.End)
        {
            UIState.winner = currentPlayerIndex;
            UIState.SetGroupState(UIStates.Group.EndGame, UIStates.State.Active);
        }
    }

    void PiecePlaced(Piece piece)
    {
        piece.Mode = Piece.EMode.Inactive;
        MakeNextPlacementPiece();
    }

    void Update()
    {
        

        UpdateUIState();
        UpdatePieceMode();
    }

    void UpdateUIState()
    {
        if (currentPhase == GamePhase.Main)
        {
            bool anyTurning = false;
            bool allLegal = true;
            foreach (Piece piece in players[currentPlayerIndex].pieces)
            {
                anyTurning |= piece.rotationRate != 0;
                allLegal &= IsValidPosition(piece);
            }

            bool hasTurned = currentSelectedPiece != null && currentSelectedPiece.rotation != 0;

            if (anyTurning || currentSelectedPiece == null)
                UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Disabled);
            else
                UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Active);


            if (allLegal && !anyTurning && hasTurned)
                UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Active);
            else
                UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Disabled);
        }
    }

    void UpdatePieceMode()
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
                    currentSelectedPiece.Mode = Piece.EMode.PlacementValid;
                else
                    currentSelectedPiece.Mode = Piece.EMode.PlacementInvalid;

            }
        }
        else if (currentPhase == GamePhase.Main)
        {
            foreach (Player player in players)
            {
                foreach (Piece piece in player.pieces)
                {
                    if (player != players[currentPlayerIndex])
                        piece.Mode = Piece.EMode.Disabled;

                    else
                    {
                        if (currentSelectedPiece == piece)
                            piece.Mode = Piece.EMode.Selected;

                        else if (currentSelectedPiece == null || currentSelectedPiece.rotation == 0)
                            piece.Mode = Piece.EMode.Active;

                        else
                            piece.Mode = Piece.EMode.Inactive;

                    }
                }
            }
        }

    }

    void MakeNextPlacementPiece()
    {
        int totalPieces = 0;
        foreach (Player player in players)
            totalPieces += player.pieces.Count;

        //have all the pieces been placed?
        if (Mathf.FloorToInt(totalPieces / numPlayers) == shapes.Count)
        {
            //start with player 0
            currentPlayerIndex = -1;
            currentPhase = GamePhase.Main;
            NextPlayer();
            return;
        }

        //for two players go 01100110 etc.
        currentPlayerIndex = Mathf.Clamp(Mathf.FloorToInt(totalPieces / numPlayers) % 2 == 0 ?
            totalPieces % numPlayers :
            (numPlayers - 1) - totalPieces % numPlayers, 0, (numPlayers - 1));

        Piece piece = PieceMakerPrefab.Make(shapes[Mathf.FloorToInt(totalPieces / 2)]);
        piece.name = shapes[Mathf.FloorToInt(totalPieces / 2)] + " Player" + (currentPlayerIndex + 1);
        piece.OnPieceClicked += OnPieceClicked;

        players[currentPlayerIndex].pieces.Add(piece);

        currentSelectedPiece = piece;


        UIState.currentPlayer = currentPlayerIndex;
    }

    private void OnPieceClicked(Piece piece, GameHex hex)
    {
        if (currentPhase == GamePhase.Setup)
        {
            if (piece == currentSelectedPiece && IsValidPosition(piece))
                PiecePlaced(piece);
        }
        else if (currentPhase == GamePhase.Main)
        {

            if (!hex.IsPivotHex)
            {
                if(piece.rotation == 0)
                    piece.SetPivotHex(hex);
            }

            if (piece != currentSelectedPiece && piece.Mode == Piece.EMode.Active)
            {
                if (currentSelectedPiece != null)
                {
                    currentSelectedPiece.ResetRotation();
                }

                currentSelectedPiece = piece;
            }
        }
    }

    void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
        UIState.currentPlayer = currentPlayerIndex;

        if (currentSelectedPiece != null)
        {
            currentSelectedPiece.LockRotation();
            currentSelectedPiece = null;
        }

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetActivePlayer(currentPlayerIndex == i);
        }


    }

    public bool IsValidPosition(Piece piece)
    {
        if (currentPhase == GamePhase.Setup)
        {
            foreach (Player player in players)
            {
                foreach (Piece otherPiece in player.pieces)
                {
                    if (otherPiece == piece)
                        continue;

                    foreach (GameHex otherHex in otherPiece.hexes)
                    {
                        foreach (GameHex hex in piece.hexes)
                        {
                            if (otherHex == hex)
                                return false;
                        }
                    }
                }
            }
            return IsPieceInArea(piece, currentPlayerIndex == 0 ? currentBoard.legalStartingHexesP1 : currentBoard.legalStartingHexesP2);
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

    public bool IsPlayerWin()
    {
        foreach (Piece piece in players[currentPlayerIndex].pieces)
        {
            if (!IsPieceInArea(piece, currentPlayerIndex == 0 ? currentBoard.legalStartingHexesP2 : currentBoard.legalStartingHexesP1))
                return false;
        }

        return true;
    }

    public bool IsPieceInArea(Piece piece, List<Hex> hexes)
    {
        bool touchingLegalArea = false;
        foreach (GameHex gHex in piece.hexes)
        {
            Hex globalHex = FractionalHex.HexRound(Layout.PixelToHex(Driver.layout, gHex.GlobalPoint));
            if (!currentBoard.InBounds(globalHex))
                return false;

            if (!touchingLegalArea)
            {
                foreach (Hex legalHex in hexes)
                {
                    if (globalHex.Equals(legalHex))
                    {
                        touchingLegalArea = true;
                        break;
                    }
                }
            }
        }
        return touchingLegalArea;
    }

}
