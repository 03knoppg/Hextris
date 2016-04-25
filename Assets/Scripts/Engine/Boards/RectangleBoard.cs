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

		GameObject hexObjPrefab = (GameObject) Resources.Load("3DAssets/HexObject");

        GameObject newHex;
        Hexes = new Hex[columns, rows];

		for(int col = 0; col < columns; col ++){

            for (int row = 0; row < rows; row++)
            {
                Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(col,row));
                OffsetCoord coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);

                Hexes[coord.col, coord.row] = hex;

                newHex = (GameObject)Instantiate(hexObjPrefab);
                newHex.transform.parent = transform;
                newHex.name = "Hex " + coord.col + " " + coord.row;
                newHex.GetComponent<GameHex>().SetColour(Color.red);

                Point p = Layout.HexToPixel(Driver.layout, hex);
                newHex.transform.position = new Vector3(p.x, 0, p.y);
 
			}
		}
	}
		

}


