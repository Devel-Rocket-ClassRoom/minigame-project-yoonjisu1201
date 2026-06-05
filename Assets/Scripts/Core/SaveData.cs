using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // GoldManager
    public int totalGold;

    // TruckRankManager
    public int currentRank;
    public float totalExp;

    // UpgradeManager
    public int cookSlotLevel;
    public int speedUpLevel;
    public int cookBoardLevel;
    public int orderHintLevel;
    public int containerSlotLevel;

    //unlockManager → 해금된 항목들의 ID만 저장
    public List<string> unlockedRecipes = new();
    public List<string> unlockedGhosts = new();
    public List<string> unlockedIngredients = new();
    public List<string> unlockedArtifacts = new();
    public List<string> unlockedAbilities = new();

    // 유물 개수 / 저장: dict → 두 리스트  /  불러오기: 두 리스트 → dict 재조립
    public List<string> artifactCountKeys = new();
    public List<int> artifactCountValues = new();
}
