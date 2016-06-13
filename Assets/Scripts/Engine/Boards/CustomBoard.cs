using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomBoard : Board
{

    
    protected override void BuildBoard()
    {
        Hexes = new List<GameHex>(GetComponentsInChildren<GameHex>());
        foreach (GameHex gGex in Hexes)
        {
            Destroy(gGex.GetComponent<Collider>());

            foreach (MeshRenderer corner in gGex.corners)
                corner.gameObject.SetActive(true);
		}
	}

    public override void HighlightPlayer(int playerIndex)
    {
        List<GameHex> highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
        foreach (GameHex gHex in Hexes)
        {
            if (highlightGameHexes.Contains(gHex))
                gHex.SetColourOuter(highlight);
            else
                gHex.SetColourOuter(outer);

        }
    }

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


