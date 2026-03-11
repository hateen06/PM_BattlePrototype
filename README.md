# PM Battle Prototype

> Project Moon 전투 흐름을 참고해 Unity로 만든 공부용 2D 턴제 전투 프로토타입
>
> 작업 기준: Day 10까지 진행한 학습 기록

## 프로젝트 목적

- 포트폴리오용 과장보다는 공부 과정과 구조 이해를 남기는 데 초점을 맞췄습니다.
- 전투 화면 구성, 행동 선택 흐름, 타겟 처리, 데이터 분리의 기초를 직접 구현하는 것을 목표로 했습니다.

## 구현 범위

### 1. 전투 흐름

- Attack / Guard / Skill 행동 선택
- 적 유닛 타겟 선택
- Start 버튼으로 플레이어 행동 실행
- 적의 반격 처리
- HP 감소, 사망 처리, 승패 판정

### 2. 코드 구조

- `BattleUIController`
  - 전투 입력, 행동 실행, 적 턴 처리, 결과 판정 담당
- `UnitHP`
  - 체력, 사망 상태, HP 텍스트 갱신 담당
- `TargetSelectable`
  - 클릭된 적을 현재 타겟으로 연결하는 역할
- `SkillData`
  - Attack / Guard / Skill 데이터 분리를 위한 `ScriptableObject`

### 3. UI 정리

- 상단 정보 패널, 전장 영역, 하단 행동 패널 분리
- 버튼 선택 상태 시각화
- 텍스트 가독성 보정
- 화면 크기에 맞춘 자동 레이아웃 정리

## AI 도움 받은 부분

- [x] 적의 반격 시스템 추가
- [x] `SkillData` 기반 기술 데이터 구조 추가
- [x] `BattleUIController` 리팩터링
- [x] 전투 UI 레이아웃 및 텍스트 가독성 개선
- [x] 타겟 선택 버그 수정

## Day 10 이후 공부 과제

- [ ] Guard를 실제 방어 계산에 반영하기
- [ ] `UnitData` 분리로 유닛 스탯 데이터 구조 확장하기
- [ ] 버프 / 디버프 / 상태이상 구조 추가하기
- [ ] 조건 기반 적 AI로 확장하기
- [ ] 로그 연출, 피격 피드백, 빌드 정리하기

## 실행 및 확인

- Unity에서 `Assets/Scenes/BattleScene.unity`를 열어 전투 화면을 확인할 수 있습니다.
- 주요 스크립트는 아래 경로에 정리되어 있습니다.
  - `Assets/Scripts/UI/BattleUIController.cs`
  - `Assets/Scripts/UI/TargetSelectable.cs`
  - `Assets/Scripts/Units/UnitHP.cs`
  - `Assets/Scripts/Skills/SkillData.cs`
