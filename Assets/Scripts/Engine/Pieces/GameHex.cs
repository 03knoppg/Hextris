using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GameHex : MonoBehaviour
{
    [SerializeField]
    //public?
    public Hex hex;
    public static Layout GolbalLayout;

    public MeshRenderer inner;
    public List<MeshRenderer> corners;


    public delegate void ClickAction(GameHex hex);
    public event ClickAction OnHexClicked;

    public delegate void MouseDownAction(GameHex hex);
    public event MouseDownAction OnHexMouseDown;

    public delegate void Collision();
    public event Collision OnCollision;

    public delegate void CollisionExit();
    public event CollisionExit OnCollisionExit;

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
    public Point GlobalPoint
    {
        get
        {
            return new Point(transform.position.x, transform.position.z);
        }
    }

    void Awake()
    {
        hex = new Hex();
    }

    void Update()
    {
        OffsetCoord coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);

        Hex globalHex = FractionalHex.HexRound(Layout.PixelToHex(GolbalLayout, GlobalPoint));
        name = "Hex Global{" + globalHex.q + ", " + globalHex.r + ", " + globalHex.s + "} " + "Offset{" + coord.col + ", " + coord.row + "} Cube{" + hex.q + ", " + hex.r + ", " + hex.s + "}";

    }

    public void Init(Layout globalLayout)
    {
        GolbalLayout = globalLayout;
    }

    void OnMouseUpAsButton()
    {
        if (OnHexClicked != null)
            OnHexClicked(this);
    }

    void OnMouseDown()
    {
        if (OnHexMouseDown != null)
            OnHexMouseDown(this);
    }

    //positive increments of 60 degrees clockwise
    public void Rotate(int amount)
    {
        hex = HexCalcs.RotateHex(hex, -amount);
    }

    public void UpdateLayout(Layout oldLayout, Layout newLayout)
    {
        //translate into new layout based on new pivot hex
        hex = FractionalHex.HexRound(Layout.PixelToHex(newLayout, Layout.HexToPixel(oldLayout, hex)));
        //UpdatePosition(newLayout);
        UpdatePosition(oldLayout);
    }

    public void UpdatePosition(Layout localLayout)
    {
        //OffsetCoord oc = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);
        //transform.localPosition = new Vector3(oc.col * localLayout.size.x, 0, oc.row * localLayout.size.y);
        Point position = Layout.HexToPixel(localLayout, hex);
        transform.localPosition = new Vector3(position.x, 0, position.y);

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

    void OnTriggerExit(Collider other)
    {
        if (OnCollisionExit != null)
            OnCollisionExit();
    }

    public static bool operator ==(GameHex a, GameHex b) 
    {
        if (a.Equals(b))
            return true;

        return FractionalHex.HexRound(Layout.PixelToHex(GolbalLayout, a.GlobalPoint)) ==
            FractionalHex.HexRound(Layout.PixelToHex(GolbalLayout, b.GlobalPoint));
    }

    public static bool operator !=(GameHex a, GameHex b)
    {
        return !(a == b);
    }

    internal void SetColourInner(Material mat)
    {
        inner.sharedMaterials = new Material[] { inner.sharedMaterials[0], mat };
    }

    internal void SetColourOuter(Material mat)
    {
        inner.sharedMaterials = new Material[] { mat, inner.sharedMaterials[1] };
        foreach (MeshRenderer corner in corners)
            corner.material = mat;
    }
}

