using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {

    public List<Piece> pieces = new List<Piece>();
    public string Name;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetActivePlayer(bool active)
    {
        foreach (Piece p in pieces)
        {
            p.SetActive(active);
        }
    }
}
