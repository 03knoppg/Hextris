using System;
using System.Collections.Generic;
using UnityEngine;


public class PieceMaker
{
    static GameObject PiecePrefab;

    
	public enum Shape {
		L, //not yet implemented
		I
	};
	
    static Dictionary<Shape, int[,]> shapes = new Dictionary<Shape, int[,]>()
	{
        //Axial coodrdinates
		{Shape.I, new int[4,2]{{0,0},{0,-1},{0,-2},{0,-3}}}
		
		
	};
	
	public static Piece Make(Shape shape){
        if(PiecePrefab == null)
            PiecePrefab = (GameObject)Resources.Load("3DAssets/Piece");

        GameObject newPiece = GameObject.Instantiate(PiecePrefab);

        Piece piece = newPiece.GetComponent<Piece>();
        

        int[,] points = shapes[shape];
        for (int i = 0; i < points.Length / 2; i++)
        {
            Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(points[i,0], points[i,1]));
            piece.AddHex(hex); 
        }

        return piece;
	
	}
}


