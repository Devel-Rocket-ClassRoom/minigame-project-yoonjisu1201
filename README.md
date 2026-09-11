# 별빛푸드트럭 (Starlight Food Truck)

> 제한된 영업 시간 동안 손님의 주문을 처리하고, 푸드트럭을 성장시키는 캐주얼 타이쿤 게임

<p align="center">
  <img src="README.assets/title.png" alt="별빛푸드트럭 타이틀 화면" width="100%">
</p>

## 게임 소개

**별빛푸드트럭**은 손님의 주문을 확인하고 재료를 조합해 음식을 만든 뒤 직접 서빙하는 캐주얼 타이쿤 게임입니다.

한 번의 영업은 기본 **120초** 동안 진행되며, 영업을 반복해 골드와 경험치를 획득하고 새로운 레시피와 손님을 해금합니다. 특정 손님에게서 얻는 유물을 수집하면 영업에 도움이 되는 패시브 능력도 해금할 수 있습니다.

<p align="center">
  <img src="README.assets/gameplay.png" alt="별빛푸드트럭 플레이 화면" width="100%">
</p>

## 플레이 흐름

### 1. 주문 확인

손님이 등장하면 주문과 인내심 게이지가 표시됩니다. 손님마다 인내심 성향이 다르며 일부 손님은 자신의 전용 메뉴를 주문합니다.

<p align="center">
  <img src="README.assets/order.png" alt="주문 화면" width="80%">
</p>

### 2. 재료 선택과 조리

필요한 재료를 조리대에 넣어 음식을 완성합니다. 잘못된 조합은 실패 음식으로 처리되고, 완성된 음식도 오래 방치하면 상합니다.

<p align="center">
  <img src="README.assets/cooking.gif" alt="조리 과정" width="70%">
</p>

### 3. 음식 서빙

완성된 음식을 손님에게 전달하면 골드와 경험치를 획득합니다. 특정 손님은 일정 확률로 유물을 드롭합니다.

<p align="center">
  <img src="README.assets/serve.gif" alt="음식 서빙" width="80%">
</p>

### 4. 푸드트럭 성장

획득한 골드로 푸드트럭을 업그레이드하고, 경험치를 모아 랭크를 올리면 새로운 콘텐츠가 해금됩니다.

<p align="center">
  <img src="README.assets/upgrade.png" alt="푸드트럭 업그레이드" width="80%">
</p>

## 주요 구현 기능

### 재료 조합 기반 조리 시스템

투입된 재료의 종류와 개수를 비교해 레시피를 판별하고, 일반 메뉴와 손님별 전용 메뉴를 분기 처리했습니다. 조리대는 `Empty → Filling → Cooking → Ready → Spoiled` 상태로 관리됩니다.

**관련 코드**  
`CookingSlot.cs` · `RecipeValidator.cs` · `DraggableFood.cs`

### 손님 주문 및 인내심 시스템

손님은 `Entering → Ordering → Exiting` 상태를 가지며, 인내심 타입에 따라 대기 시간이 달라집니다. 남은 인내심에 따라 표정이 변하고 일부 손님은 자신의 전용 메뉴를 주문합니다.

**관련 코드**  
`GuestController.cs` · `GuestSpawner.cs` · `OrderPopup.cs`

### 랭크 및 콘텐츠 해금

서빙 성공으로 경험치를 획득하고 누적 경험치에 따라 랭크가 상승합니다. 랭크 상승 시 손님과 레시피가 해금되며, 레시피에 필요한 재료도 함께 해금됩니다.

**관련 코드**  
`TruckRankManager.cs` · `UnlockManager.cs`

### 유물 수집 및 패시브 능력

특정 손님 서빙 성공 시 기본 **15% 확률**로 유물을 획득합니다. 최초 획득 시 도감에 등록되고, 같은 유물을 5개 모으면 패시브 능력이 해금되며 이후 중복 유물은 100 Gold로 변환됩니다.

**관련 코드**  
`UnlockManager.cs` · `ArtifactEffectApplier.cs` · `ArtifactSO.cs`

### 푸드트럭 업그레이드

골드를 사용해 **조리대 확장, 조리 속도 단축, 조리대 재료 현황, 주문서 힌트, 레시피 즐겨찾기**를 단계적으로 강화할 수 있습니다.

**관련 코드**  
`UpgradeManager.cs` · `CookingSlotManager.cs` · `PreparedRecipeManager.cs`

### 로컬 + Firebase 저장 동기화

진행 데이터를 로컬과 **Firebase Realtime Database**에 함께 저장합니다. 양쪽의 저장 시각을 비교해 최신 데이터를 선택하고 오래된 쪽을 동기화하도록 구현했습니다. Firebase 초기화, 인증, 저장/불러오기에는 `UniTask`를 사용했습니다.

**관련 코드**  
`SaveManager.cs` · `SaveData.cs` · `AuthManager.cs` · `FirebaseInitializer.cs`

## 기타 구현

- 첫 영업 단계형 튜토리얼
- CSV 기반 텍스트 데이터 관리
- ScriptableObject 기반 레시피 / 손님 / 유물 / 밸런스 데이터 관리
- 영업 세션 및 결과 정산
- 도감 UI
- Audio / Pause / Settings

## 기술 스택

| 분류 | 사용 기술 |
| --- | --- |
| Engine | Unity |
| Language | C# |
| Backend | Firebase Authentication, Firebase Realtime Database |
| Async | UniTask |
| Gameplay Flow | Coroutine |
| Data | ScriptableObject, CSV |
| Version Control | Git, GitHub |

## 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 프로젝트명 | 별빛푸드트럭 |
| 영문명 | Starlight Food Truck |
| 장르 | 캐주얼 타이쿤 |
| 개발 형태 | 개인 프로젝트 |
| 플랫폼 | Mobile |
| 담당 | 기획 / 프로그래밍 / UI 구성 / 데이터 설계 |
| 버전 | 1.0.0 |

## 프로젝트 구조

```text
Assets/Scripts
├── Core
├── Cooking
├── Customer
├── Progression
├── Guide
├── Data
└── UI
```

게임의 주요 시스템 코드는 `Assets/Scripts`에서 확인할 수 있습니다.

## 실행 방법

```bash
git clone https://github.com/Devel-Rocket-ClassRoom/minigame-project-yoonjisu1201.git
```

1. Unity Hub에서 프로젝트 폴더를 추가합니다.
2. 프로젝트에서 사용한 Unity Editor 버전으로 실행합니다.
3. 패키지 임포트가 완료되면 `Title` 씬부터 실행합니다.
