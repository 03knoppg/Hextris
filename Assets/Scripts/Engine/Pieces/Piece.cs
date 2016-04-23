using System;
using System.Collections.Generic;
using UnityEngine;


public class Piece: MonoBehaviour
{
    public List<GameHex> hexes = new List<GameHex>();
    public Point position;
    public Layout layout;
    public GameObject GameHexPrefab;

    Layout tempLayout;

    bool turning = false;

    void Start()
    {

        GameHex.OnClicked += OnHexClicked;
    }

    public void AddHex(Hex hex)
    {
        GameObject GameHexPrefab = (GameObject)Resources.Load("3DAssets/HexObjectBlue");
        GameObject newGameObject = GameObject.Instantiate<GameObject>(GameHexPrefab);
        
        newGameObject.transform.parent = transform;
        newGameObject.name += UnityEngine.Random.value;

        GameHex newGameHex = newGameObject.GetComponent<GameHex>();
        newGameHex.hex = hex;
        hexes.Add(newGameHex);

        Point position = Layout.HexToPixel(layout, hex);
        newGameObject.transform.position = new Vector3(position.x, 0.2f, position.y);
        
    }

    private void OnHexClicked(GameHex hex)
    {
        tempLayout = layout;
        turning = true;
        Rotate();


    }

    void Update()
    {
        /*if (turning)
        {
            Orientation tempOrientation = tempLayout.orientation;
            tempOrientation.start_angle += Time.deltaTime;
            tempLayout = new Layout(tempOrientation, layout.size, layout.origin);
            foreach (GameHex hex in hexes)
            {
                Point position = Layout.HexToPixel(tempLayout, hex.hex);
                hex.gameObject.transform.position = new Vector3(position.x, 0.2f, position.y);
            }
        }*/
    }

    void Rotate()
    {
        foreach (GameHex hex in hexes)
        {
            hex.Rotate(1);
            Point position = Layout.HexToPixel(layout, hex.hex);
            hex.transform.position = new Vector3(position.x, 0.2f, position.y);
        }
    }


}


