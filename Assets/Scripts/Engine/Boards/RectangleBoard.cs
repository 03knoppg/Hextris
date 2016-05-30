using System;
using System.Collections.Generic;
using UnityEngine;

public class RectangleBoard : Board
{
    public int columns = 15;
    public int rows = 15;

    public override void InitBoard()
    {
        name = "RectangleBoard";

        gameHexes = new List<GameHex>();

        GameHex newHex;
        Hexes = new Hex[columns, rows];

		for(int col = 0; col < columns; col ++){

            for (int row = 0; row < rows; row++)
            {
                Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(col,row));
                OffsetCoord coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);

                Hexes[coord.col, coord.row] = hex;

                newHex = Instantiate<GameHex>(GameHexPrefab);
                newHex.transform.parent = transform;
                gameHexes.Add(newHex);
                Destroy(newHex.GetComponent<Collider>());
                
                newHex.SetPosition(Game.layout, hex);
                foreach (MeshRenderer corner in newHex.corners)
                    corner.gameObject.SetActive(true);

                if (col == 0 && row % 2 == 1)
                    legalStartingHexesP1.Add(hex);
                
                else if (col == columns - 1 && row % 2 == 0)
                    legalStartingHexesP2.Add(hex);
            
			}
		}
	}
}


