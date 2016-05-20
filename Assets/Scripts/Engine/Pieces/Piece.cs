using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * The position of this game object is equal to the global position of the pivot hex.
 * Each other hex is calcualted on local layout reletive to pivot the hex.
 */
public class Piece: MonoBehaviour
{
    public delegate void PieceClicked(Piece piece, GameHex hex);
    public PieceClicked OnPieceClicked;

    public List<GameHex> hexes = new List<GameHex>();
    public GameHex GameHexPrefab;

    public Shape shape;
    public int rotation = 0;
    public float rotationRate = 0;

    public enum Shape
    {
        //L, //not yet implemented
        //I, //not yet implemented
        Triangle,
        S,
        C
    };

    static Dictionary<Shape, int[,]> shapes = new Dictionary<Shape, int[,]>()
	{
        //Axial coodrdinates?
		{Shape.Triangle,    new int[4,2]{{0,0},{0,1},{1,0},{0,-1}}},
		{Shape.S,           new int[4,2]{{0,0},{0,-1},{0,-2},{0,-3}}},
		{Shape.C,           new int[4,2]{{0,0},{0,1},{0,2},{1,2}}}
	};

    public enum EMode
    {
        PlacementValid,
        PlacementInvalid,
        Disabled, //other player's turn
        Selected, //selected piece
        Active,   //able to be selected
        Inactive  //unable to be selected
    }

    EMode mode;
    public EMode Mode
    {
        get { return mode; }
        set
        {
            switch (value)
            {
                case EMode.PlacementValid:
                    SetColour(Color.green);
                    break;

                case EMode.PlacementInvalid:
                    SetColour(Color.red);
                    break;

                case EMode.Selected:
                    foreach (GameHex ghex in hexes)
                    {
                        //old pivot hex
                        if (ghex.IsPivotHex)
                            ghex.SetColour(Color.green);
                        else if (rotation == 0)
                            ghex.SetColour(Color.blue);
                        else
                            ghex.SetColour(new Color(0.5f, 0.5f, 1));
                    }
                    break;

                case EMode.Active:
                    SetColour(Color.blue);
                    break;

                case EMode.Inactive:
                    SetColour(new Color(0.5f, 0.5f, 1));
                    break;

                case EMode.Disabled:
                    SetColour(Color.gray);
                    break;


            }
            mode = value;
        }
    }

    //local layout origin is always 0,0
    //To move the piece simply set the transform position
    //the position of each hex is based on the local layout where the pivot hex is always at (0,0)
    public Layout localLayout;

    public Point Point
    {
        get { return new Point(transform.position.x, transform.position.z); }
        set { transform.localPosition = new Vector3(value.x, 0.2f, value.y); }
    }

    void Awake()
    {
        localLayout = new Layout(Game.layout.orientation, Game.layout.size, new Point(0, 0));
    }

    void Start()
    {
        int[,] points = shapes[shape];
        for (int i = 0; i < points.Length / 2; i++)
        {
            Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(points[i, 0], points[i, 1]));
            AddHex(hex);
        }

        foreach (GameHex gHex in hexes)
        {
            gHex.OnCollision += HexCollision;
        }
    }

    private void HexCollision()
    {
        if (mode == EMode.Selected)
        {
            rotationRate = 0;
            if (rotation > 0)
                rotation--;
            else
                rotation++;
        }
    }

    void Update()
    {
        //Debug.Log(rotation * 60 + " " + transform.rotation.eulerAngles.y + " " + Mathf.Abs(Mathf.DeltaAngle(rotation * 60, transform.rotation.eulerAngles.y)));
        //rotate gameObject and detect collisions until near destination then snap to new position
        if(Mathf.Abs(Mathf.DeltaAngle(rotation * 60, transform.rotation.eulerAngles.y)) > 0.05f)
            transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, rotation * 60, ref rotationRate, 0.3f), 0);

        else if (Mathf.Abs(rotationRate) > 0)
        {
            rotationRate = 0;
        }
    
    }

    public void LockRotation()
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.Rotate(rotation);
            gHex.UpdatePosition(localLayout);
        }

        rotationRate = 0;
        rotation = 0;
        transform.rotation = Quaternion.identity;
    }

    public void ResetRotation()
    {
        rotationRate = 0;
        rotation = 0;
        transform.rotation = Quaternion.identity;
    }

    public void AddHex(Hex hex)
    {
        GameHex newGameHex = Instantiate<GameHex>(GameHexPrefab);

        newGameHex.transform.parent = transform;
        newGameHex.OnClicked += OnHexClicked;
        newGameHex.SetPosition(localLayout, hex);

        hexes.Add(newGameHex);
        if (hexes.Count == 1)
            SetPivotHex(newGameHex);
    }

    private void OnHexClicked(GameHex gameHex)
    {
        if (rotationRate == 0 && mode != EMode.Inactive)
        {
            if (OnPieceClicked != null)
                OnPieceClicked(this, gameHex);
        }
    }

    public void SetPivotHex(GameHex pivotHex)
    {

        transform.position = pivotHex.transform.position;
        Layout newLocalLayout = new Layout(localLayout.orientation, localLayout.size, pivotHex.LocalPoint);

        foreach(GameHex gameHex in hexes)
        {
            gameHex.UpdateLayout(localLayout, newLocalLayout);
        }

        pivotHex.SetColour(Color.green);
    }

    public void RotateCCW()
    {
        if (rotationRate == 0)
            //rotation = (rotation + 5) % 6;
            rotation--;
    }
    public void RotateCW()
    {
        if (rotationRate == 0)
            //rotation = (rotation + 1) % 6;
            rotation++;
    }

    void SetColour(Color color)
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.SetColour(color);
        }
    }
}


