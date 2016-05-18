using System;
using System.Collections.Generic;
using UnityEngine;


public class PieceMaker : MonoBehaviour
{
    public Piece PiecePrefab;


    public enum Shape
    {
        //L, //not yet implemented
        //I, //not yet implemented
        Triangle,
        S,
        C
    };
	
    static Dictionary<Shape, int[,]> shapes = new Dictionary<Shape, int[,]>()
	{
        //Axial coodrdinates?
		{Shape.Triangle,    new int[4,2]{{0,0},{0,1},{1,0},{0,-1}}},

		{Shape.S,           new int[4,2]{{0,0},{0,-1},{0,-2},{0,-3}}},

		{Shape.C,           new int[4,2]{{0,0},{0,1},{0,2},{1,2}}}

		
		
	};
	
	public Piece Make(Shape shape){

        Piece piece = Instantiate<Piece>(PiecePrefab);
                

        int[,] points = shapes[shape];
        for (int i = 0; i < points.Length / 2; i++)
        {
            Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(points[i,0], points[i,1]));
            piece.AddHex(hex); 
        }

        return piece;
	
	}
}


