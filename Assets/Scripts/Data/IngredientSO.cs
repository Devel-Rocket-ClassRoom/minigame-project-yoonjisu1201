using UnityEngine;

public enum IngredientCategory
{
    Base, // 빵, 반죽 등
    Fresh, // 딸기, 바나나 등
    Sauce, // 시럽, 꿀 등
    Special // 특수재료
}
[CreateAssetMenu(fileName = "NewIngredient", menuName = "Scriptable Objects/IngredientSO")]
public class IngredientSO : ScriptableObject
{
    //id, 재료 이름, 이미지, 카테고리
    public string id;
    public string displayName;
    public Sprite icon;
    public IngredientCategory category;
}
