using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomBoard : Board
{

    [HexBuilder]
    [SerializeField]
    HexListWrapper HexListWrapper;

    public override List<GameHex> Hexes
    { get {
        return HexListWrapper.GameHexes ?? (HexListWrapper.GameHexes = new List<GameHex>()); 
    } }

    [HexBuilder]
    [SerializeField]
    HexListWrapper legalStartingHexesWrapperP1;
    public override List<GameHex> LegalStartingHexesP1
    { get {
        return legalStartingHexesWrapperP1.GameHexes ?? (legalStartingHexesWrapperP1.GameHexes = new List<GameHex>()); 
    } }

    [HexBuilder]
    [SerializeField]
    HexListWrapper legalStartingHexesWrapperP2;
    public override List<GameHex> LegalStartingHexesP2
    { get {

        return legalStartingHexesWrapperP2.GameHexes ?? (legalStartingHexesWrapperP2.GameHexes = new List<GameHex>()); 
    } }



    void Awake()
    {
        string assetPath = "Assets/Prefabs/AssetDB/" + name.Replace("(Clone)", "");
        legalStartingHexesWrapperP2 = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "legalStartingHexesWrapperP2.asset");
        legalStartingHexesWrapperP1 = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "legalStartingHexesWrapperP1.asset");
        HexListWrapper = AssetDatabase.LoadAssetAtPath<HexListWrapper>(assetPath + "HexListWrapper.asset");

        if (legalStartingHexesWrapperP2 == null)
        {
            legalStartingHexesWrapperP2 = ObjectFactory.HexListWrapper();
            AssetDatabase.CreateAsset(legalStartingHexesWrapperP2, assetPath + "legalStartingHexesWrapperP2.asset");
        }
        if (legalStartingHexesWrapperP1 == null)
        {
            legalStartingHexesWrapperP1 = ObjectFactory.HexListWrapper();
            AssetDatabase.CreateAsset(legalStartingHexesWrapperP1, assetPath + "legalStartingHexesWrapperP1.asset");
        }
        if (HexListWrapper == null)
        {
            HexListWrapper = ObjectFactory.HexListWrapper();
            AssetDatabase.CreateAsset(HexListWrapper, assetPath + "HexListWrapper.asset");
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    protected override void BuildBoard()
    {
        name = "CustomBoard";
        
        GameHex newHex;
        foreach (Hex hex in HexListWrapper.Hexes)
        {
            //OffsetCoord coord = OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, hex);


            newHex = ObjectFactory.GameHex(globalLayout);
            newHex.transform.parent = transform;
            Hexes.Add(newHex);
            Destroy(newHex.GetComponent<Collider>());
                
            newHex.SetPosition(globalLayout, hex);
            foreach (MeshRenderer corner in newHex.corners)
                corner.gameObject.SetActive(true);
		}
	}

    public override void HighlightPlayer(int playerIndex)
    {
        List<GameHex> highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
        List<Hex> highlightHexes = playerIndex == 0 ? legalStartingHexesWrapperP1.Hexes : legalStartingHexesWrapperP2.Hexes;
        foreach (GameHex gHex in Hexes)
        {
            if (highlightGameHexes.Contains(gHex) || highlightHexes.Contains(gHex.hex))
                gHex.SetColourOuter(highlight);
            else
                gHex.SetColourOuter(outer);

        }
    }

    //public override bool InStartingArea(GameHex gHex, int playerIndex)
    //{
    //    List<GameHex> highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
    //    List<Hex> highlightHexes = playerIndex == 0 ? legalStartingHexesWrapperP1.Hexes : legalStartingHexesWrapperP2.Hexes;
       
    //    return highlightGameHexes.Contains(gHex) || highlightHexes.Contains(gHex.hex);
    //}
    public override bool InStartingArea(Hex hex, int playerIndex)
    {
        //this doesnt work because hex is in a differnet local layout
        List<GameHex> highlightGameHexes = playerIndex == 0 ? LegalStartingHexesP1 : LegalStartingHexesP2;
        List<Hex> highlightHexes = playerIndex == 0 ? legalStartingHexesWrapperP1.Hexes : legalStartingHexesWrapperP2.Hexes;

        foreach (GameHex highlightGameHex in highlightGameHexes)
        {
            if (highlightGameHex.hex == hex)
                return true;
        }

        foreach (Hex highlightHex in highlightHexes)
        {
            if (highlightHex == hex)
                return true;
        }
        return false;
    }
}


