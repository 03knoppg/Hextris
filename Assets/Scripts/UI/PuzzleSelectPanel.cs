using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PuzzleSelectPanel : UIThing {

    public PuzzleSelectButton PuzzleSelectButtonPrefab;

	// Use this for initialization
	new void Start () {
        base.Start();

        List<Game> GamePrefabs = FindObjectOfType<Driver>().GamePrefabs;

        for(int i = 0; i < GamePrefabs.Count; i ++)
        {
            PuzzleSelectButton button = Instantiate<PuzzleSelectButton>(PuzzleSelectButtonPrefab);
            button.levelIndex = i;
            button.GetComponentInChildren<Text>().text = GamePrefabs[i].name;
            button.transform.SetParent(transform);
        }
	}
}
