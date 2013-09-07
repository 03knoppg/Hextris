using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class driver : MonoBehaviour
	{
		void Start(){
			
			PieceMaker.Make(PieceMaker.Shape.I);	
		}
	}
}

