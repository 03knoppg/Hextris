using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * The position of this game object is equal to the global position of the pivot hex.
 * Each other hex is calculated on local layout relative to pivot the hex.
 */
[ExecuteInEditMode]
public class Piece: MonoBehaviour
{
	public delegate void PieceClicked(Piece piece, GameHex hex);
	public PieceClicked OnPieceClicked;

	//public List<GameHex> hexes = new List<GameHex>();

    [HexBuilder]
    [SerializeField]
    HexListWrapper HexListWrapper;

    public List<GameHex> Hexes { get { return HexListWrapper.GameHexes; } }

    public float mouseOffset;
	
	public int targetRotation = 0;
	public int lastGoodRotation = 0;
	public float rotationFloat = 0;
	public float rotationRate = 0;
	public float oldRotateAngle = 0;

	public float realRotationFloat = 0;
	public bool colliding = false;

    [HideInInspector]
    public Material OuterInactive;
    [HideInInspector]
    public Material OuterPivot;
    [HideInInspector]
    public Material OuterSelected;
    [HideInInspector]
    public Material InnerActive;
    [HideInInspector]
	public Material InnerDisabled;


//    public enum Shape
//    {
//        //L, //not yet implemented
//        //I, //not yet implemented
//        Two,
//        Triangle,
//        S,
//        C,
//Three,
//Four
//    };

//    static Dictionary<Shape, int[,]> shapes = new Dictionary<Shape, int[,]>()
//    {
//        //Axial coordinates?
//        {Shape.Two,         new int[2,2]{{0,0},{0,1}}},
//        {Shape.Three,       new int[3,2]{{0,0},{0,1},{-1,2}}},
//        {Shape.Four,        new int[4,2]{{0,0},{0,1},{-1,2},{-1,3}}},
//        {Shape.Triangle,    new int[4,2]{{0,0},{0,1},{1,0},{0,-1}}},
//        {Shape.S,           new int[4,2]{{0,0},{0,-1},{0,-2},{0,-3}}},
//        {Shape.C,           new int[4,2]{{0,0},{0,1},{0,2},{1,2}}}
//    };

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
                    foreach (GameHex ghex in Hexes)
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
    public Layout globalLayout;

	public Point Point
	{
		get { return new Point(transform.position.x, transform.position.z); }
		set { transform.localPosition = new Vector3(value.x, 0.2f, value.y); }
	}

    public void Awake()
    {
        LoadAsset();
    }

    public void LoadAsset()
    {
        string assetPath = "Assets/Prefabs/Pieces/AssetDB/" + name.Replace("(Clone)", "");
        HexListWrapper = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "HexListWrappers.asset");

        if (HexListWrapper == null)
        {
            Debug.LogError("HexListWrapper asset null");
        }
    }

	public void Init(Layout layout, int startRotation)
	{
        globalLayout = new Layout(layout.orientation, layout.size, new Point(0, 0)); 
        localLayout = new Layout(layout.orientation, layout.size, new Point(0, 0));


        HexListWrapper.GameHexes = new List<GameHex>();
        foreach (Hex hex in HexListWrapper.Hexes)
        {
            GameHex newGameHex = ObjectFactory.GameHex(globalLayout);

            newGameHex.transform.parent = transform;
            newGameHex.OnHexClicked += OnHexClicked;
            newGameHex.OnHexMouseDown += OnHexMouseDown;
            newGameHex.SetPosition(localLayout, hex);

            Hexes.Add(newGameHex);
            //if (Hexes.Count == 1)
            //    SetPivotHex(newGameHex);
        }
        foreach (GameHex gHex in Hexes)
		{
			gHex.OnCollision += HexCollision;
			gHex.OnCollisionExit += HexCollisionExit;
		}

		FixCorners();

		SetColourInner(InnerActive);
		SetColourOuter(OuterSelected);

        targetRotation = startRotation;
        LockRotation();
	}


	private void FixCorners()
	{
        foreach (GameHex gHex in Hexes)
		{
			foreach (MeshRenderer corner in gHex.corners)
				corner.gameObject.SetActive(false);

			for (int direction = 0; direction < 6; direction++)
			{
				Hex neighbour = Hex.Neighbor(gHex.hex, direction);
				foreach (GameHex gHex2 in Hexes)
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
		if (mode == EMode.Selected && !colliding)
		{
            if (Input.GetMouseButtonDown(0))
                mouseOffset = Input.mousePosition.x;

            else if (Input.GetMouseButton(0))
            {
                rotationFloat += (Input.mousePosition.x - mouseOffset) / 100;
                if (Mathf.Abs(rotationFloat) > Mathf.PI / 3)
                {
                    if (rotationFloat > 0)
                        targetRotation++;
                    else if (rotationFloat < 0)
                        targetRotation--;

                    rotationFloat = 0;
                }
                mouseOffset = Input.mousePosition.x;
            }
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
        foreach (GameHex gHex in Hexes)
		{
			gHex.Rotate(targetRotation);
			gHex.UpdatePosition(localLayout);
		}
		FixCorners();
        ResetRotation();
	}

	public void ResetRotation()
	{
        targetRotation = 0;
        lastGoodRotation = 0;
        rotationFloat = 0;
        rotationRate = 0;
        oldRotateAngle = 0;

        realRotationFloat = 0;
		transform.rotation = Quaternion.identity;
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

        foreach (GameHex gameHex in Hexes)
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
        foreach (GameHex gHex in Hexes)
		{
			gHex.SetColourInner(mat);
		}
	}
	void SetColourOuter(Material mat)
	{
        foreach (GameHex gHex in Hexes)
		{
			gHex.SetColourOuter(mat);
		}
	}
}


