using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameHex))]
public class GameHexEditor : Editor {

    void OnSceneGUI()
    {
        Layout.defaultLayout = new Layout(Layout.pointy, new Point(1, 1), new Point(0, 0));
        if (Event.current != null)
        {
            GameHex gHex = target as GameHex;

            if (Event.current.type == EventType.MouseUp)
            {
                gHex.UpdatePosition();
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                gHex.UpdateHex(new Point(gHex.transform.parent.transform));

                gHex.coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, gHex.hex);

            }
        }
    }
}
