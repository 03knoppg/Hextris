using System.Collections.Generic;
using UnityEngine;


public abstract class Board : MonoBehaviour{

	public virtual List<GameHex> Hexes { get; set; }

	[SerializeField]
	EColorPallet ColorPallet;

	public List<GameHex> LegalStartingHexesP1;
	public List<GameHex> LegalStartingHexesP2;

	float highlightAngle;

	public virtual void Init()
	{
		BuildBoard();

		foreach (GameHex gHex in Hexes)
		{
			gHex.SetColourInner(BoardColourPallet.Inner(ColorPallet));
			gHex.SetColourOuter(BoardColourPallet.Outer(ColorPallet));
		}
	}

	protected abstract void BuildBoard();

	internal bool InBounds(Hex hex)
	{
		foreach (GameHex legalHex in Hexes)
		{
			if (hex == legalHex.hex)
				return true;
		}
		return false;
	}
	
	public virtual void HighlightPlayer(int playerIndex)
	{
        HighlightGameHexes(playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2);
		
	}

    void HighlightGameHexes(List<GameHex> highlightHexes)
    {
        foreach (GameHex gHex in Hexes)
        {
            Material mat = highlightHexes.Contains(gHex) ?
                BoardColourPallet.OuterHighlight(ColorPallet) :
                BoardColourPallet.Outer(ColorPallet);
            
            gHex.SetColourOuter(mat);

        }
    }
    

	public virtual bool InStartingArea(Piece piece, int playerIndex)
	{
		foreach(GameHex gHex in piece.GameHexes)
		{
			Hex tempHex = FractionalHex.HexRound(Layout.PixelToHex(gHex.GlobalPoint));
			if (InStartingArea(tempHex, playerIndex))
				return true;   
		}
		return false;
	}

	public virtual bool InStartingArea(Hex globalHex, int playerIndex)
	{
		List<GameHex> highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
		foreach (GameHex gHex in highlightGameHexes)
		{
			if (gHex.hex == globalHex)
				return true;
		}
		return false;
	}
}
