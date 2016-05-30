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
    public int targetRotation = 0;
    public int lastGoodRotation = 0;
    public float rotationFloat = 0;
    public float rotationRate = 0;
    public float oldRotateAngle = 0;

    public float realRotationFloat = 0;
    public bool colliding = false;

    public Material OuterInactive;
    public Material OuterPivot;
    public Material OuterSelected;
    public Material InnerActive;
    public Material InnerDisabled;


    public enum Shape
    {
        //L, //not yet implemented
        //I, //not yet implemented
        Two,
        Triangle,
        S,
        C
    };

    static Dictionary<Shape, int[,]> shapes = new Dictionary<Shape, int[,]>()
	{
        //Axial coodrdinates?
		{Shape.Triangle,    new int[2,2]{{0,0},{0,1}}},
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
                    SetColourInner(InnerActive);
                    SetColourOuter(OuterSelected);
                    break;

                case EMode.PlacementInvalid:
                    SetColourInner(InnerDisabled);
                    SetColourOuter(OuterSelected);
                    break;

                case EMode.Selected:
                    foreach (GameHex ghex in hexes)
                    {
                        //old pivot hex
                        if (ghex.IsPivotHex)
                            ghex.SetColourOuter(OuterPivot);
                        else if (targetRotation == 0)
                            ghex.SetColourOuter(OuterSelected);
                        else
                            ghex.SetColourOuter(OuterInactive);
                    }
                    break;

                case EMode.Active:
                    SetColourOuter(OuterSelected);
                    break;

                case EMode.Inactive:
                    SetColourOuter(OuterInactive);
                    break;

                case EMode.Disabled:
                    SetColourInner(InnerDisabled);
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
            gHex.OnCollisionExit += HexCollisionExit;
        }

        FixCorners();

        SetColourInner(InnerActive);
        SetColourOuter(OuterSelected);
    }


    private void FixCorners()
    {
        foreach (GameHex gHex in hexes)
        {
            foreach (MeshRenderer corner in gHex.corners)
                corner.gameObject.SetActive(false);

            for (int direction = 0; direction < 6; direction++)
            {
                Hex neighbour = Hex.Neighbor(gHex.hex, direction);
                foreach (GameHex gHex2 in hexes)
                {
                    if (gHex2.hex == neighbour)
                    {

                        gHex.corners[(direction + 5) % 6].gameObject.SetActive(true);
                        gHex.corners[direction].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private void HexCollision()
    {
        if (mode == EMode.Selected)
        {
            rotationFloat = 0;
            rotationRate = 0;
            targetRotation = lastGoodRotation;
            colliding = true;
        }
    }
    private void HexCollisionExit()
    {
        colliding = false;
    }

    void Update()
    {
        if (mode == EMode.Selected && Input.GetMouseButton(0) && !colliding)
        {
            Vector3 mouseOffset = Input.mousePosition - UnityEngine.Camera.main.WorldToScreenPoint(transform.position);
            float pointerDistance = mouseOffset.magnitude;
            if (pointerDistance > 50)
            {
                float mouseAngle = Mathf.Atan2(mouseOffset.x, mouseOffset.y);
                float rotationDelta = mouseAngle - oldRotateAngle;
                if (oldRotateAngle != 100 && Mathf.Abs(rotationDelta) < 1)
                {
                    //Debug.Log(rotationDelta);
                    rotationFloat += rotationDelta;
                    if (Mathf.Abs(rotationFloat) > Mathf.PI / 3)
                    {
                        if (rotationFloat > 0)
                            targetRotation++;
                        else if (rotationFloat < 0)
                            targetRotation--;

                        rotationFloat = 0;
                    }
                }
                oldRotateAngle = mouseAngle;
            }
            else
                oldRotateAngle = 100;
            //Debug.Log(mouseOffset);
        }
       
        
        //rotate gameObject and detect collisions until near destination then snap to new position


        if (Mathf.Abs(Mathf.DeltaAngle(targetRotation * 60, transform.rotation.eulerAngles.y)) > 0.05f)
            realRotationFloat = Mathf.SmoothDampAngle(realRotationFloat, targetRotation * 60, ref rotationRate, 0.3f);

        else if (Mathf.Abs(rotationRate) > 0)
        {
            transform.rotation = Quaternion.Euler(0, targetRotation * 60, 0);
            rotationRate = 0;
        }

        if (realRotationFloat - lastGoodRotation * 60 > 60)
            lastGoodRotation ++;
        else if(realRotationFloat - lastGoodRotation * 60 < -60)
            lastGoodRotation--;
           

        transform.rotation = Quaternion.Euler(0, realRotationFloat, 0);
    }

    public void LockRotation()
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.Rotate(targetRotation);
            gHex.UpdatePosition(localLayout);
        }
        FixCorners();

        rotationRate = 0;
        rotationFloat = 0;
        targetRotation = 0;
        transform.rotation = Quaternion.identity;
    }

    public void ResetRotation()
    {
        rotationRate = 0;
        targetRotation = 0;
        transform.rotation = Quaternion.identity;
    }

    public void AddHex(Hex hex)
    {
        GameHex newGameHex = Instantiate<GameHex>(GameHexPrefab);

        newGameHex.transform.parent = transform;
        newGameHex.OnHexClicked += OnHexClicked;
        newGameHex.OnHexMouseDown += OnHexMouseDown;
        newGameHex.SetPosition(localLayout, hex);

        hexes.Add(newGameHex);
        if (hexes.Count == 1)
            SetPivotHex(newGameHex);
    }

    private void OnHexMouseDown(GameHex gameHex)
    {
        if (rotationRate == 0 && mode == EMode.Active)
        {
            if (OnPieceClicked != null)
                OnPieceClicked(this, gameHex);
        }
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
            pivotHex.SetColourOuter(OuterSelected);
        }

        pivotHex.SetColourOuter(OuterPivot);
    }

    public void RotateCCW()
    {
        if (rotationRate == 0)
            //rotation = (rotation + 5) % 6;
            targetRotation--;
    }
    public void RotateCW()
    {
        if (rotationRate == 0)
            //rotation = (rotation + 1) % 6;
            targetRotation++;
    }

    void SetColourInner(Material mat)
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.SetColourInner(mat);
        }
    }
    void SetColourOuter(Material mat)
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.SetColourOuter(mat);
        }
    }
}


