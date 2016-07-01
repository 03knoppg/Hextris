using UnityEngine;
using System.Collections.Generic;
using System.Collections;

				
public abstract class Board : MonoBehaviour{

    public virtual List<GameHex> Hexes { get; set; }

    public List<GameHex> LegalStartingHexesP1;
    public List<GameHex> LegalStartingHexesP2;
    
    protected Signals UISignals;

    public Material inner;
    public Material outer;
    public Material highlight;
    public Material outerHighlight;

    List<GameHex> highlightGameHexes;
    float highlightAngle;

    public virtual void Init(Signals UISignals)
    {
        this.UISignals = UISignals;

        BuildBoard();

        foreach (GameHex gHex in Hexes)
        {
            gHex.SetColourInner(inner);
            gHex.SetColourOuter(outer);
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
        highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
        
    }

    public virtual void Update()
    {
        foreach (GameHex gHex in Hexes)
        {
            if (highlightGameHexes.Contains(gHex))
            {
                gHex.SetColourOuter(outerHighlight);

            }
            else
                gHex.SetColourOuter(outer);

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
