using UnityEngine;
using System.Collections.Generic;
using System.Collections;

				

public abstract class Board : MonoBehaviour{

    public GameHex GameHexPrefab;
    public List<Hex> legalStartingHexesP1;
    public List<Hex> legalStartingHexesP2;
    protected Hex[,] Hexes;

    void Start()
    {
        InitBoard();
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
}
