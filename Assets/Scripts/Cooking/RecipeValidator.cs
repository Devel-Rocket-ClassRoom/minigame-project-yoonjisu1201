using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RecipeValidator
{
    // 순서 무관, 재료 종류/개수만 일치하면 true
    private static bool IngredientsMatch(List<IngredientSO> a, List<IngredientSO> b)
    {
        if (a.Count != b.Count) return false;
        return a.OrderBy(i => i.id).SequenceEqual(b.OrderBy(i => i.id));
    }
    public static RecipeSO FindMatchingRecipe(
        List<IngredientSO> slotIngredients,
        List<RecipeSO> availableRecipe)
    {
        foreach (var recipe in availableRecipe)
        {
            if (MatchesRecipe(slotIngredients, recipe))
                return recipe;
        }
        return null;
    }
    private static bool MatchesRecipe(List<IngredientSO> slotIngredients, RecipeSO recipe)
    {
        //기본&일반 메뉴
        if (!recipe.isSignatureMenu)
            return IngredientsMatch(slotIngredients, recipe.basicIngredients);

        //전용 메뉴 (리스트 복사하는 이유는 basicIngredients가 참조로 변하지 않도록 하기위함)
        var expectedNormal = new List<IngredientSO>(recipe.basicIngredients) { recipe.normalLast_Ing };
        var expectedSpecial = new List<IngredientSO>(recipe.basicIngredients) { recipe.special_Ingredient };

        return IngredientsMatch(slotIngredients, expectedNormal) || IngredientsMatch(slotIngredients, expectedSpecial);
    }
    //Case 1 - 손님 서빙 시 - 주문한 메뉴와 일치하는지
    public static bool ValidateForGuest(
        List<IngredientSO> slotIngredients,
        RecipeSO recipe,
        GhostSO currentGuest)
    {
        //일반메뉴
        if (!recipe.isSignatureMenu)
            return IngredientsMatch(slotIngredients, recipe.basicIngredients);

        //전용메뉴
        IngredientSO lastIngredient = recipe.ownerGhost == currentGuest
           ? recipe.special_Ingredient
           : recipe.normalLast_Ing;

        var expected = new List<IngredientSO>(recipe.basicIngredients) { lastIngredient };
        return IngredientsMatch(slotIngredients, expected);
    }

}
