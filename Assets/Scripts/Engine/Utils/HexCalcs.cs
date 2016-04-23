using System;
using UnityEngine;

public static class HexCalcs
{



	public static int[] adjacentPositions(int position)
	{
		
		if(position == 0)
			return new int[] { 1, 2, 3, 4, 5, 6 };
		
		
		int[] adj = new int[6];	
		
		int rank = GetRank(position);
		bool cornerPiece = (position - 1) % rank == 0;
		
		adj[0] = cwPosition(position);
		adj[1] = ccwPosition(position);
		
		//if(shapeNum << (position - 1)){
		if(cornerPiece)
		{
			//straight out
			adj[2] = position + (6 * rank) + ((position - totalPositionsByRank(rank - 1)) / rank);
			adj[3] = cwPosition(adj[2]);
			adj[4] = ccwPosition(adj[2]);
			//straight in
			adj[5] = position - positionsInRank(rank - 1) - ((position - totalPositionsByRank(rank - 1)) / rank);
			
		}
		else
		{
			adj[2] = position + positionsInRank(rank) 	   + ((position - totalPositionsByRank(rank - 1)) / rank);
			adj[3] = cwPosition(adj[2]);
			adj[4] = position - 1 - positionsInRank(rank - 1) - ((position - totalPositionsByRank(rank - 1)) / rank);
			adj[5] = cwPosition(adj[4]);
			
		}
		
		return adj;
	}
	
	//bug here
	//return position adjcent to position in same rank in counter clockwise direction
	public static int ccwPosition(int position)
	{
		if(position <= 0)
			throw new ArgumentOutOfRangeException(position + " : position must be > 0");
		
		//could use some optimization
		int rank = GetRank (position);
		
        //???
		//return ((position - 1 - totalPositionsByRank(rank - 1)) + (6 * rank) ) % (rank * 6) + totalPositionsByRank(rank - 1);

        return rank == GetRank(position - 1) ? position - 1 : position + positionsInRank(rank) - 1;
    
    }
	
	
	//return position adjcent to position in same rank in clockwise direction
	public static int cwPosition(int position)
	{
		if(position <= 0)
			throw new ArgumentOutOfRangeException(position + " : position must be > 0");
		
		//could use some optimization
		int rank = GetRank (position);
		
        //?
		//return ((position + 1 - totalPositionsByRank(rank -1)) % (rank * 6) ) + totalPositionsByRank(rank -1);

        return (position + 1) % positionsInRank(rank);
    
    }
	
	public static int GetRank(int position)
	{
		return (int)Mathf.Ceil((Mathf.Sqrt((12 * (position + 1)) - 3) - 3) / 6);
	}
		
	public static int totalPositionsByRank(int rank)
	{
		if(rank < 1)
			return 1;
		
		return (3*rank*rank) + (3*rank) + 1;
			
	}
	
	public static int positionsInRank(int rank)
	{
		if(rank == 0)
			return 1;
		
		return 6 * rank;
	}
	
	public static int relativePosition(int p1, int p2)
	{
		//TODO!
		throw new NotImplementedException();
		
	}

    static public Hex cube_to_hex(Vector2 h) // axial
    {
        int q = (int)h.x;
        int r = (int)h.y;
        return new Hex(q, -(q + r), r);
    }

    public static Vector3 hex_to_cube(Hex h)//: # axial
    {
        float x = h.q;
        float z = h.r;
        float y = -x - z;
        return new Vector3(x, y, z);
    }

    public static Hex RotateHex(Hex hex, int direction)
    {

        Vector3 cube = HexCalcs.hex_to_cube(hex);
        for (; direction > 0; direction--)
        {
            float temp = cube.x;
            cube.x = -cube.z;
            cube.z = -cube.y;
            cube.y = -temp;
        }
        for (; direction < 0; direction++)
        {
            float temp = cube.x;
            cube.x = -cube.y;
            cube.y = -cube.z;
            cube.z = -temp;
        }
        return HexCalcs.cube_to_hex(cube);
    }
}


