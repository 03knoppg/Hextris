using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Game : MonoBehaviour {

    public float order;

    public enum GameType
    {
        Classic,
        Puzzle
    }

    [Serializable]
    public struct PieceData
    {
        public Piece piecePrefab;
        public int startRotation;
        public bool useStartPosition;
        public OffsetCoord startPosition;
        public bool lockPivotHex;
        public OffsetCoord lockedPivotHex;
        public bool lockSelected;
    }
    [SerializeField]
    List<PieceData> PieceDatas;

    public List<Player> players;
    public Player CurrentPlayer { get { return players[currentPlayerIndex]; } }
    int currentPlayerIndex = 0;

    IEnumerable<Piece> AllPieces { get { return players.SelectMany(p => p.pieces); } }

    Board currentBoard;

    [HideInInspector]
    public Piece lastSelectedPiece;
    [HideInInspector]
    public Piece currentSelectedPiece;

    [SerializeField]
    Board BoardPrefab;
    
    public float layoutSize = 1;
    public int numPlayers = 1;

    public void Init()
    {

        Layout.defaultLayout = new Layout(Layout.pointy, new Point(layoutSize, layoutSize), new Point(0, 0));

        players = new List<Player>();
        for (int i = 0; i < numPlayers; i++)
        {
            Player p = new Player();
            players.Add(p);
            p.Name = "Player" + i;
        }

        currentBoard = ObjectFactory.Board(BoardPrefab);
    }

    public void StartSetup()
    {
        SelectPiece(null);
        MakeNextPlacementPiece();
    }

    public void StartMain()
    {
        foreach(Piece piece in AllPieces)
        {
            piece.OnPieceClicked.RemoveListener(OnPieceClickedSetup);
            piece.OnPieceClicked.AddListener(OnPieceClickedMain);
        }
    }

    Bounds GetBoardBounds()
    {
        Bounds bounds = new Bounds();
        foreach (GameHex gHex in currentBoard.Hexes)
        {
            bounds.Encapsulate(new Bounds(gHex.transform.position, Vector3.one * (layoutSize + 1)));
        }

        return bounds;
    }

    void PiecePlaced(Piece piece)
    {
        piece.Mode = Piece.EMode.Inactive;
        MakeNextPlacementPiece();
    }

    public void UpdateUIStateMain()
    {
        bool anyTurning = false;
        bool allLegal = true;
        foreach (Piece piece in CurrentPlayer.pieces)
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
            UIStates.SetGroupState(UIStates.Group.Undo, UIStates.State.Active);
        else
            UIStates.SetGroupState(UIStates.Group.Undo, UIStates.State.Disabled);

        //if (allLegal && !anyTurning && hasTurned)
        //    UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Active);
        //else
        //    UIState.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Disabled);
    }

    void SetPieceModeSetup(Piece piece)
    {
        Plane boardPlane = new Plane(Vector3.up, currentBoard.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        bool hit = boardPlane.Raycast(ray, out rayDistance);

        if (hit)
        {
            Vector3 point = ray.GetPoint(rayDistance);
            FractionalHex fHex = Layout.PixelToHex(new Point(point.x, point.z));
            Point p = Layout.HexToPixel(FractionalHex.HexRound(fHex));

            piece.Point = p;

            piece.Mode =
                IsValidSetupPosition(piece) ?
                Piece.EMode.PlacementValid:
                Piece.EMode.PlacementInvalid;

        }
    }

    public void SetPieceModeEnd()
    {
        foreach (Piece piece in AllPieces)
        {
            piece.Mode = Piece.EMode.Active;
        }
    }

    public void UpdateCollisions()
    {
        if (currentSelectedPiece == null)
            return;
        
        //check pieces colliding with each other
        foreach (Piece piece in AllPieces)
        {
            if(piece != currentSelectedPiece && currentSelectedPiece.IsColliding(piece))
            {
                if (!currentSelectedPiece.colliding)
                    currentSelectedPiece.HexCollision();
                return;
            }
        }

        //check pieces colliding with board
        foreach (GameHex hex in currentSelectedPiece.GameHexes)
        {
            foreach (GameHex boardHex in currentBoard.Hexes)
            {
                if (hex.Bounds.Overlapping(boardHex.Bounds))
                {
                    if(!currentSelectedPiece.colliding)
                        currentSelectedPiece.HexCollision();
                    return;
                }
            }
        }

        currentSelectedPiece.HexCollisionExit();
    }



    void MakeNextPlacementPiece()
    {
        int numPieces = AllPieces.Count();
        

        //have all the pieces been placed?
        if (Mathf.FloorToInt(numPieces / numPlayers) == PieceDatas.Count)
        {
            //start with player 0
            currentPlayerIndex = players.Count-1;
            Signals.Invoke(ESignalType.SetupComplete);
            NextPlayer();
            return;
        }

        //for two players go 01100110 etc.
        currentPlayerIndex = Mathf.Clamp(Mathf.FloorToInt(numPieces / numPlayers) % 2 == 0 ?
            numPieces % numPlayers :
            (numPlayers - 1) - numPieces % numPlayers, 0, (numPlayers - 1));

        int index = Mathf.FloorToInt(numPieces / 2);


        PieceData PieceData = PieceDatas[index];
        Piece piece = ObjectFactory.Piece(PieceData);

        piece.name = PieceData.piecePrefab.name + " " + CurrentPlayer.Name;

        CurrentPlayer.pieces.Add(piece);

        if (piece.lockPivotHex)
            piece.SetPivotHex(PieceData.lockedPivotHex, true);
        

        piece.OnPieceClicked.AddListener(OnPieceClickedSetup);
        piece.OnMovementFinished.AddListener(OnMovementFinished);
        piece.OuterInactive = PieceColourPallet.OuterInactive(currentPlayerIndex);
        piece.OuterPivot = PieceColourPallet.OuterPivot(currentPlayerIndex);
        piece.OuterSelected = PieceColourPallet.OuterSelected(currentPlayerIndex);
        piece.InnerPivot = PieceColourPallet.InnerPivot(currentPlayerIndex);
        piece.InnerActive = PieceColourPallet.InnerActive(currentPlayerIndex);
        piece.InnerDisabled = PieceColourPallet.InnerDisabled(currentPlayerIndex);

        if (PieceData.useStartPosition)
            PiecePlaced(piece);
        else
        {
            SelectPiece(piece);
            currentBoard.HighlightPlayer(currentPlayerIndex);

            Signals.Invoke(ESignalType.PlayerTurn, currentPlayerIndex);
        }
        if(currentSelectedPiece)
            SetPieceModeSetup(currentSelectedPiece);
    }

    public void OnMovementFinished()
    {
        if (IsPlayerWin())
        {
            Signals.Invoke(ESignalType.PlayerWin, currentPlayerIndex);

            if (players.Count == 1)
                Signals.Invoke(ESignalType.PuzzleComplete, 3);
            return;
        }
        else
        {
            bool anyTurning = false;
            bool allLegal = true;
            foreach (Piece piece in CurrentPlayer.pieces)
            {
                anyTurning |= piece.IsRotating();
                allLegal &= IsValidPosition(piece);
            }

            bool hasTurned = currentSelectedPiece != null && currentSelectedPiece.IsRotated();


            if (allLegal && !anyTurning && hasTurned)
            {
                NextPlayer();
            }
        }
    }

    void OnPieceClickedSetup(Piece piece, GameHex hex)
    {
        if (piece == currentSelectedPiece && IsValidPosition(piece))
            PiecePlaced(piece);
    }

    void OnPieceClickedMain(Piece piece, GameHex hex)
    {
        if (!hex.IsPivotHex)
        {
            if (!piece.IsRotated())
                piece.SetPivotHex(hex);
        }

        if (piece != currentSelectedPiece && piece.Mode == Piece.EMode.Active)
        {
            lastSelectedPiece = null;
            SelectPiece(piece);
        }
    }

    void SelectPiece(Piece piece)
    {
        if (currentSelectedPiece != null)
        {
            if (currentSelectedPiece.lockSelected)
                return;

            currentSelectedPiece.ResetRotation();
            lastSelectedPiece = currentSelectedPiece;
            lastSelectedPiece.Mode = Piece.EMode.Inactive;
        }
        currentSelectedPiece = piece;
        if(piece) piece.Mode = Piece.EMode.Selected;
        Signals.Invoke(ESignalType.PieceSelected, piece);
    }

    void NextPlayer()
    {
        CurrentPlayer.SetActivePlayer(false);

        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;

        CurrentPlayer.SetActivePlayer(true);

        currentBoard.HighlightPlayer(currentPlayerIndex + 1);
        Signals.Invoke(ESignalType.PlayerTurn, currentPlayerIndex);

        if (currentSelectedPiece != null)
        {
            currentSelectedPiece.LockRotation();
            if(numPlayers == 1)
                lastSelectedPiece = currentSelectedPiece;
            currentSelectedPiece = null;
        }
    }

    bool IsValidSetupPosition(Piece piece)
    {
        foreach (Piece otherPiece in AllPieces)
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
        
        return currentBoard.InStartingArea(piece, currentPlayerIndex);
    }

    public bool IsValidPosition(Piece piece)
    {
        foreach (GameHex gHex in piece.GameHexes)
        {
            Hex hex = FractionalHex.HexRound(Layout.PixelToHex(gHex.GlobalPoint));

            if (!currentBoard.InBounds(hex))
                return false;
                
        }
        return true;
    }

    public bool IsPlayerWin()
    {
        if (CurrentPlayer.pieces.Any(piece => !IsValidPosition(piece)))
            return false;

        if (CurrentPlayer.pieces.Any(piece => currentBoard.InStartingArea(piece, (currentPlayerIndex + 1) % 2)))
            return true;


        return false;
    }
    

    public void End()
    {
        Destroy(currentBoard.gameObject);

        foreach (Player player in players)
        {
            player.ClearPieces();
        }

        Destroy(gameObject);
    }

    public static Game currentGame;
    public static int currentGameIndex;
    public static void SelectGame(int? index = null)
    {
        currentGame?.End();

        currentGameIndex = index ?? ++currentGameIndex;

        currentGame = ObjectFactory.Game(Progression.Puzzles.Obj[currentGameIndex].prefab);

        Signals.Invoke(ESignalType.CamPosition, currentGame.GetBoardBounds());
        Signals.Invoke(ESignalType.GameStart, currentGameIndex);
    }
}
