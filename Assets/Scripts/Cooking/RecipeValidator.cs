using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RecipeValidator
{
    //Case 1 - 조리 완성 시 - 현재 레시피에 있는 요리를 만들었는지
    public static RecipeSO FindMatchingRecipe(
        List<IngredientSO> slotIngredients,
        List<RecipeSO> availableRecipe)
    {
        foreach (var recipe in availableRecipe)
        {
            if (MatchsRecipe(slotIngredients, recipe))
            {
                return recipe;            
            }
        }
        return null;
    }
    private static bool MatchsRecipe(List<IngredientSO> slotIngredients, RecipeSO recipe)
    {
        //일반메뉴
        if (!recipe.isSignatureMenu)
        {
            //두 리스트의 개수와 순서가 모두 같을때 true
            return slotIngredients.SequenceEqual(recipe.basicIngredients);
        }
        //전용메뉴 (리스트 복사하는 이유는 basicIngredients가 참조로 변하지 않도록 하기위함)
        var expectedNormal = new List<IngredientSO>(recipe.basicIngredients);
        expectedNormal.Add(recipe.normalLast_Ing);
        var expectedSpecial = new List<IngredientSO>(recipe.basicIngredients);
        expectedSpecial.Add(recipe.special_Ingredient);

        Debug.Log($"[전용] {recipe.displayName}\n슬롯:       {string.Join(", ", slotIngredients.ConvertAll(i => i.displayName))}\n기대(일반): {string.Join(", ", expectedNormal.ConvertAll(i => i.displayName))}\n기대(스페셜): {string.Join(", ", expectedSpecial.ConvertAll(i => i.displayName))}");

        return slotIngredients.SequenceEqual(expectedNormal) || slotIngredients.SequenceEqual(expectedSpecial);
    }
    //Case 1 - 손님 서빙 시 - 주문한 메뉴와 일치하는지
    public static bool ValidateForGuest(
        List<IngredientSO> slotIngredients,
        RecipeSO recipe,
        GhostSO currentGuest)
    {
        //일반메뉴
        if (!recipe.isSignatureMenu)
            return slotIngredients.SequenceEqual(recipe.basicIngredients);

        //전용메뉴
         IngredientSO lastIngredient = recipe.ownerGhost == currentGuest ? 
            recipe.special_Ingredient : recipe.normalLast_Ing;

        var expected = new List<IngredientSO>(recipe.basicIngredients);
        expected.Add(lastIngredient);
        return slotIngredients.SequenceEqual(expected);
    }

}
