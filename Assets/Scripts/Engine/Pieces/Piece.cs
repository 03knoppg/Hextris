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
    public int rotation = 0;
    public float rotationRate = 0;


    EMode mode;
    public EMode Mode
    {
        get { return mode; }
        set
        {
            switch (value)
            {
                case EMode.Selected:
                    foreach (GameHex ghex in hexes)
                    {
                        //old pivot hex
                        if (ghex.IsPivotHex)
                            ghex.SetColour(Color.green);
                        else
                            ghex.SetColour(Color.blue);
                    }
                    break;

                case EMode.Active:
                    foreach (GameHex ghex in hexes)
                        ghex.SetColour(Color.blue);
                    break;

                case EMode.Inactive:
                    foreach (GameHex ghex in hexes)
                        ghex.SetColour(Color.grey);
                    break;


            }
            mode = value;
        }
    }
    public enum EMode
    {
        Placement,
        Selected,
        Active,
        Inactive
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
        localLayout = new Layout(Driver.layout.orientation, Driver.layout.size, new Point(0, 0));
    }

    void Start()
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.OnCollision += HexCollision;
        }
    }

    private void HexCollision()
    {
        if (mode == EMode.Selected)
        {
            if (rotationRate > 0)
                rotation--;
            else
                rotation++;
        }
    }

    void Update()
    {
        //Debug.Log(rotation * 60 + " " + transform.rotation.eulerAngles.y + " " + Mathf.Abs(Mathf.DeltaAngle(rotation * 60, transform.rotation.eulerAngles.y)));
        //rotate gameObject and detect collisions until near destination then snap to new position
        if(Mathf.Abs(Mathf.DeltaAngle(rotation * 60, transform.rotation.eulerAngles.y)) > 0.01f)
            transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, rotation * 60, ref rotationRate, 0.5f), 0);

        else if (Mathf.Abs(rotationRate) > 0)
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
        if (mode != EMode.Inactive)
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

    public void Rotate()
    {
        if(rotationRate == 0)
            rotation = -1;
    }

    internal void SetColor(Color color)
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.SetColour(color);
        }
    }




}


