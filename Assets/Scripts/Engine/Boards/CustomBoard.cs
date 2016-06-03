using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomBoard : Board
{

    [HexBuilder]
    [SerializeField]
    HexListWrapper HexListWrapper;

    public override List<Hex> Hexes
    { get { 
        return HexListWrapper.Hexes; 
    } }

    [HexBuilder]
    [SerializeField]
    HexListWrapper legalStartingHexesWrapperP1; 
    public override List<Hex> LegalStartingHexesP1
    { get { 
        return legalStartingHexesWrapperP1.Hexes; 
    } }

    [HexBuilder]
    [SerializeField]
    HexListWrapper legalStartingHexesWrapperP2;
    public override List<Hex> LegalStartingHexesP2
    { get {  
            
        return legalStartingHexesWrapperP2.Hexes; 
    } }



    void Awake()
    {
        Debug.Log("Awake");
        string assetPath = "Assets/Prefabs/Boards/AssetDB/" + name.Replace("(Clone)", "");
        legalStartingHexesWrapperP2 = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "HexListWrappersP2.asset");
        legalStartingHexesWrapperP1 = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "HexListWrappersP1.asset");
        HexListWrapper = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "HexListWrappers.asset");

        if (legalStartingHexesWrapperP2 == null)
        {
            legalStartingHexesWrapperP2 = ScriptableObject.CreateInstance<HexListWrapper>();
            AssetDatabase.CreateAsset(legalStartingHexesWrapperP2, assetPath + "HexListWrappersP2.asset");
        }
        if (legalStartingHexesWrapperP1 == null)
        {
            legalStartingHexesWrapperP1 = ScriptableObject.CreateInstance<HexListWrapper>();
            AssetDatabase.CreateAsset(legalStartingHexesWrapperP1, assetPath + "HexListWrappersP1.asset");
        }
        if (HexListWrapper == null)
        {
            HexListWrapper = ScriptableObject.CreateInstance<HexListWrapper>();
            AssetDatabase.CreateAsset(HexListWrapper, assetPath + "HexListWrappers.asset");
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    protected override void BuildBoard()
    {
        name = "CustomBoard";

        gameHexes = new List<GameHex>();

        GameHex newHex;
        foreach (Hex hex in HexListWrapper.Hexes)
        {
            OffsetCoord coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);


            newHex = ObjectFactory.GameHex(globalLayout);
            newHex.transform.parent = transform;
            gameHexes.Add(newHex);
            Destroy(newHex.GetComponent<Collider>());
                
            newHex.SetPosition(globalLayout, hex);
            foreach (MeshRenderer corner in newHex.corners)
                corner.gameObject.SetActive(true);
            
			
		}
	}
}


