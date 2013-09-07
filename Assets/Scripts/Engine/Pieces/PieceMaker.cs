using System;
using System.Collections.Generic;


public class PieceMaker
{

	public enum Shape {
		L,
		I
	};
	
	static Dictionary<Shape,int> shapes = new Dictionary<Shape, int>()
	{
	
		{Shape.I, 10000011}
		
		
	};
	
	static Piece Make(Shape shape){
		
		Piece basePiece = new Piece();
		
		int shapeNum = shapes[shape];
		
		while(shapeNum > 0)	{
			if((shapeNum % 2) == 1){
				basePiece.links[i] = new Piece();
			}			
			shapeNum /= 10;
		}		
	
	}
	
	static void addLinks(int position, int shapeNum){
				
		int rank = position == 0 ? 0 : Mathf.Ceil((Mathf.Sqrt((12 * position) - 3) - 3) / 6);
		bool cornerPiece = (position - 1) % rank == 0;
	}
	
}


