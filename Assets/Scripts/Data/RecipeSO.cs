using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Scriptable Objects/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    //id, 메뉴이름, 공통재료, 조리시간, 판매금액, 해금레벨, 전용메뉴인지
    // 전용 유령, 마지막 재료(일반, 특수)
    public string id;
    public List<IngredientSO> basicIngredients;
    public Sprite icon;
    public Sprite specialIcon;
    public float cookTime;
    public int sellPrice;
    public int unlockRank;
    public bool isSignatureMenu;
    public GhostSO ownerGhost;
    public IngredientSO special_Ingredient;
    public IngredientSO normalLast_Ing;
    public string dialogueID; //임시

}
