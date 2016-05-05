using UnityEngine;
using System.Collections.Generic;
using System.Collections;

				

public abstract class Board : MonoBehaviour{

    public GameHex GameHexPrefab;
    protected Hex[,] Hexes;

    void Start()
    {
        InitBoard();
    }
    public abstract void InitBoard();
	
}
