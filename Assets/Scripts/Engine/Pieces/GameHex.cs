using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GameHex : MonoBehaviour
{
    //would like to set transform.position here but dont want to piece's local layout
    public Hex hex;


    public delegate void ClickAction(GameHex hex);
    public static event ClickAction OnClicked;


    void OnMouseUpAsButton()
    {
        if(OnClicked != null)
            OnClicked(this);
    }

    //positive increments of 60 degrees clockwise
    public void Rotate(int amount)
    {
        hex = HexCalcs.RotateHex(hex, amount);
    }

    public void SetColour(Color newColour)
    {
        GetComponent<Renderer>().material.SetColor("_Color", newColour);
    }
}

