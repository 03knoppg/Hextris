using UnityEngine;
using System.Collections.Generic;
using System.Collections;

				
/*
 * A board is defined as a set of hexes. It is arranged as concentric
 * circles around a center point. Each ring around the center has 6*r hexes.
 */
public abstract class Board : MonoBehaviour{
		
	
	protected List<GameObject> hexObjects;
	
	
	void Start(){
		hexObjects = new List<GameObject>();
		
		
		InitBoard();
		
	//	addBoardToScene();
	}
	
	
	protected abstract void InitBoard();
	
	public void addBoardToScene(){
		
		
		foreach(GameObject hexObj in hexObjects){
			
			hexObj.transform.parent = transform;
			
		}
	}


}
