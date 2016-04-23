using System;
using System.Collections.Generic;
using UnityEngine;


public class PieceMaker
{
    
	public enum Shape {
		L,
		I
	};
	
    static Dictionary<Shape, int[,]> shapes = new Dictionary<Shape, int[,]>()
	{

		{Shape.I, new int[4,2]{{0,0},{0,-1},{0,-2},{0,-3}}}
		
		
	};
	
	public static Piece Make(Layout layout, Shape shape){

        GameObject piecePrefab = (GameObject)Resources.Load("3DAssets/Piece");

        GameObject newPiece = GameObject.Instantiate(piecePrefab);

        Piece piece = newPiece.GetComponent<Piece>();
        piece.layout = layout;
        

        int[,] points = shapes[shape];
        for (int i = 0; i < points.Length / 2; i++)
        {
            Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(points[i,0], points[i,1]));
            piece.AddHex(hex); 
        }

        return piece;
	
	}


}


