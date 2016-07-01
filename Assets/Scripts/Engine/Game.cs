using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;


public class Game : MonoBehaviour {

    public delegate void GamePhaseChange(GamePhase newPhase);
    public GamePhaseChange OnGamePhaseChange;

    public float order;

    public enum GameType
    {
        Classic,
        Puzzle
    }

    public enum GamePhase
    {
        Setup,
        Main,
        End
    }


    [Serializable]
    struct StartStruct
    {
        public Piece piece;
        public int startRotation;
        public bool useStartPosition;
        public OffsetCoord startPosition;
        public bool lockPivotHex;
        public OffsetCoord lockedPivotHex;
        public bool lockSelected;
    }
    [SerializeField]
    List<StartStruct> StartStructs;

    List<Player> players;
    int currentPlayerIndex = 0;

    Board currentBoard;
    GamePhase currentPhase;
    Piece lastSelectedPiece;
    Piece currentSelectedPiece;

    [SerializeField]
    public GameType type;

    [SerializeField]
    Board BoardPrefab;

    Signals Signals;
    UIStates UIState;

    public float layoutSize = 1;
    public int numPlayers = 1;



    public Material OuterInactive;
    public Material OuterPivot;
    public Material OuterSelected;
    public Material P1InnerActive;
    public Material P1InnerPivot;
    public Material P1InnerDisabled;
    public Material P2InnerActive;
    public Material P2InnerPivot;
    public Material P2InnerDisabled;


    void Start()
    {
        Layout.defaultLayout = new Layout(Layout.pointy, new Point(layoutSize, layoutSize), new Point(0, 0));

        Signals = FindObjectOfType<Signals>();
        UIState = FindObjectOfType<UIStates>();

        Signals.AddListeners(OnUISignal, new List<Signal>() { 
            Signal.RotateCCW, 
            Signal.RotateUndo, 
            Signal.RotateCW, 
            Signal.EndTurn});

        players = new List<Player>();
        for (int i = 0; i < numPlayers; i++)
        {
            Player p = new Player();
            players.Add(p);
            p.Name = "Player" + i;
        }

        StartGame();
    }

    public void StartGame()
    {

        currentBoard = ObjectFactory.Board(BoardPrefab);

        SetPhase(GamePhase.Setup);
        MakeNextPlacementPiece();

        if (currentBoard.Hexes.Count > 0)
        {
            Bounds boardBounds = new Bounds(currentBoard.Hexes[0].transform.position, Vector3.zero);
            foreach (GameHex gHex in currentBoard.Hexes)
            {
                boardBounds.Encapsulate(new Bounds(gHex.transform.position, Vector3.one * 2));
            }

            Signals.Invoke(Signal.CamPosition, boardBounds);
        }
    }

    public void OnUISignal(Signal signal, object arg1)
    {
        switch (signal)
        {
            case Signal.EndTurn:
                OnMovementFinished();
                break;
            case Signal.RotateCCW:
                currentSelectedPiece.RotateCCW();
                break;
            case Signal.RotateCW:
                currentSelectedPiece.RotateCW();
                break;
            case Signal.RotateUndo:
                if (lastSelectedPiece != null)
                    lastSelectedPiece.UndoRotation();
                else
                    currentSelectedPiece.ResetRotation();
                break;

        }

    }

    void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        if (OnGamePhaseChange != null)
            OnGamePhaseChange(currentPhase);

        switch(newPhase)
        {

            case GamePhase.Setup:
                UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Hidden);
                UIState.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
                UIState.SetGroupState(UIStates.Group.PuzzleSelection, UIStates.State.Hidden);
            break;
            case GamePhase.Main:
                UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Active);
                break;
            case GamePhase.End:
                UIState.SetGroupState(UIStates.Group.EndGame, UIStates.State.Active);
                UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Disabled);
                foreach (Player player in players)
                {
                    player.SetActivePlayer(false);
                }
            break;
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
                anyTurning |= piece.IsRotating();
                allLegal &= IsValidPosition(piece);
            }

            bool hasTurned = currentSelectedPiece != null && currentSelectedPiece.IsRotated();

            //if (anyTurning || currentSelectedPiece == null)
            //    UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Disabled);
            //else
            //    UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Active);
            if (lastSelectedPiece != null || hasTurned)
                UIState.SetGroupState(UIStates.Group.Undo, UIStates.State.Active);
            else
                UIState.SetGroupState(UIStates.Group.Undo, UIStates.State.Disabled);

            //if (allLegal && !anyTurning && hasTurned)
            //    UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Active);
            //else
            //    UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Disabled);
        }
        else if (currentPhase == GamePhase.End)
        {
            UIState.SetGroupState(UIStates.Group.Undo, UIStates.State.Disabled);
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
                FractionalHex fHex = Layout.PixelToHex(new Point(point.x, point.z));
                Point p = Layout.HexToPixel(FractionalHex.HexRound(fHex));

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
                        if (currentSelectedPiece == piece || piece.lockSelected)
                            piece.Mode = Piece.EMode.Selected;

                        else if (currentSelectedPiece == null || !currentSelectedPiece.IsRotated())
                            piece.Mode = Piece.EMode.Active;

                        else
                            piece.Mode = Piece.EMode.Inactive;

                    }
                }
            }
        }
        else if (currentPhase == GamePhase.End)
        {
            foreach (Player player in players)
            {
                foreach (Piece piece in player.pieces)
                {
                    piece.Mode = Piece.EMode.Active;
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
        if (Mathf.FloorToInt(totalPieces / numPlayers) == StartStructs.Count)
        {
            //start with player 0
            currentPlayerIndex = -1;
            SetPhase(GamePhase.Main);
            NextPlayer();
            return;
        }

        //for two players go 01100110 etc.
        currentPlayerIndex = Mathf.Clamp(Mathf.FloorToInt(totalPieces / numPlayers) % 2 == 0 ?
            totalPieces % numPlayers :
            (numPlayers - 1) - totalPieces % numPlayers, 0, (numPlayers - 1));

        int index = Mathf.FloorToInt(totalPieces / 2);

        Piece piece = StartStructs[index].useStartPosition ?
            ObjectFactory.Piece(
                StartStructs[index].piece,
                players[currentPlayerIndex],
                StartStructs[index].startRotation,
                StartStructs[index].startPosition) :
            ObjectFactory.Piece(
                StartStructs[index].piece,
                players[currentPlayerIndex],
                StartStructs[index].startRotation);

        piece.lockSelected = StartStructs[index].lockSelected;
        piece.lockPivotHex = StartStructs[index].lockPivotHex;

        if (piece.lockPivotHex)
            piece.SetPivotHex(StartStructs[index].lockedPivotHex, true);
        

        piece.OnPieceClicked.AddListener(OnPieceClicked);
        piece.OnMovementFinished.AddListener(OnMovementFinished);
        piece.OuterInactive = OuterInactive;
        piece.OuterPivot = OuterPivot;
        piece.OuterSelected = OuterSelected;
        piece.InnerActive = currentPlayerIndex == 0 ? P1InnerActive : P2InnerActive;
        piece.InnerPivot = currentPlayerIndex == 0 ? P1InnerPivot : P2InnerPivot;
        piece.InnerDisabled = currentPlayerIndex == 0 ? P1InnerDisabled : P2InnerDisabled;

        if (StartStructs[index].useStartPosition)
            PiecePlaced(piece);
        else
        {
            currentSelectedPiece = piece;
            currentBoard.HighlightPlayer(currentPlayerIndex);

            Signals.Invoke(Signal.PlayerTurn, currentPlayerIndex);
        }
    }

    private void OnMovementFinished()
    {
        if (IsPlayerWin())
        {
            if (type == GameType.Classic)
                Signals.Invoke(Signal.PlayerWin, currentPlayerIndex);
            else
                Signals.Invoke(Signal.PuzzleComplete, 3);
            SetPhase(GamePhase.End);
            return;
        }
        else
        {
            bool anyTurning = false;
            bool allLegal = true;
            foreach (Piece piece in players[currentPlayerIndex].pieces)
            {
                anyTurning |= piece.IsRotating();
                allLegal &= IsValidPosition(piece);
            }

            bool hasTurned = currentSelectedPiece != null && currentSelectedPiece.IsRotated();
            

            if (allLegal && !anyTurning && hasTurned)
                NextPlayer();
        }
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
                if(!piece.IsRotated())
                    piece.SetPivotHex(hex);
            }

            if (piece != currentSelectedPiece && piece.Mode == Piece.EMode.Active)
            {
                if (currentSelectedPiece != null)
                {
                    currentSelectedPiece.ResetRotation();
                }
                lastSelectedPiece = null;
                currentSelectedPiece = piece;
            }
        }
    }

    void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
        
        currentBoard.HighlightPlayer(currentPlayerIndex + 1);
        Signals.Invoke(Signal.PlayerTurn, currentPlayerIndex);

        if (currentSelectedPiece != null)
        {
            currentSelectedPiece.LockRotation();
            if(numPlayers == 1)
                lastSelectedPiece = currentSelectedPiece;
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

                    foreach (GameHex otherHex in otherPiece.GameHexes)
                    {
                        foreach (GameHex hex in piece.GameHexes)
                        {
                            if (otherHex == hex)
                                return false;
                        }
                    }
                }
            }
            return currentBoard.InStartingArea(piece, currentPlayerIndex);
        }
        else if (currentPhase == GamePhase.Main)
        {
            foreach (GameHex gHex in piece.GameHexes)
            {
                Hex hex = FractionalHex.HexRound(Layout.PixelToHex(gHex.GlobalPoint));

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
            if (!IsValidPosition(piece))
                return false;
        }
        foreach (Piece piece in players[currentPlayerIndex].pieces)
        {
            if (currentBoard.InStartingArea(piece, (currentPlayerIndex + 1) % 2))
                return true;
        }

        return false;
    }
    

    internal void End()
    {
        Destroy(currentBoard.gameObject);

        foreach (Player player in players)
        {
            player.ClearPieces();
        }

        Destroy(gameObject);
    }
}
