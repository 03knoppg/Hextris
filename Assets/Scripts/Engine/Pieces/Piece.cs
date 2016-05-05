using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * The position of this game object is equal to the global position of the pivot hex.
 * Each other hex is calcualted on local layout reletive to pivot the hex.
 */
public class Piece: MonoBehaviour
{
    public List<GameHex> hexes = new List<GameHex>();
    public GameHex GameHexPrefab;

    public Mode mode;
    public enum Mode
    {
        Placement,
        Active,
        Inactive
    }

    //the position of each hex is based on the local layout where the mainHex is always at (0,0)
    Layout localLayout;


    void Awake()
    {
        localLayout = new Layout(Driver.layout.orientation, Driver.layout.size, new Point(0, 0));
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
        if (mode == Mode.Active)
        {
            if (gameHex.IsPivotHex)
                Rotate();
            else
                SetPivotHex(gameHex);
        }
    }

    private void SetPivotHex(GameHex pivotHex)
    {
        Layout newLocalLayout = new Layout(localLayout.orientation, localLayout.size, pivotHex.LocalPoint);

        foreach(GameHex gameHex in hexes)
        {
            gameHex.UpdateLayout(localLayout, newLocalLayout);
        }

        pivotHex.SetColour(Color.green);
        localLayout = newLocalLayout;
    }

    //Simply set the transform position
    public void SetPosition(Point position)
    {
        transform.position = new Vector3(position.x, 0.2f, position.y);
    }

    void Rotate()
    {
        foreach (GameHex gameHex in hexes)
        {
            gameHex.Rotate(1);
            gameHex.UpdatePosition(localLayout);
        }
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
}


