using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Game : MonoBehaviour {

    public delegate void GamePhaseChange(GamePhase newPhase);
    public GamePhaseChange OnGamePhaseChange;
    
    Layout layout;

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

    List<Player> players;
    int currentPlayerIndex = 0;

    Board currentBoard;
    GamePhase currentPhase;
    Piece currentSelectedPiece;

    [SerializeField]
    GameType type;

    [SerializeField]
    Board BoardPrefab;

    UISignals UISignals;
    UIStates UIState;


    public float layoutSize = 1;
    public int numPlayers = 1;
    public List<Piece> piecePrefabs;
    public List<int> startingRotations;



    public Material OuterInactive;
    public Material OuterPivot;
    public Material OuterSelected;
    public Material P1InnerActive;
    public Material P1InnerDisabled;
    public Material P2InnerActive;
    public Material P2InnerDisabled;


    void Start()
    {
        layout = new Layout(Layout.pointy, new Point(layoutSize, layoutSize), new Point(0, 0));

        UISignals = FindObjectOfType<UISignals>();
        UIState = FindObjectOfType<UIStates>();

        UISignals.AddListeners(OnUISignal, new List<UISignal>() { 
            UISignal.RotateCCW, 
            UISignal.RotateUndo, 
            UISignal.RotateCW, 
            UISignal.EndTurn });

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

        currentBoard = ObjectFactory.Board(BoardPrefab, layout);

        SetPhase(GamePhase.Setup);
        MakeNextPlacementPiece();

    }

    public void OnUISignal(UISignal signal, object arg1)
    {
        switch (signal)
        {
            case UISignal.EndTurn:
                if (IsPlayerWin())
                {
                    SetPhase(GamePhase.End);
                    return;
                }
                else
                    NextPlayer();
                break;
            case UISignal.RotateCCW:
                currentSelectedPiece.RotateCCW();
                break;
            case UISignal.RotateCW:
                currentSelectedPiece.RotateCW();
                break;
            case UISignal.RotateUndo:
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
                UISignals.Click(global::UISignal.PlayerWin, currentPlayerIndex);
                UIState.SetGroupState(UIStates.Group.EndGame, UIStates.State.Active);
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
                anyTurning |= piece.rotationRate != 0;
                allLegal &= IsValidPosition(piece);
            }

            bool hasTurned = currentSelectedPiece != null && currentSelectedPiece.targetRotation != 0;

            //if (anyTurning || currentSelectedPiece == null)
            //    UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Disabled);
            //else
            //    UIState.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Active);


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
                FractionalHex fHex = Layout.PixelToHex(layout, new Point(point.x, point.z));
                Point p = Layout.HexToPixel(layout, FractionalHex.HexRound(fHex));

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

                        else if (currentSelectedPiece == null || currentSelectedPiece.targetRotation == 0)
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
        if (Mathf.FloorToInt(totalPieces / numPlayers) == piecePrefabs.Count)
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

        Piece piece = ObjectFactory.Piece(
            piecePrefabs[Mathf.FloorToInt(totalPieces / 2)], 
            layout,
            players[currentPlayerIndex],
            startingRotations[Mathf.FloorToInt(totalPieces / 2)]);
        
        piece.OnPieceClicked += OnPieceClicked;
        piece.OuterInactive = OuterInactive;
        piece.OuterPivot = OuterPivot;
        piece.OuterSelected = OuterSelected;
        piece.InnerActive = currentPlayerIndex == 0 ? P1InnerActive : P2InnerActive;
        piece.InnerDisabled = currentPlayerIndex == 0 ? P1InnerDisabled : P2InnerDisabled;


        currentSelectedPiece = piece;
        currentBoard.HighlightPlayer(currentPlayerIndex);

        UISignals.Click(UISignal.PlayerTurn, currentPlayerIndex);
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
                if(piece.targetRotation == 0)
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
        
        currentBoard.HighlightPlayer(currentPlayerIndex + 1);
        UISignals.Click(UISignal.PlayerTurn, currentPlayerIndex);

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
                Hex hex = FractionalHex.HexRound(Layout.PixelToHex(layout,  gHex.GlobalPoint));

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
