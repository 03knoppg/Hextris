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
		
		Piece basePiece = new Piece();
		
		//converts the number from base 2 to base 10
		int shapeNum = Convert.ToInt32(shapes[shape].ToString(), 2);
		
		
		pieces = new List<Piece>();
		
		//addLinks(0, shapeNum);
		
		return basePiece;
	
	}
	
	static void addLinks(int position, int shapeNum){
				
		pieces[position] = new Piece();
		
		int[] adj = adjacentPositions(position);
		
		for(int i = 0; i < adj.Length; i ++){
		
			if(pieces[adj[i]] == null)
				addLinks(adj[i], shapeNum);
			
			pieces[position].links[124314134] = pieces[adj[i]];
		}
		
		
		
		
	}
	
	static int[] adjacentPositions(int position)
	{
		int[] adj = new int[6];	
		
		int rank = getRank(position);
		bool cornerPiece = (position - 1) % rank == 0;
		
		adj[0] = cwPosition(position);
		adj[1] = ccwPosition(position);
		
		//if(shapeNum << (position - 1)){
		if(cornerPiece)
		{
			//straight out
			adj[2] = position + (6 * rank) + ((position - totalPositionsByRank(rank)) / rank);
			adj[3] = cwPosition(adj[2]);
			adj[4] = ccwPosition(adj[2]);
			//straight in
			adj[5] = position - (6 * (rank - 1)) - ((position - totalPositionsByRank(rank)) / rank);
			
		}
		else
		{
			adj[2] = (int)Mathf.Floor(position + (6 * rank) + ((position - totalPositionsByRank(rank)) / rank));
			adj[3] = (int)Mathf.Ceil(position + (6 * rank) + ((position - totalPositionsByRank(rank)) / rank));
			adj[4] = (int)Mathf.Floor(position - (6 * (rank - 1)) - ((position - totalPositionsByRank(rank)) / rank));
			adj[5] = (int)Mathf.Ceil(position - (6 * (rank - 1)) - ((position - totalPositionsByRank(rank)) / rank));
			
		}
		
		return adj;
	}
	
	//bug here
	//return position adjcent to position in same rank in couter clockwise direction
	static int ccwPosition(int position)
	{
		if(position <= 0)
			throw new ArgumentOutOfRangeException(position + " : position must be > 0");
		
		//could use some optimization
		int rank = getRank (position);
		
		return ((position - 1 - totalPositionsByRank(rank - 1)) % (rank * 6) ) + totalPositionsByRank(rank);
	}
	
	
	//return position adjcent to position in same rank in clockwise direction
	static int cwPosition(int position)
	{
		if(position <= 0)
			throw new ArgumentOutOfRangeException(position + " : position must be > 0");
		
		//could use some optimization
		int rank = getRank (position);
		
		return ((position + 1 - totalPositionsByRank(rank -1)) % (rank * 6) ) + totalPositionsByRank(rank -1);
	}
	
	static int getRank(int position)
	{
		return (int)Mathf.Ceil((Mathf.Sqrt((12 * (position + 1)) - 3) - 3) / 6);
	}
		
	static int totalPositionsByRank(int rank)
	{
		
		return (3*rank*rank) + (3*rank) + 1;
			
	}
}


