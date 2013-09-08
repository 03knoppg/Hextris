using System;
using System.Collections.Generic;
using UnityEngine;


public class PieceMaker
{

	public enum Shape {
		L,
		I
	};
	
	static List<Piece> pieces;
	
	static Dictionary<Shape,int> shapes = new Dictionary<Shape, int>()
	{
		//base 2
		{Shape.I, 10000011}
		
		
	};
	
	public static Piece Make(Shape shape){
		
		adjacentPositions(10);
		
		Piece basePiece = new Piece();
		
		//converts the number from base 2 to base 10
		int shapeNum = Convert.ToInt32(shapes[shape].ToString(), 2);
		
		
		pieces = new List<Piece>();
		
		//addLinks(0, shapeNum);
		
		return basePiece;
	
	}
	
	static void addLinks(int position, int shapeNum){
				
		pieces[position] = new Piece();
		
		int[] adj = HexCalcs.adjacentPositions(position);
		
		for(int i = 0; i < adj.Length; i ++){
		
			if(pieces[adj[i]] == null)
				addLinks(adj[i], shapeNum);
			
			pieces[position].links[HexCalcs.relativePosition(position, adj[i])] = pieces[adj[i]];
		}
		
	}
	
	
}


