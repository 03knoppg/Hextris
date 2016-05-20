using System;
using UnityEngine;


public class Driver : MonoBehaviour
{

    public Game GamePrefab;


	void Awake(){
        Game newGame = Instantiate<Game>(GamePrefab);
	}


}

 


