using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   CharacterCustom.cs
 * Desc :   캐릭터 커스텀 제어 ( 커스텀 부위 : 머리카락, 눈썹, 수염, 얼굴 문신, 상체, 하체 )
 *
 & Functions
 &  [Public]
 &  : NextPart()    - 캐릭터의 부위를 다음 파츠로 변경
 &  : SaveCustom()  - 현재 부위들을 저장
 &
 &  [Private]
 &  : CharaterRotation() - 캐릭터 회전
 &  : ChangePart()       - 파츠 부위 변경 
 &  : SetSkinned()       - SkinnedMeshRenderer 데이터 저장
 *
 */

public class CharacterCustom : MonoBehaviour
{
    public  bool    stopRotation        = false;    // 회전 제어

    [SerializeField]
    private float   rotationSpeed       = 3.5f;     // 회전 속도
    private float   currentRotation_Y   = 0.01f;    // 캐릭터 Y 회전값 

    // 부위별 파츠 리스트
    [SerializeField] List<GameObject> hairList          = new List<GameObject>();
    [SerializeField] List<GameObject> headList          = new List<GameObject>();
    [SerializeField] List<GameObject> eyebrowsList      = new List<GameObject>();
    [SerializeField] List<GameObject> facialHairList    = new List<GameObject>();
    [SerializeField] List<GameObject> torsoList         = new List<GameObject>();
    [SerializeField] List<GameObject> hipsList          = new List<GameObject>();

    // 부위별 현재 List index
    private int currentHairIndex        = 0;
    private int currentHeadIndex        = 0;
    private int currentEyebrowsIndex    = 0;
    private int currentFacialHairIndex  = 0;
    private int currentTorsoIndex       = 0;
    private int currentHipsIndex        = 0;

    private void Update()
    {
        CharaterRotation();
    }

    // ~ UI_CustomButton.cs 에서 파츠 변경 버튼을 누를 때 호출
    public void NextPart(Define.DefaultPart partType, bool isNext)
    {
        // 부위 타입에 맞게 변경
        switch (partType)
        {
            case Define.DefaultPart.Hair:
                ChangePart(hairList, ref currentHairIndex, isNext);
                break;
            case Define.DefaultPart.Head:
                ChangePart(headList, ref currentHeadIndex, isNext);
                break;
            case Define.DefaultPart.Eyebrows:
                ChangePart(eyebrowsList, ref currentEyebrowsIndex, isNext);
                break;
            case Define.DefaultPart.FacialHair:
                ChangePart(facialHairList, ref currentFacialHairIndex, isNext);
                break;
            case Define.DefaultPart.Torso:
                ChangePart(torsoList, ref currentTorsoIndex, isNext);
                break;
            case Define.DefaultPart.Hips:
                ChangePart(hipsList, ref currentHipsIndex, isNext);
                break;
        }
    }

    // ~ UI_CustomButton.cs 에서 확인 버튼을 누를 때 호출
    public void SaveCustom()
    {
        // 딕셔너리 생성
        Managers.Game.DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

        // GameManager의 데이터에 저장
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hair, SetSkinned(hairList[currentHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Head, SetSkinned(headList[currentHeadIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Eyebrows, SetSkinned(eyebrowsList[currentEyebrowsIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.FacialHair, SetSkinned(facialHairList[currentFacialHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Torso, SetSkinned(torsoList[currentTorsoIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hips, SetSkinned(hipsList[currentHipsIndex]));
    }

    // 캐릭터 회전 (Update)
    private void CharaterRotation()
    {
        // 회전 제어
        if (stopRotation == true)
            return;

        // UI를 클릭하면 회전 X
        if (Input.GetMouseButtonDown(0) == true || Input.GetMouseButtonDown(1) == true)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
        }

        // A Key, ◀ Key : 왼쪽으로 회전
        // D Key, ▶ Key : 오른쪽으로 회전
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || 
            Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            SetRotation(-Input.GetAxis("Horizontal"));
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            SetRotation(-Input.GetAxis("Mouse X"));
        }
    }

    // 회전 설정
    private void SetRotation(float horizontal)
    {
        currentRotation_Y += horizontal * rotationSpeed;
        
        transform.localRotation = Quaternion.Euler(0f, currentRotation_Y, 0f);
    }

    // 파츠 부위 변경
    private void ChangePart(List<GameObject> partList, ref int currentIndex, bool isNext)
    {
        // 현재 부위 비활성화
        partList[currentIndex].SetActive(false);

        // ( ▶ ) Button
        if (isNext == true)
        {
            currentIndex++;

            // 다음버튼 눌렀을 때 현재 인덱스가 마지막이라면 처음으로 이동
            if (currentIndex >= partList.Count)
                currentIndex = 0;
        }
        // ( ◀ ) Button
        else
        {
            currentIndex--;

            // 뒤로버튼 눌렀을 때 현재 인덱스가 처음이라면 마지막으로 이동
            if (currentIndex < 0)
                currentIndex = partList.Count - 1;
        }

        // 변경된 부위 활성화
        partList[currentIndex].SetActive(true);
    }

    // SkinnedMeshRenderer 필요 정보 저장
    private SkinnedData SetSkinned(GameObject skinnedObject)
    {
        // SkinnedMeshRenderer 컴포넌트 받기
        SkinnedMeshRenderer skinnedMesh = skinnedObject.GetComponent<SkinnedMeshRenderer>();

        // 이름, localBounds, rootBone을 저장
        SkinnedData skinned = new SkinnedData(){
            sharedMeshName = skinnedMesh.name,
            bounds = skinnedMesh.localBounds,
            rootBoneName = skinnedMesh.rootBone.name,
        };

        // bones 저장
        skinned.bones = new List<string>();
        foreach(Transform child in skinnedMesh.bones)
            skinned.bones.Add(child.name);
        
        return skinned;
    }
}
