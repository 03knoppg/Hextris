using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HextrisCam : MonoBehaviour {

    //Vector3 oldMousePos;

	// Use this for initialization
	void Start () {
        FindObjectOfType<UISignals>().AddListeners(OnCamPosition, new List<UISignal>() { UISignal.CamPosition });
	}

    private void OnCamPosition(UISignal signal, object arg1)
    {
        transform.position = (Vector3) arg1;
    }
	
    

	// Update is called once per frame
    //void Update () {
    //    if (Input.GetMouseButton(1))
    //    {
    //        Vector3 mouseDelta = (Input.mousePosition - oldMousePos) / 100;

    //        transform.position = new Vector3(
    //            transform.position.x - mouseDelta.x,
    //            transform.position.y,
    //            transform.position.z - mouseDelta.y);
    //    }
    //    oldMousePos = Input.mousePosition;
    //}
}
