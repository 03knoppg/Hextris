using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[SelectionBase, Serializable] 
public class GameHex : MonoBehaviour
{
    [SerializeField]
    public OffsetCoord coord;

    //public?
    public Hex hex;

    public MeshRenderer inner;
    public List<MeshRenderer> corners;
    
    public delegate void MouseDownAction(GameHex hex);
    public event MouseDownAction OnHexMouseDown;

    public delegate void Collision();
    public event Collision OnCollision;

    public delegate void CollisionExit();
    public event CollisionExit OnCollisionExit;

    public int layer = 0;

    public bool IsPivotHex
    {
        get
        {
            return Hex.Length(hex) == 0;
        }
    }
    private Point LocalPoint
    {
        set
        {
            transform.localPosition = new Vector3(value.x, 0.2f * layer, value.y);
        }
    }
    public Point GlobalPoint
    {
        get
        {
            return new Point(transform.position.x, transform.position.z);
        }
    }
    
    void Update()
    {
        OffsetCoord coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);

        Hex globalHex = FractionalHex.HexRound(Layout.PixelToHex(GlobalPoint));
        name = "Hex Global{" + globalHex.q + ", " + globalHex.r + ", " + globalHex.s + "} " + "Offset{" + coord.col + ", " + coord.row + "} Cube{" + hex.q + ", " + hex.r + ", " + hex.s + "}";

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

    public void UpdateHex(Point pivotGlobalPoint)
    {
        //translate into new position based on new pivot hex
        hex = FractionalHex.HexRound(Layout.PixelToHex(GlobalPoint - pivotGlobalPoint));
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        LocalPoint = Layout.HexToPixel(hex);
    }

    public void SetPosition(Hex hex)
    {
        this.hex = hex;
        UpdatePosition();
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

        return FractionalHex.HexRound(Layout.PixelToHex(a.GlobalPoint)) ==
            FractionalHex.HexRound(Layout.PixelToHex(b.GlobalPoint));
    }

    public static bool operator !=(GameHex a, GameHex b)
    {
        return !(a == b);
    }

    public override bool Equals(object o)
    {
        return base.Equals(o);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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

