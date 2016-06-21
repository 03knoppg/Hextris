using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * The position of this game object is equal to the global position of the pivot hex.
 * Each other hex is calculated on local layout relative to pivot the hex.
 */

public class Piece: MonoBehaviour
{

    public class MovementFinished : UnityEvent { };
    public MovementFinished OnMovementFinished;
    public class PieceClicked : UnityEvent<Piece, GameHex> { };
    public PieceClicked OnPieceClicked;


    public List<GameHex> GameHexes;

    public float mouseOffset;

    public int previousRotation = 0;
	
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
    [HideInInspector]
    public Material InnerPivot;



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
                    foreach (GameHex ghex in GameHexes)
					{
						//old pivot hex
                        if (ghex.IsPivotHex)
                        {
                            if (targetRotation % 6 == 0)
                                ghex.SetColourOuter(OuterSelected);
                            else
                                ghex.SetColourOuter(OuterInactive);
                            ghex.SetColourInner(InnerPivot);
                        }
                        else if (targetRotation % 6 == 0)
                        {
                            ghex.SetColourOuter(OuterSelected);
                            ghex.SetColourInner(InnerActive);
                        }
                        else
                        {
                            ghex.SetColourOuter(OuterInactive);
                            ghex.SetColourInner(InnerDisabled);

                        }
					}
					break;

                case EMode.Active:
                    SetColourInner(InnerActive);
					SetColourOuter(OuterSelected);
					break;

                case EMode.Inactive:
                    SetColourInner(InnerActive);
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
		//get { return new Point(transform.position.x, transform.position.z); }
		set {
            transform.localPosition = new Vector3(value.x, 0, value.y);
        }
	}



	public void Init(Layout layout, int startRotation, OffsetCoord? startPosition = null)
	{

        OnMovementFinished = new MovementFinished();
        OnPieceClicked = new PieceClicked();

        globalLayout = new Layout(layout.orientation, layout.size, new Point(0, 0)); 
        localLayout = new Layout(layout.orientation, layout.size, new Point(0, 0));

        GameHexes = new List<GameHex>(GetComponentsInChildren<GameHex>());
        foreach (GameHex gHex in GameHexes)
        {
            gHex.OnHexMouseDown += OnHexMouseDown;
			gHex.OnCollision += HexCollision;
			gHex.OnCollisionExit += HexCollisionExit;
            gHex.layer = 1;
		}

		FixCorners();

		SetColourInner(InnerActive);
		SetColourOuter(OuterSelected);

        targetRotation = startRotation;
        LockRotation();

        Point = Layout.HexToPixel(globalLayout, OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, startPosition.GetValueOrDefault()));
	}


	private void FixCorners()
	{
        foreach (GameHex gHex in GameHexes)
		{
			foreach (MeshRenderer corner in gHex.corners)
				corner.gameObject.SetActive(false);

			for (int direction = 0; direction < 6; direction++)
			{
				Hex neighbour = Hex.Neighbor(gHex.hex, direction);
				foreach (GameHex gHex2 in GameHexes)
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
                float mouseDelta = (Input.mousePosition.x - mouseOffset) * 20 / Screen.width;
                if (Mathf.Abs(mouseDelta) > 0.0005f && Mathf.Abs(rotationRate) < 100)
                {
                    if (Mathf.Sign(mouseDelta) != Mathf.Sign(rotationFloat))
                        rotationFloat = 0;

                    rotationFloat += mouseDelta;
                }
                if (Mathf.Abs(rotationFloat) > 1)
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


        if (Mathf.Abs(Mathf.DeltaAngle(targetRotation * 60, realRotationFloat)) > 0.05f)
			realRotationFloat = Mathf.SmoothDampAngle(realRotationFloat, targetRotation * 60, ref rotationRate, 0.3f);

		else if (Mathf.Abs(rotationRate) > 0 && !Input.GetMouseButton(0))
		{
			transform.rotation = Quaternion.Euler(0, targetRotation * 60, 0);
			rotationRate = 0;
            rotationFloat = 0;
            OnMovementFinished.Invoke();
		}

		if (realRotationFloat - lastGoodRotation * 60 > 60)
			lastGoodRotation ++;
		else if(realRotationFloat - lastGoodRotation * 60 < -60)
			lastGoodRotation--;
		   

		transform.rotation = Quaternion.Euler(0, realRotationFloat, 0);
	}

	public void LockRotation()
	{
        previousRotation = targetRotation;
        foreach (GameHex gHex in GameHexes)
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

    public void UndoRotation()
    {
        targetRotation = -previousRotation;
        LockRotation();
        previousRotation = 0;
    }

	private void OnHexMouseDown(GameHex gameHex)
	{
        if (mode != EMode.Inactive)
		{
			OnPieceClicked.Invoke(this, gameHex);
		}
	}
    
	public void SetPivotHex(GameHex pivotHex)
	{
        //this could enable an exploit 
        LockRotation();

		transform.position = new Vector3(pivotHex.transform.position.x, 0, pivotHex.transform.position.z);
		Layout newLocalLayout = new Layout(localLayout.orientation, localLayout.size, pivotHex.LocalPoint);

        foreach (GameHex gameHex in GameHexes)
		{
			gameHex.UpdateLayout(localLayout, newLocalLayout);
			pivotHex.SetColourOuter(OuterSelected);
		}

		pivotHex.SetColourOuter(OuterPivot);
	}

	public void RotateCCW()
	{
		if (rotationRate == 0)
			targetRotation--;
	}
	public void RotateCW()
	{
		if (rotationRate == 0)
			targetRotation++;
	}

	void SetColourInner(Material mat)
	{
        foreach (GameHex gHex in GameHexes)
		{
			gHex.SetColourInner(mat);
		}
	}
	void SetColourOuter(Material mat)
	{
        foreach (GameHex gHex in GameHexes)
		{
			gHex.SetColourOuter(mat);
		}
	}

}


