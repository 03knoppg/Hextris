using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public enum GameType
    {
        Classic
    }

    [SerializeField]
    GameType type;

    [SerializeField]
    GameObject BoardPrefab;

    public void StartGame()
    {

        GameObject.Instantiate(BoardPrefab);

        Piece IPiece = PieceMaker.Make(PieceMaker.Shape.I);	

    }
}
