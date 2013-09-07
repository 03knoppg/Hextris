using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class CircleBoard : Board
	{
		public int radius = 5;
		
		
		
		protected override void InitBoard(){
			
			GameObject hexObj = (GameObject) Resources.Load("3DAssets/HexObject");
		
			GameObject newHex = (GameObject) Instantiate(hexObj);
			newHex.GetComponent<Hex>().Init(0,0);
					
			//r radius
			for(int r = 0; r < radius; r++){
				
				// t theta
				for(int t = 0; t < 6 * r; t ++){
					
					newHex = (GameObject) Instantiate(hexObj);
					
					newHex.GetComponent<Hex>().Init(r,t);
					
					hexObjects.Add(newHex);
				}
			}
		}
	}
}

