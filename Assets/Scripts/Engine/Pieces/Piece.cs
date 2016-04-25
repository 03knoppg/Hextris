using System;
using System.Collections.Generic;
using UnityEngine;


public class Piece: MonoBehaviour
{
    public List<GameHex> hexes = new List<GameHex>();
    public GameObject GameHexPrefab;

    //the position of each hex is based on the local layout where the mainHex is always at (0,0)
    Layout localLayout;

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
        GameObject newGameObject = GameObject.Instantiate<GameObject>(GameHexPrefab);
        
        newGameObject.transform.parent = transform;
        newGameObject.name += UnityEngine.Random.value;

        GameHex newGameHex = newGameObject.GetComponent<GameHex>();
        newGameHex.hex = hex;
        hexes.Add(newGameHex);
        if (hexes.Count == 1)
            SetPivotHex(newGameHex);

        Point position = Layout.HexToPixel(localLayout, hex);
        newGameObject.transform.position = new Vector3(position.x, 0.2f, position.y);

    }

    private void OnHexClicked(GameHex gameHex)
    {
        turning = true;
        if (Hex.Length(gameHex.hex) > 0)
            SetPivotHex(gameHex);
        else
            Rotate();
    }

    private void SetPivotHex(GameHex gameHex)
    {

        Layout newLocalLayout = new Layout(localLayout.orientation, localLayout.size, Layout.HexToPixel(localLayout, gameHex.hex));

        foreach(GameHex ghex in hexes)
        {
            //old pivot hex
            if(Hex.Length(ghex.hex) == 0)
                ghex.SetColour(Color.blue);

            //translate into new layout based on new main hex
            ghex.hex = FractionalHex.HexRound(Layout.PixelToHex(newLocalLayout, Layout.HexToPixel(localLayout, ghex.hex)));
            Point position = Layout.HexToPixel(newLocalLayout, ghex.hex);
            ghex.transform.position = new Vector3(position.x, 0.2f, position.y);
        }

        gameHex.SetColour(Color.green);
        localLayout = newLocalLayout;
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


}


