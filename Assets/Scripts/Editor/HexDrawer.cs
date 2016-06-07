using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer (typeof(HexBuilder))]
public class HexDrawer : PropertyDrawer {

    float height;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        HexListWrapper hlw = (HexListWrapper)property.objectReferenceValue;

        if (EditorGUI.ToggleLeft(new Rect(position.x + 50, position.y + 15, 45, 15), "Save", false))
        {
            string assetPath = "Assets/Prefabs/Pieces/AssetDB/" + property.serializedObject.targetObject.name.Replace("(Clone)", "");
            hlw = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "HexListWrappers.asset");

            
            if (hlw == null)
            {
                hlw = ObjectFactory.HexListWrapper();
                AssetDatabase.CreateAsset(hlw, assetPath + "HexListWrappers.asset");
            }

            EditorUtility.SetDirty(hlw);
            AssetDatabase.SaveAssets();

            //((Piece)property.serializedObject.targetObject).LoadAsset();
            hlw = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "HexListWrappers.asset");
        }

        if (hlw == null || hlw.Hexes == null) { 
            EditorGUI.LabelField(position, "HexListWrapper is null");
            height = 15;
            return;
        }
 
        //need to display flipped image for some reason so orientation matches game
        Layout layout = new Layout(Layout.pointy, new Point(10, -10), new Point(position.x, position.y + hlw.boardSize * 15));


        EditorGUI.LabelField(new Rect(position.x + 150, position.y, position.width - 50, 15), property.name);
        

        hlw.boardSize = EditorGUI.DelayedIntField(new Rect(position.x, position.y + 15, 50, 15), hlw.boardSize);

        for (int col = 0; col < hlw.boardSize; col++)
        {
            for (int row = 0; row < hlw.boardSize; row++)
            {
                Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(col, row));
                Point point = Layout.HexToPixel(layout, hex);
                Rect pos = new Rect(new Vector2(point.x, point.y + 15), new Vector2(15, 15));
                bool contains = hlw.Hexes.Contains(hex);
                if (EditorGUI.Toggle(pos, contains))
                {
                    if (!contains)
                        hlw.Hexes.Add(hex);
                }
                else
                    hlw.Hexes.Remove(hex);
            }
        }

        height = hlw.boardSize * 15 + 15 + 15;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return height;
    }

}
