# [Unity 3D] RPG Game Portfolio

<div align="center">

  <img src="https://github.com/LHuHyeon/LHuHyeon.github.io/assets/110723307/454b1924-4120-4fba-854f-b4bd74b33501" width="49%" height="230"/>
  <img src="https://github.com/LHuHyeon/LHuHyeon.github.io/assets/110723307/5e55198b-bf0b-4861-a7f1-8587381777ca" width="49%" height="230"/>
  <img src="https://github.com/LHuHyeon/LHuHyeon.github.io/assets/110723307/32d119d7-bbec-43f8-8bf9-e5a5aea8e6c5" width="49%" height="230"/>
  <img src="https://github.com/LHuHyeon/LHuHyeon.github.io/assets/110723307/b03533e5-91fb-4048-8f19-2def11c6e9d5" width="49%" height="230"/>

  < 게임 플레이 사진 >

</div>

## 1. 소개
+ Unity 3D RPG 게임입니다.

+ 게임 개발자가 되기 위해 스스로 역량을 쌓고 처음으로 제작한 RPG 포트폴리오입니다.

+ 현재 Repository에는 유료에셋 사용으로 인해 소스코드만 등록되어 있습니다.

+ 개발기간: 2023.05.10 ~ 2023.08.18 ( 약 3개월 )

+ 형상관리: Git SourceTree

## 2. 개발 환경
+ Unity 2021.3.21f1 LTS

+ C#

+ Window 10

## 3. 사용 기술
| 기술 | 설명 |
|:---:|:---|
| 디자인 패턴 | ● **싱글톤** 패턴을 사용하여 Manager 관리 <br> ● **State** 패턴을 사용하여 캐릭터의 기능을 직관적으로 관리 |
| SkinnedMeshRender | 캐릭터의 얼굴을 꾸미고, 장비 장착 시 캐릭터의 의상이 변경되도록 구현 |
| GoogleSheet | 구글 스프레드 시트를 사용해 데이터 관리 |
| Save | 게임 데이터를 모두 json으로 변환하여 관리 ( Dictionary 포함 ) |
| Object Pooling | 자주 사용되는 객체는 Pool 관리하여 재사용 |
| UI 자동화 | 유니티 UI 상에서 컴포넌트로 Drag&Drop되는 일을 줄이기 위한 편의성 |

## 4. 구현 기능
+ Object
    - 플레이어:
        - 전사
    - 일반 몬스터:
        - 해골 검사
        - 날렵한 해골 검사
        - 강력한 해골 검사
    - 보스 몬스터:
        - 암흑 기사 ( 패턴 : 기본 공격 2개, 이동 공격 3개, 스킬 2개 )
    - NPC:
        - Shop NPC (포션, 장신구, 방어구, 무기)
        - Upgrade NPC
        - Quest NPC
    - 아이템:
        - HP 회복 물약, MP 회복 물약
        - 무기, 방어구, 장신구
+ UI
    - Scene:
        - PlayScene : 게임 진행 시 사용 <br>
        ( 플레이어 스탯, 미니맵, 퀘스트 알림, 스킬 퀵슬롯, 소비 아이템 퀵슬롯 )
        - CustomScene : 캐릭터 커스텀 시 사용 <br>
        ( 커스텀 부위 변경 버튼, 확인 버튼, 나가기 버튼 )
        - TitleScene : 게임 접속 시 사용 <br>
        ( 게임 시작 버튼, 세이브 로드 버튼, 나가기 버튼 )
    - Popup:
        - 인벤토리창, 장비창, 스킬창, 퀘스트창, 상점창, 강화창, 대화창, 메뉴창
        - 확인창, 개수 입력창, 메뉴창, 부활창, 슬롯Tip창, Scene Load창
    - WorldSpace
        - 피격 데미지 Effect, 체력 Bar, 이름 Bar, 네비게이션, 퀘스트 Icon

## 5. 기술 문서
+ https://docs.google.com/presentation/d/1jDW8nCpSjZaHVW5mElcxNQNMD1Ai76vOAgrC0b9hbXA/edit?usp=sharing

## 6. History 블로그
+ https://lhuhyeon.github.io/categories/3d-unity-my-rpg/

## 7. 플레이 영상
+ https://www.youtube.com/watch?v=mVkvd95_ezo

## 8. 게임 다운로드
+ https://drive.google.com/drive/folders/1X-CLIYkAuRs7gpSAvIbH40HmISIl5l-7
