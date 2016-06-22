using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {

    public List<Piece> pieces = new List<Piece>();
    public string Name;


	// Update is called once per frame
	void Update () {
	
	}

    public void SetActivePlayer(bool active)
    {
        foreach (Piece p in pieces)
        {
            if (active)
                p.Mode = Piece.EMode.Active;
            
            else
                p.Mode = Piece.EMode.Disabled;
        }
    }

    internal void ClearPieces()
    {
        foreach (Piece piece in pieces)
            GameObject.Destroy(piece.gameObject);

        pieces.Clear();
    }
}
