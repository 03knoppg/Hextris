using System.Collections.Generic;
using UnityEngine;

public enum EColorPallet
{
    Normal
}

public class BoardColourPallet : ScriptableObject {

    [SerializeField]
    EColorPallet colorPallet;

    [SerializeField]
    public Material inner;
    [SerializeField]
    public Material outer;
    [SerializeField]
    public Material highlight;
    [SerializeField]
    public Material outerHighlight;
    
    static Dictionary<EColorPallet, BoardColourPallet> PalletDict;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        PalletDict = new Dictionary<EColorPallet, BoardColourPallet>();

        foreach(BoardColourPallet pallet in Resources.LoadAll<BoardColourPallet>("AssetFiles/BoardColorPallets"))
        {
            PalletDict.Add(pallet.colorPallet, pallet);
        }
    }

    public static Material Inner(EColorPallet colorPallet)
    {
        return PalletDict[colorPallet].inner;
    }
    public static Material Outer(EColorPallet colorPallet)
    {
        return PalletDict[colorPallet].outer;
    }
    public static Material Highlight(EColorPallet colorPallet)
    {
        return PalletDict[colorPallet].highlight;
    }
    public static Material OuterHighlight(EColorPallet colorPallet)
    {
        return PalletDict[colorPallet].outerHighlight;
    }

}
