using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class driver : MonoBehaviour
    {

        public float size = 5;
        Piece IPiece;

		void Start(){

            Layout layout = new Layout(Layout.pointy, new Point(size, size), new Point(0, 0));

            GameObject boardObject = new GameObject();
            RectangleBoard board = boardObject.AddComponent<RectangleBoard>();

            board.InitBoard(layout);

            IPiece = PieceMaker.Make(layout, PieceMaker.Shape.I);	
		}


	}

 
}

