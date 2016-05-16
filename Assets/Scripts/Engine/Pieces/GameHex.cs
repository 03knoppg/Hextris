using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GameHex : MonoBehaviour
{
    [SerializeField]
    Hex hex;


    public delegate void ClickAction(GameHex hex);
    public event ClickAction OnClicked;

    public delegate void Collision();
    public event Collision OnCollision;

    public bool IsPivotHex
    {
        get
        {
            return Hex.Length(hex) == 0;
        }
    }
    public Point LocalPoint
    {
        get
        {
            return new Point(transform.localPosition.x, transform.localPosition.z);
        }
    }

    void Awake()
    {
        hex = new Hex();
    }

    void OnMouseUpAsButton()
    {
        if(OnClicked != null)
            OnClicked(this);
    }

    //positive increments of 60 degrees clockwise
    public void Rotate(int amount)
    {
        hex = HexCalcs.RotateHex(hex, -amount);
    }

    public void SetColour(Color newColour)
    {
        GetComponent<Renderer>().material.SetColor("_Color", newColour);
    }

    public void UpdateLayout(Layout oldLayout, Layout newLayout)
    {
        //old pivot hex
        if (IsPivotHex)
            SetColour(Color.blue);

        //translate into new layout based on new pivot hex
        hex = FractionalHex.HexRound(Layout.PixelToHex(newLayout, Layout.HexToPixel(oldLayout, hex)));
        //UpdatePosition(newLayout);
        UpdatePosition(oldLayout);
    }

    public void UpdatePosition(Layout localLayout)
    {
        Point position = Layout.HexToPixel(localLayout, hex);
        transform.localPosition = new Vector3(position.x, 0, position.y);

        OffsetCoord coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);
        name = "Hex{" + coord.col + ", " + coord.row + "}";
    }

    public void SetPosition(Layout layout, Hex hex)
    {
        this.hex = hex;
        UpdatePosition(layout);
    }

    public bool Equals(Hex otherHex)
    {
        return otherHex == hex;
    }

    void OnTriggerEnter(Collider other)
    {
        if (OnCollision != null)
            OnCollision();
    }
}

