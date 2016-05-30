using UnityEngine;
using System.Collections.Generic;
using System.Collections;

				

public abstract class Board : MonoBehaviour{

    public GameHex GameHexPrefab;
    protected List<GameHex> gameHexes;
    public List<Hex> legalStartingHexesP1;
    public List<Hex> legalStartingHexesP2;
    protected Hex[,] Hexes;

    UISignals UISignals;

    public Material inner;
    public Material outer;
    public Material highlight;

    void Awake()
    {
        InitBoard();
        foreach (GameHex gHex in gameHexes)
        {
            gHex.SetColourInner(inner);
            gHex.SetColourOuter(outer);
        }
    }

    void Start()
    {
        UISignals = FindObjectOfType<UISignals>();
        
    }
    public abstract void InitBoard();


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
        List<Hex> highlightHexes = playerIndex == 0 ? legalStartingHexesP1 : legalStartingHexesP2;
        foreach (GameHex gHex in gameHexes)
        {
            if(highlightHexes.Contains(gHex.hex))
                gHex.SetColourOuter(highlight);
            else
                gHex.SetColourOuter(outer);

        }
    }
}
