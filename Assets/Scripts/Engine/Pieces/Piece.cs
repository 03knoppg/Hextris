using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * The position of this game object is equal to the global position of the pivot hex.
 * Each other hex is calcualted on local layout reletive to pivot the hex.
 */
public class Piece: MonoBehaviour
{
    //public delegate void Placed();
    //public Placed OnPlaced;

    public List<GameHex> hexes = new List<GameHex>();
    public GameObject GameHexPrefab;



    public enum Mode
    {
        Placement,
        Active,
        Inactive
    }

    //the position of each hex is based on the local layout where the mainHex is always at (0,0)
    Layout localLayout;

    public Mode mode;
    bool turning = false;

    public Piece()
    {
        localLayout = new Layout(Driver.layout.orientation, Driver.layout.size, new Point(0, 0));
    }

    void Start()
    {
        GameHex.OnClicked += OnHexClicked;

    }

    public void AddHex(Hex hex)
    {
        //GameObject GameHexPrefab = (GameObject)Resources.Load("3DAssets/HexObjectBlue");
        GameObject newHexObject = GameObject.Instantiate<GameObject>(GameHexPrefab);
        
        newHexObject.transform.parent = transform;
        newHexObject.name += UnityEngine.Random.value;

        GameHex newGameHex = newHexObject.GetComponent<GameHex>();
        newGameHex.hex = hex;
        hexes.Add(newGameHex);
        if (hexes.Count == 1)
            SetPivotHex(newGameHex);

        Point position = Layout.HexToPixel(localLayout, hex);
        newHexObject.transform.position = new Vector3(position.x, 0.2f, position.y);

    }



    private void OnHexClicked(GameHex gameHex)
    {
        if (mode == Mode.Active)
        {
            turning = true;
            if (Hex.Length(gameHex.hex) > 0)
                SetPivotHex(gameHex);
            else
                Rotate();
        }
    }

    private void SetPivotHex(GameHex gameHex)
    {

        Layout newLocalLayout = new Layout(localLayout.orientation, localLayout.size, Layout.HexToPixel(localLayout, gameHex.hex));

        foreach(GameHex ghex in hexes)
        {
            //old pivot hex
            if(Hex.Length(ghex.hex) == 0)
                ghex.SetColour(Color.blue);

            //translate into new layout based on new pivot hex
            ghex.hex = FractionalHex.HexRound(Layout.PixelToHex(newLocalLayout, Layout.HexToPixel(localLayout, ghex.hex)));
            Point point = Layout.HexToPixel(newLocalLayout, ghex.hex);
            ghex.transform.position = new Vector3(point.x, 0.2f, point.y);
        }

        gameHex.SetColour(Color.green);
        localLayout = newLocalLayout;
    }

    //SImply set the transform position
    public void SetPosition(Point position)
    {
        transform.position = new Vector3(position.x, 0.2f, position.y);
    }

    void Rotate()
    {
        foreach (GameHex gameHex in hexes)
        {
            gameHex.Rotate(1);
            Point position = Layout.HexToPixel(localLayout, gameHex.hex);
            gameHex.transform.position = new Vector3(position.x, 0.2f, position.y);
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
                if (Hex.Length(ghex.hex) == 0)
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


