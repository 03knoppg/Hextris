using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

    Vector3 oldMousePos;

	// Use this for initialization
	void Start () {
        oldMousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - oldMousePos;

            transform.position = new Vector3(
                transform.position.x + mouseDelta.x,
                transform.position.y + mouseDelta.y,
                transform.position.z + Input.mouseScrollDelta.x);
        }
        oldMousePos = Input.mousePosition;
	}
}
