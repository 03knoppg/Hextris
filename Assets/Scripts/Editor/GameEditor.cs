using UnityEditor;

[CustomEditor(typeof(Game))]
public class GameEditor : Editor
{

    void OnSceneGUI()
    {
        Game game = target as Game;
        if(game == null || game.numPlayers > 1)
            return;

        game.name = "Puzzle" + game.order;
    }
}
