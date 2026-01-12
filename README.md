## 1. 프로젝트 폴더 구조

본 과제에서 직접 구현한 모든 리소스는  
`Assets/Pretest_Ujin` 폴더 하위에 정리했습니다.

외부 에셋 또는 Unity 기본 리소스와 구분하기 위해  
과제 제출용 구현물은 `Pretest_Ujin` 폴더만 사용하도록 구성했습니다.

```text
Assets
└─ Pretest_Ujin
   ├─ EffectAssets
   │  └─ Effects
   │     ├─ _Materials
   │     └─ _Prefabs
   ├─ Sound
   │  ├─ EffectSound
   │  └─ Music
   ├─ Prefabs
   │  └─ Mirror_ujin
   ├─ Scene
   │  └─ Scene_UJin
   └─ Script
      ├─ EffectSoundController.cs
      ├─ Laser
      │  ├─ LaserController.cs
      │  └─ LaserReflectionCounterUI.cs
      ├─ Mirror
      │  ├─ MirrorManager.cs
      │  ├─ MirrorSurface.cs
      │  └─ MirrorTransformController.cs
      └─ Receiver
         └─ ReceiverStateEffect.cs
```  

과제 검증 시 Assets/Pretest_Ujin 폴더만 확인하면  
레이저, 거울, Receiver, UI, 사운드까지 모든 구현을 확인할 수 있습니다.

---

## 2. Mirror 프리팹 관리 방식

기본 제공 프리팹을 직접 수정하지 않고,  
Mirror_ujin 프리팹을 별도로 복사하여 과제 전용 거울 프리팹으로 사용했습니다.

Mirror_ujin 프리팹에는 다음 컴포넌트를 추가했습니다.

- MirrorSurface.cs (레이저 반사 대상 마커)  
- MirrorTransformController.cs (이동 및 회전 제어)

이를 통해 제공 에셋과 과제 구현을 명확히 분리하고,  
제출용 구조를 깔끔하게 유지했습니다.

---

## 3. 시스템 구조

본 프로젝트는 Laser, Mirror, Receiver를 각각 독립된 컴포넌트로 분리하여  
역할과 책임이 명확한 구조로 설계했습니다.

Laser  
 └─ LaserController.cs  

Mirror  
 ├─ MirrorManager.cs  
 ├─ MirrorTransformController.cs  
 └─ MirrorSurface.cs  

Receiver  
 └─ ReceiverStateEffect.cs  

UI  
 └─ LaserReflectionCounterUI.cs  

Sound  
 └─ EffectSoundController.cs  

---

## 4. Laser

LaserController는 매 프레임 Raycast 기반으로 레이저를 발사합니다.

레이저는 다음 규칙으로 동작합니다.

- 충돌 지점까지 LineRenderer로 시각화
- MirrorSurface에 닿으면 Vector3.Reflect를 사용하여 정반사
- 최대 10회까지만 반사 가능
- ReceiverStateEffect에 닿으면 히트 처리 수행

---

## 5. Mirror

MirrorManager는 플레이 중 거울의 생성, 선택, 삭제를 담당합니다.

- Z 키로 거울 생성  
- 마우스 클릭으로 선택  
- Delete 키로 선택된 거울 삭제  

MirrorTransformController는 선택된 거울만 조작할 수 있도록 제한하며,  
MirrorSurface는 레이저 반사 대상 여부를 구분하는 마커 역할을 합니다.

---

## 5.1 Mirror 조작 방식

MirrorTransformController.cs 에서 관리 - 이동/회전 속도 조절 가능

- 거울 생성 : z
- 거울 회전 : q/e
- 거울 이동 : 마우스 드래그
- 거울 위/아래 이동 : w/s
- 거울 위/아래 회전 : r/f
- 거울 삭제 : 삭제 대상 클릭 후 del

---

## 6. Receiver

ReceiverStateEffect는 레이저 히트 상태에 따라 시각적 피드백을 제공합니다.

- 히트 중  
  - 이펙트 활성화  
  - 스케일 확대  
  - 색상 변경  

- 히트 해제 시  
  - 이펙트 비활성화  
  - 스케일 및 색상 원복  

프레임 단위 판정 구조로,  
레이저가 끊기면 자동으로 원래 상태로 복귀합니다.

---

## 추가 구현 기능

---

## 7. UI

LaserReflectionCounterUI는 레이저의 남은 반사 횟수를 표시합니다.
- 반사가 남아 있을 때: 숫자로 표시  
- 반사 소진 시: "No Reflections Left" 문구와 경고 색상 표시  

---

## 8. 사운드

EffectSoundController는 씬에 하나만 존재하는 싱글톤 구조이며,  
레이저가 거울 또는 Receiver에 처음 닿는 순간 효과음을 재생합니다.
