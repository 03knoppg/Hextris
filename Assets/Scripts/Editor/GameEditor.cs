using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Game))]
public class GameEditor : Editor
{

    void OnSceneGUI()
    {
        Game game = target as Game;
        if(game == null || game.type != Game.GameType.Puzzle)
            return;

        game.name = "Puzzle" + game.order;
    }
}
