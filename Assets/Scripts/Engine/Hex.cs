using UnityEngine;
using System.Collections;

public class Hex : MonoBehaviour {
	
	
	//distance from center
	int r;
	//clockwise position [0, 6 * r) (12:00 is 0)
	int t;
	
	public void Init(int r, int t){
		this.r = r;
		this.t = t;
		
		
		//move hex into position
		float theta = r == 0 ? 0 : (Mathf.PI * 2 * t) / (6 * r);
		
		transform.Translate(new Vector3(Mathf.Cos(theta) * r * transform.collider.bounds.extents.magnitude, 2,  Mathf.Sin(theta) * r * transform.collider.bounds.extents.magnitude));
	}
	
}
