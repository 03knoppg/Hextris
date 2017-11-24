using System.Collections.Generic;
using UnityEngine;

public class CustomBoard : Board
{
    public override List<GameHex> Hexes
    {
        get
        {
            return new List<GameHex>(GetComponentsInChildren<GameHex>());
        }
        set
        {
            base.Hexes = value;
        }
    }
    
    protected override void BuildBoard()
    {
        foreach (GameHex gHex in Hexes)
        {
            if(gHex.layer == 0)
                Destroy(gHex.GetComponent<Collider>());

            foreach (MeshRenderer corner in gHex.corners)
                corner.gameObject.SetActive(true);
		}
	}

    //public override void HighlightPlayer(int playerIndex)
    //{
    //    List<GameHex> highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
    //    foreach (GameHex gHex in Hexes)
    //    {
    //        if (highlightGameHexes.Contains(gHex))
    //            gHex.SetColourOuter(highlight);
    //        else
    //            gHex.SetColourOuter(outer);

    //    }
    //}

    public override bool InStartingArea(Hex hex, int playerIndex)
    {
        List<GameHex> highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
        
        foreach (GameHex highlightGameHex in highlightGameHexes)
        {
            if (highlightGameHex.hex == hex)
                return true;
        }
        return false;
    }
}


