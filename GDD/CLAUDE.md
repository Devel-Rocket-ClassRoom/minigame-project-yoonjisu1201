# 별빛 푸드트럭 — 개발 가이드

Unity 6.3 모바일 2D 감성 힐링 요리 타이쿤. 1인 개발, 평일 15일(3주) 일정.

> 게임 시스템·진행 규칙·콘텐츠는 **GDD.md** 참조. 이 문서는 **코드 작성 가이드**입니다.

## 작성자 컨텍스트

- 프로그래밍 학습 3개월차. 코드 의뢰 시 **초보자가 이해할 수 있는 수준**으로 설명 포함.
- 모르는 개념은 추측 말고 짚어줄 것 (예: "여기서 LINQ의 SequenceEqual을 쓰는데, 이건 ~").
- 한국어로 응답할 것.

---

## 1. 프로젝트 핵심 결정사항

| 항목 | 결정 | 이유 |
|---|---|---|
| 엔진 | Unity 6.3 | 확정 |
| 플랫폼 | Mobile (iOS/Android), 가로 화면 | 확정 |
| 입력 | 탭 중심 + 완성 음식만 드래그 | 한 손/두 손 호환 |
| 조리 슬롯 자료구조 | `List<IngredientSO>` (Stack 아님) | 재료 투입 순서가 검증 핵심 |
| 레시피 검증 | `SequenceEqual` 일괄 비교 | 단순·직관적 |
| 페일 정책 | Soft Fail only | 감성 힐링 톤 유지 |
| 재료 재고 | 무한 | 관리 부담 제거 |
| 저장 | JsonUtility + File I/O | 외부 라이브러리 X |
| 대화 시스템 | **v1.1로 이연** (필드만 선반영) | 1차 빌드 완주 우선 |

---

## 2. 폴더 구조

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/          # GameManager, GameContext, SaveSystem, EventBus
│   │   ├── Cooking/       # CookingSlot, RecipeValidator
│   │   ├── Customer/      # GhostSpawner, PatienceGauge
│   │   ├── Progression/   # TruckRank, ArtifactSystem
│   │   ├── UI/            # 모든 UI 컴포넌트
│   │   └── Data/          # ScriptableObject 정의
│   ├── ScriptableObjects/ # SO 인스턴스 에셋
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Art/
│   └── Audio/
```

신규 스크립트는 위 폴더 구조를 따를 것. 임의로 새 폴더 만들지 말 것.

---

## 3. 네이밍 컨벤션

- 클래스 / 메서드: `PascalCase`
- 변수 / 파라미터: `camelCase`
- private 필드: `_camelCase` (언더스코어 prefix)
- 상수: `UPPER_SNAKE_CASE`
- ScriptableObject 클래스: `XxxSO` 접미사 (예: `RecipeSO`, `GhostSO`)
- SO 에셋 파일: `SO_Recipe_Tomato.asset` 형태
- 이벤트: `OnXxx` (예: `OnGuestSatisfied`, `OnRankUp`)

---

## 4. ScriptableObject 스키마

신규 필드 추가 시 기존 SO와 일관성 유지. v1.1 대화 시스템 대비 `dialogueID` 필드는 미리 포함.

```csharp
// IngredientCategory: 재료 분류 (재료 패널 탭 분리 + 특수재료 식별 겸용)
public enum IngredientCategory {
    Base,    // 베이스 (빵, 반죽, 우유 등)
    Fresh,   // 과일/신선 (딸기, 바나나, 양상추 등)
    Sauce,   // 소스 (초코시럽, 꿀, 잼 등)
    Special  // 특수재료 (단골 시그니처 — 영혼모닥불, 서류더미 설탕 등)
}

// IngredientSO: 재료 1종
public class IngredientSO : ScriptableObject {
    public string id;
    public string displayName;
    public Sprite icon;
    public IngredientCategory category;  // 탭 분리 + 검증 분기 겸용 (Special == 특수재료)
}

// RecipeSO: 레시피 1종
public class RecipeSO : ScriptableObject {
    public string id;
    public string displayName;
    public List<IngredientSO> baseIngredients;       // 공통 재료 (마지막 재료 제외), 순서 중요
    public float cookTime;
    public int sellPrice;
    public int unlockRank;                           // 이 등급에서 해금
    public bool isSignatureMenu;                     // 전용 메뉴 여부
    public GhostSO ownerGhost;                       // 전용 메뉴가 아니면 null
    public IngredientSO requiredSpecialIngredient;   // 단골 본인 주문 시 정답 마지막 재료 (Special 카테고리)
    public IngredientSO normalCounterpart;           // 다른 손님 주문 시 정답 마지막 재료 (일반 카테고리)
    public string dialogueID;                        // v1.1 대비 (현재 미사용)
}

// GhostSO: 유령 손님 1종
public class GhostSO : ScriptableObject {
    public string id;
    public string displayName;
    public Sprite portrait;
    public List<RecipeSO> preferredOrders;
    public float patienceSeconds;
    public int unlockRank;        // 이 등급에서 해금 (0 = 시작부터)
    public ArtifactSO artifact;   // 고유 유물
    public string firstMeetDialogueID;     // v1.1 대비
    public string[] memoirDialogueIDs;     // v1.1 대비
}

// ArtifactSO: 유물 1종
public class ArtifactSO : ScriptableObject {
    public string id;
    public string displayName;
    public Sprite icon;
    public string memoirText;      // 1개 획득 시 해금되는 방명록
    public string functionalDesc;  // 5개 누적 시 적용 (효과는 코드에서 분기)
}

// DialogueSO: v1.1 대비 빈 껍데기
public class DialogueSO : ScriptableObject {
    public string id;
    public string speakerName;
    public string[] lines;
}
```

---

## 5. 코드 패턴

> 시스템 규칙(예: 유물 5개 = 보상 활성화)은 **GDD.md** 참조. 여기서는 **그 규칙을 어떻게 코드로 구현하는지**의 표준 패턴만 명시.

### 5.1 조리 슬롯 상태 머신

```csharp
public enum CookingSlotState {
    Empty,     // 빈 슬롯
    Filling,   // 재료 투입 중
    Cooking,   // 타이머 가동 중
    Ready,     // 수령 가능
    Spoiled    // 방치되어 상함
}
```

- 활성 슬롯은 한 번에 하나. 슬롯 탭 = 활성 전환.
- 레시피 검증은 **수령 시점**에 일괄 판정. 조리 중 비교 금지.
  1. 일반 메뉴(`!isSignatureMenu`)이면 `slotIngredients`를 `recipe.baseIngredients`와 그대로 `SequenceEqual` 비교
  2. 전용 메뉴이면 마지막 재료 분기:
     - `recipe.ownerGhost == currentGuest` → 정답 = `baseIngredients + [requiredSpecialIngredient]`
     - 그 외 → 정답 = `baseIngredients + [normalCounterpart]`
  3. 슬롯 재료가 위 정답 리스트와 `SequenceEqual` 일치 시 성공, 아니면 망한 음식 (Soft Fail)
- 조리 타이머는 코루틴.

**표준 검증 함수 시그니처** (Day 5에 구현):

```csharp
// Cooking/RecipeValidator.cs
public static bool ValidateRecipe(
    List<IngredientSO> slotIngredients,
    RecipeSO recipe,
    GhostSO currentGuest)
{
    // 일반 메뉴: baseIngredients 그대로 비교
    if (!recipe.isSignatureMenu) {
        return slotIngredients.SequenceEqual(recipe.baseIngredients);
    }

    // 전용 메뉴: 손님이 ownerGhost인지에 따라 마지막 재료 결정
    IngredientSO lastIngredient = (recipe.ownerGhost == currentGuest)
        ? recipe.requiredSpecialIngredient
        : recipe.normalCounterpart;

    // baseIngredients + [lastIngredient]가 정답
    var expected = new List<IngredientSO>(recipe.baseIngredients) { lastIngredient };
    return slotIngredients.SequenceEqual(expected);
}
```

> 💡 `SequenceEqual`은 LINQ 메서드로, 두 시퀀스의 **개수와 순서가 모두 같은지** 비교합니다. `using System.Linq;` 필요.

### 5.2 유물 드롭 표준 처리 패턴

```csharp
// 이 패턴 유지할 것. 1개째/5개째/초과 분기 순서가 중요.
void OnArtifactDrop(ArtifactSO artifact) {
    int count = _artifactCounts[artifact.id];
    if (count >= 5) {
        _gold += 100;  // 잉여 보상
        return;
    }
    _artifactCounts[artifact.id] = count + 1;
    if (count == 0) UnlockMemoir(artifact);          // 1개째: 방명록 해금
    if (count + 1 == 5) ActivateBonus(artifact);     // 5개째: 기능 보상 활성화
}
```

### 5.3 GameContext — 중앙 효과 변수 (Day 8~10에 사전 도입)

**유물 효과가 영업/조리 시스템에 영향을 주는 값들을 중앙 변수로 분리**.
이렇게 하면 Day 12에 유물 효과 적용이 "변수 값 수정 한 줄"로 끝남.

```csharp
// Core/GameContext.cs — 정적 클래스 또는 싱글톤
public static class GameContext {
    // 손님 관련 (Day 8에 미리 도입)
    public static float customerSpawnInterval = 5f;       // 유물 #5 대비
    public static float customerPatienceMultiplier = 1f;  // 유물 #1 대비
    public static float customerRetainChance = 0f;        // 유물 #8 대비

    // 골드/팁 관련 (Day 9에 미리 도입)
    public static float foodPriceMultiplier = 1f;         // 유물 #10 대비
    public static float tipChance = 0f;                   // 유물 #2 대비
    public static float artifactDropChanceBonus = 0f;     // 유물 #9 대비

    // 영업/UI 관련 (Day 10에 미리 도입)
    public static int sessionStartBonusGold = 0;          // 유물 #7 대비
    public static float ingredientButtonAnimSpeed = 1f;   // 유물 #3 대비
}
```

**규칙**: 손님 스폰, 골드 계산, 영업 시작 처리 등에서 **이 값들을 직접 하드코딩하지 말고 GameContext에서 읽어올 것.**

```csharp
// 좋은 예: GameContext 경유
int finalGold = (int)(recipe.sellPrice * GameContext.foodPriceMultiplier);

// 나쁜 예: 하드코딩
int finalGold = recipe.sellPrice * 1;
```

Day 12에 유물 5개 보상 활성화는 이렇게 끝남:

```csharp
void ActivateBonus(ArtifactSO artifact) {
    switch (artifact.id) {
        case "compass":  GameContext.customerPatienceMultiplier += 0.05f; break;
        case "marble":   GameContext.tipChance += 0.10f; break;
        case "thimble":  GameContext.foodPriceMultiplier += 0.15f; break;
        // ...
    }
}
```

### 5.4 트럭 등급 임계값 (매직 넘버 금지)

```csharp
// Core/RankThresholds.cs — 상수로 분리
public static class RankThresholds {
    public static readonly int[] EXP_REQUIRED = {
        0, 5, 12, 20, 30, 42, 55, 70, 85, 100, 120
    };  // 인덱스 = 등급-1
}
```

GDD에서 곡선 조정 결정이 나면 이 배열만 수정.

---

## 6. 코드 작성 규칙

- **컴포넌트 결합도 낮추기**: 시스템 간 통신은 C# event 또는 UnityEvent로. 직접 참조 최소화.
- **MonoBehaviour는 얇게**: 로직은 일반 C# 클래스나 SO에. MonoBehaviour는 Unity 연결부만.
- **코루틴 우선**: 타이머·지연 연출은 코루틴. `Update()` 안에서 시간 누적 지양.
- **매직 넘버 금지**: 등급 임계값, 유물 5개, 골드 100 같은 값은 SO나 상수로 분리.
- **GameContext 경유**: 유물 영향 받는 값(스폰 간격, 가격 배율 등)은 GameContext에서 읽기.
- **주석**: 자료구조 선택 이유 등 "왜"를 주석으로 (코드가 설명하는 "무엇"은 생략).
- **Public 최소화**: 직렬화 필요 없는 필드는 `[SerializeField] private`.

---

## 7. 모바일 최적화 기본

- Sprite Atlas 활용 (드로우콜 절감).
- UI는 Canvas 분리 (정적/동적/팝업).
- `Instantiate` 남발 금지. 손님 스폰은 오브젝트 풀 권장 (Day 8에 도입).
- 빌드 타깃: Android 최소 API 24, iOS 13+.

---

## 8. 개발 일정 (15일 = 3주 로드맵)

평일 8시간 작업 기준. 실 코딩 4~5시간 + 학습/디버깅 3~4시간 가정.

### 8.1 주차별 큰 흐름

| 주차 | 기간 | 핵심 목표 | 주말 체크포인트 |
|---|---|---|---|
| **1주차** | Day 1~5 | 데이터 구조 + 단일 슬롯 조리 검증 | 한 슬롯에서 조리 → 완성/실패 판정 작동 |
| **2주차** | Day 6~10 | 멀티슬롯 + 영업 루프 + **GameContext 훅 사전 심기** | 2~3분 세션 1회 완주 + 모든 효과 변수가 GameContext 경유 |
| **3주차** | Day 11~15 | 메타 진행(등급·유물·도감) + 저장/엔딩/빌드 | 등급 상승 → 해금 → 유물 → 엔딩까지 풀 사이클 |

**핵심 전략**: v1.7에서 신설된 유물 시스템(10종 효과)의 부담을 줄이기 위해, **2주차에 GameContext 중앙 변수를 미리 심어둠**. 그러면 3주차 Day 12에 유물 효과 적용이 변수 값 수정만으로 끝남.

### 8.2 페이즈 구성

| 페이즈 | 기간 | 목표 | 산출물 |
|---|---|---|---|
| P1. 기반 구축 | Day 1~3 | Unity 프로젝트 셋업, SO 데이터 구조, 기본 화면 전환 | 재료/레시피/손님 SO, 씬 골격 |
| P2. 조리 시스템 | Day 4~7 | List 기반 슬롯 조리, 활성 슬롯 전환, 타이머, 검증, 망한 음식 처리 | 플레이 가능한 조리 프로토타입 |
| P3. 영업 루프 + 훅 심기 | Day 8~10 | 손님 스폰/인내심/주문, 드래그 전달, 골드 수급, 세션 타이머 + **GameContext 도입** | 한 사이클 완주 가능한 영업 모드 |
| P4. 수집·진행 | Day 11~12 | 도감 UI, 트럭 등급 시스템, 레시피·유령 해금, 유물 5개 누적 로직 | 메타 진행 가능 |
| P5. 마무리 | Day 13~15 | 저장/로드, 엔딩 연출, 모바일 빌드, 버그 수정 + 예비일 | APK/IPA 빌드 |

### 8.3 일별 상세 계획

| Day | 작업 내용 | 학습 포인트 |
|---|---|---|
| **1주차** | | |
| Day 1 | Unity 6.3 프로젝트 생성, 폴더 구조 정리, Git 셋업, 메인 씬 골격(영업/도감/업그레이드) | Unity 프로젝트 구조, Scene 관리 |
| Day 2 | SO 정의: IngredientSO(+`IngredientCategory` enum), RecipeSO(+`normalCounterpart` 필드), GhostSO, ArtifactSO, DialogueSO(빈 껍데기). GhostSO/RecipeSO에 dialogueID 필드 미리 추가. 더미 데이터 3~5개씩 입력 (재료는 카테고리별 안배) | SO 설계, enum, Asset 생성 워크플로우 |
| Day 3 | 재료 패널 UI (**카테고리 탭 분리: Base/Fresh/Sauce/Special**), 슬롯 UI 프리팹, 활성 슬롯 전환 로직, 슬롯 강조 표현 | UI Canvas, Button 이벤트, 탭 패널 전환, 상태 관리 |
| Day 4 | CookingSlot 클래스 — 재료 List push, '조리 시작' → 코루틴 타이머, 슬롯 상태머신(Empty/Filling/Cooking/Ready/Spoiled) | List\<T\>, Coroutine, enum 상태 |
| Day 5 | 레시피 검증 로직 (SequenceEqual), 완성/실패 분기, 망한 음식 팝업 UI, 통통 튀는 재료 연출 | LINQ, UI 팝업, Animator/Tween |
| **2주차** | | |
| Day 6 | 멀티슬롯 독립 동작 확인, 활성 슬롯 전환 시 UI 동기화, 조리 중도 취소 기능 | 다중 인스턴스, 이벤트 시스템 |
| Day 7 | 조리 시스템 통합 테스트, 버그 수정, 1차 영상 캡처 — '조리 프로토타입 완료' | 디버깅, 리팩터링 |
| Day 8 | 손님 스폰 시스템 (큐), 손님 프리팹, 주문 말풍선, 인내심 게이지 UI. **GameContext 도입 + 손님 관련 변수 3종(spawnInterval/patienceMultiplier/retainChance) 분리** | Queue\<T\>, Instantiate, Slider UI, 정적 클래스 |
| Day 9 | 드래그 전달 — 완성 음식 → 손님 매칭, 잘못된 손님 시 슬롯 복귀, 골드 획득. **GameContext에 골드/팁 변수 3종(foodPriceMultiplier/tipChance/dropChanceBonus) 추가**. 골드 계산을 변수 경유로 작성 | Drag & Drop, 매칭 로직 |
| Day 10 | 세션 타이머(2~3분), 영업 마감 연출, 인내심 소진 시 손님 이탈, 음식 재활용. **GameContext에 영업/UI 변수 2종(sessionStartBonusGold/ingredientButtonAnimSpeed) 추가** | 전역 타이머, 정비 화면 전환 |
| **3주차** | | |
| Day 11 | 트럭 등급 시스템 (EXP 누적, 등급 상승 시 SO 풀에서 유령·레시피 해금), 도감 UI (유령 카드 그리드, 잠금/해금 상태) | ScrollView, Dictionary\<,\>, 카운터 로직 |
| Day 12 | 유물 드롭(확률), 같은 유물 5개 누적 카운터, 1개째 = 방명록 해금, 5개째 = 기능 보상 활성화(GameContext 변수 수정), 초과 시 골드 변환. 업그레이드 메뉴 기본형 | Random, switch 분기, 패시브 적용 |
| Day 13 | JSON 저장/로드 (해금 레시피·유령·유물 카운트·골드·트럭 등급), 게임 재시작 시 복원 | JsonUtility, PlayerPrefs, File I/O |
| Day 14 | 엔딩 연출(할머니 편지), 사운드 1~2개 삽입, UX 폴리싱, 모바일 빌드 1차 | Android Build Settings |
| Day 15 | 버그 수정, 실기기 테스트, 빌드 최적화, 예비 시간 | 프로파일링, 빌드 사이즈 |

### 8.4 주차별 위험 신호

- **1주차 끝 (Day 5)**: 레시피 검증이 안 되면 2주차 위험. 단일 슬롯이라도 완벽히 돌아야 멀티슬롯 진입 가능.
- **2주차 끝 (Day 10)**: 영업 루프 한 사이클이 안 돌면 Day 11 등급 시스템에 EXP 공급원이 없음. 컷오프 규칙 발동 검토.
- **3주차 중반 (Day 12)**: GameContext 훅을 잘 심어뒀다면 유물 효과 적용은 10줄 안에 끝남. 여기서 밀리면 분기 처리·카운터 로직부터 점검.

---

## 9. 스코프 컷오프 (사수해야 함)

일정이 밀릴 때 잘라낼 순서. **여기 적힌 것 외에는 잘라내지 말 것.**

- Day 7까지 조리 프로토타입 미완성 → 멀티슬롯 1개로 축소
- Day 10까지 영업 루프 미완성 → 등급 11 → 5로 축소
- Day 12까지 유물 시스템 미동작 → 유물 통째로 v2.0 이연 (등급 해금만 유지)
- Day 12 종료 시점에 유물 효과 10종 중 절반 이하 작동 → **단순한 5종(인내심/팁/가격/보너스 골드/드롭률)만 1차 출시 포함**, 복잡한 것(이탈 손님 재머무름, UI 결합 효과)은 v1.1로 이연
- Day 14 빌드 실패 → 엔딩 단순화 (텍스트 노출만)

---

## 10. v1.1로 미룬 항목 (현재 작업 X)

- 대화 시스템 (DialogueSO 필드만 선반영, 실제 UI/로직 X)
- BGM/SFX 본격 적용
- 유물 효과 밸런싱
- 트럭 등급 곡선 조정

이 항목들에 대한 코드 요청 시 "v1.1로 이연된 항목"임을 알리고 작업 제외.

---

## 11. 응답 시 지켜야 할 것

- **추측 금지**: GDD에 없는 사양을 임의로 만들지 말 것. 모호하면 질문할 것.
- **GDD 우선 참조**: 게임 규칙·콘텐츠 관련 질문은 **GDD.md**에서 먼저 확인.
- **컷오프 인지**: 일정에 영향을 줄 수 있는 제안은 "이건 컷오프 대상이 될 수 있다" 명시.
- **GameContext 경유 확인**: 유물 영향 받는 값을 다루는 코드를 생성할 때, 하드코딩이 아니라 GameContext 경유로 작성됐는지 자기 점검할 것.
- **자료구조 이유 설명**: List vs Stack, Dictionary 선택 등은 학습 목적상 이유를 짧게 곁들일 것.
- **테스트 코드**: 1차 빌드까지는 작성 안 함. 시간 절약 우선.
