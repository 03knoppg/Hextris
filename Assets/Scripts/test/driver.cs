using System;
using UnityEngine;


public class Driver : MonoBehaviour
{

    public Game ClassicGamePrefab;

    public float size = 5;

    public static Layout layout;

	void Start(){

        layout = new Layout(Layout.pointy, new Point(size, size), new Point(0, 0));

        Game newGame = GameObject.Instantiate<Game>(ClassicGamePrefab);
        newGame.StartGame();
	}


}

 


