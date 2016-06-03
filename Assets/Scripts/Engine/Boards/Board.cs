using UnityEngine;
using System.Collections.Generic;
using System.Collections;

				
[ExecuteInEditMode]
public abstract class Board : MonoBehaviour{

    protected List<GameHex> gameHexes;
    List<Hex> legalStartingHexesP1;
    public virtual List<Hex> LegalStartingHexesP1
    { get { return legalStartingHexesP1; } }
    List<Hex> legalStartingHexesP2;
    public virtual List<Hex> LegalStartingHexesP2
    { get { return legalStartingHexesP2; } }


    protected UISignals UISignals;
    protected Layout globalLayout;

    public Material inner;
    public Material outer;
    public Material highlight;

    List<Hex> hexes;
    public virtual List<Hex> Hexes
    { get { return hexes; } }


    public virtual void InitBoard(Layout globalLayout, UISignals UISignals)
    {
        this.globalLayout = globalLayout;
        this.UISignals = UISignals;

        BuildBoard();

        foreach (GameHex gHex in gameHexes)
        {
            gHex.SetColourInner(inner);
            gHex.SetColourOuter(outer);
        }
    }

    protected abstract void BuildBoard();

    internal bool InBounds(Hex hex)
    {
        foreach (Hex legalHex in Hexes)
        {
            if (hex == legalHex)
                return true;
        }
        return false;
    }
    
    public void HighlightPlayer(int playerIndex)
    {
        List<Hex> highlightHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
        foreach (GameHex gHex in gameHexes)
        {
            if(highlightHexes.Contains(gHex.hex))
                gHex.SetColourOuter(highlight);
            else
                gHex.SetColourOuter(outer);

        }
    }
}
