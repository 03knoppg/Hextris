using System;
using UnityEngine;


public class Driver : MonoBehaviour
{

    public float size = 5;
    Piece IPiece;

    public static Layout layout;

	void Start(){

        layout = new Layout(Layout.pointy, new Point(size, size), new Point(0, 0));

        GameObject boardObject = new GameObject();
        RectangleBoard board = boardObject.AddComponent<RectangleBoard>();

        board.InitBoard();

        IPiece = PieceMaker.Make(PieceMaker.Shape.I);	
	}


}

 


