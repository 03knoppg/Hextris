using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PieceColourPallet : ScriptableObject {

    
    [SerializeField]
    Material outerInactive;
    [SerializeField]
    Material outerPivot;
    [SerializeField]
    Material outerSelected;
    [SerializeField]
    public Material innerActive;
    [SerializeField]
    public Material innerPivot;
    [SerializeField]
    public Material innerDisabled;

    [SerializeField]
    int playerNum;
    

    static List<PieceColourPallet> PalletDict;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        PalletDict = Resources.LoadAll<PieceColourPallet>("AssetFiles/PieceColorPallets").OrderBy(pallet => pallet.playerNum).ToList();
    }

    public static Material OuterInactive(int playerNum)
    {
        return PalletDict[playerNum].outerInactive;
    }
    public static Material OuterPivot(int playerNum)
    {
        return PalletDict[playerNum].outerPivot;
    }
    public static Material OuterSelected(int playerNum)
    {
        return PalletDict[playerNum].outerSelected;
    }
    public static Material InnerActive(int playerNum)
    {
        return PalletDict[playerNum].innerActive;
    }
    public static Material InnerPivot(int playerNum)
    {
        return PalletDict[playerNum].innerPivot;
    }
    public static Material InnerDisabled(int playerNum)
    {
        return PalletDict[playerNum].innerDisabled;
    }

}
