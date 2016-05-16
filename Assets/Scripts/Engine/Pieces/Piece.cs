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
    int rotation = 0;
    float rotationRate = 0;


    public Mode mode;
    public enum Mode
    {
        Placement,
        Selected,
        Active,
        Inactive
    }

    //the position of each hex is based on the local layout where the mainHex is always at (0,0)
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
        if (transform.rotation.eulerAngles.y > rotation * 60)
            rotation--; 
        else
            rotation++;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, rotation * 60, ref rotationRate, 0.5f), 0);
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
        if (mode != Mode.Inactive)
        {
            if (OnPieceClicked != null)
                OnPieceClicked(this, gameHex);
        }
    }

    public void SetPivotHex(GameHex pivotHex)
    {

        Layout newLocalLayout = new Layout(localLayout.orientation, localLayout.size, pivotHex.LocalPoint);

        foreach(GameHex gameHex in hexes)
        {
            gameHex.UpdateLayout(localLayout, newLocalLayout);
        }

        pivotHex.SetColour(Color.green);
        localLayout = newLocalLayout;
    }

    public void Rotate()
    {
        rotation = (rotation + 1) % 6;
    }



    public void SetActive(bool active)
    {
        if (active)
        {
            mode = Mode.Active;
            foreach (GameHex ghex in hexes)
            {
                //old pivot hex
                if (ghex.IsPivotHex)
                    ghex.SetColour(Color.green);
                else
                    ghex.SetColour(Color.blue);
            }
        }
        else
        {
            mode = Mode.Inactive;
            foreach (GameHex ghex in hexes)
            {
                ghex.SetColour(Color.grey);
            }
        }
    }



    internal void SetColor(Color color)
    {
        foreach (GameHex gHex in hexes)
        {
            gHex.SetColour(color);
        }
    }


}


