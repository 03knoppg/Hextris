using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer (typeof(HexBuilder))]
public class HexDrawer : PropertyDrawer {

    float height;
    HexListWrapper hlw;
    string assetPath;
    SerializedProperty property;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.property = property;
        hlw = (HexListWrapper)property.objectReferenceValue;
        assetPath = "Assets/Prefabs/AssetDB/" + property.serializedObject.targetObject.name.Replace("(Clone)", "") + property.name + ".asset";
            
        if (hlw == null)
        {
            hlw = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath);

            if (hlw == null)
            {
                hlw = ObjectFactory.HexListWrapper();
                property.objectReferenceValue = hlw;
                AssetDatabase.CreateAsset(hlw, assetPath);

                EditorUtility.SetDirty(hlw);
                AssetDatabase.SaveAssets();
            }
            property.objectReferenceValue = hlw;
            return;
        }

        if (hlw == null || hlw.Hexes == null) { 
            EditorGUI.LabelField(position, "HexListWrapper is null");
            height = 15;
            return;
        }
 
        Layout layout = new Layout(Layout.pointy, new Point(10, 10), new Point(0, 0));
        Vector2 offset = new Vector2(position.x, position.y + 30);

        EditorGUI.LabelField(new Rect(position.x + 150, position.y, position.width - 50, 15), property.name);
        

        hlw.boardSize = EditorGUI.DelayedIntField(new Rect(position.x, position.y + 15, 50, 15), hlw.boardSize);

        for (int col = 0; col < hlw.boardSize; col++)
        {
            for (int row = 0; row < hlw.boardSize; row++)
            {
                Hex hex = OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(col, row));
                Point point = Layout.HexToPixel(layout, hex);

                //need to display flipped image so orientation matches game
                Rect pos = new Rect(new Vector2(offset.x + point.x, offset.y + (hlw.boardSize * 15) - (point.y + 15)), new Vector2(15, 15));
                bool contains = hlw.Hexes.Contains(hex);
                if (EditorGUI.Toggle(pos, contains))
                {
                    if (!contains)
                    {
                        hlw.Hexes.Add(hex);
                        Save();
                    }

                }
                else
                {
                    hlw.Hexes.Remove(hex);
                    if (contains)
                        Save();
                }
            }
        }

        height = hlw.boardSize * 15 + 15 + 15;
    }

    void Save()
    {
        if (AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath) == null)
        {
            AssetDatabase.CreateAsset(hlw, assetPath);
        }
        property.objectReferenceValue = hlw;
        EditorUtility.SetDirty(hlw);
        AssetDatabase.SaveAssets();

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return height;
    }

}
