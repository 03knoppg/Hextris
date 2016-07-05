using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HextrisCam : MonoBehaviour {

    //Vector3 oldMousePos;
    float hDist;
    float vDist;
    float distance;
    float angle;
    Bounds bounds;
    Camera cam;

	// Use this for initialization
	void Start () {
        FindObjectOfType<Signals>().AddListeners(OnCamPosition, new List<Signal>() { Signal.CamPosition });
	}

    private void OnCamPosition(Signal signal, object arg1)
    {

        bounds = (Bounds)arg1;

        cam = GetComponent<Camera>();

    }

    void Update()
    {
        if (cam == null)
            return;
        
        float vFOV = Mathf.Deg2Rad * cam.fieldOfView;
        float hFOV = 2 * Mathf.Atan(Mathf.Tan(vFOV / 2) * cam.aspect);
      

        angle = Mathf.Deg2Rad * transform.rotation.eulerAngles.x;

        hDist = bounds.extents.x / Mathf.Sin(hFOV / 2);

        vDist = Mathf.Sin(angle) * bounds.extents.z / Mathf.Sin(vFOV / 2);

        distance = Mathf.Max(hDist, vDist);


        float x = bounds.center.x;

        float y = bounds.center.y + distance * Mathf.Sin(angle);

        float z = bounds.center.z - (distance + bounds.extents.z) * Mathf.Cos(angle);

        transform.position = new Vector3(x, y, z);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
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
