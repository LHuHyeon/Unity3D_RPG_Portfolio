using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterCustom : MonoBehaviour
{
    /*
    1. 커스텀 부위 : 머리카락, 눈썹, 수염, 얼굴 문신, 상체, 하체
    2. 완성 시 해당 부위들 저장
    */

    [SerializeField] List<GameObject> hairList = new List<GameObject>();
    [SerializeField] List<GameObject> headList = new List<GameObject>();
    [SerializeField] List<GameObject> eyebrowsList = new List<GameObject>();
    [SerializeField] List<GameObject> facialHairList = new List<GameObject>();
    [SerializeField] List<GameObject> torsoList = new List<GameObject>();
    [SerializeField] List<GameObject> hipsList = new List<GameObject>();

    int currentHairIndex = 0;
    int currentHeadIndex = 0;
    int currentEyebrowsIndex = 0;
    int currentFacialHairIndex = 0;
    int currentTorsoIndex = 0;
    int currentHipsIndex = 0;

    float playerRotationY = 0.01f;

    [SerializeField]
    float rotationSpeed = 3.5f;

    void Start()
    {
        currentHairIndex = 0;
        currentHeadIndex = 0;
        currentEyebrowsIndex = 0;
        currentFacialHairIndex = 0;
        currentTorsoIndex = 0;
        currentHipsIndex = 0;
    }

    void Update()
    {
        CharaterRotation();
    }

    void CharaterRotation()
    {
        // UI를 클릭했을 때
        if (Input.GetMouseButtonDown(0) == true || Input.GetMouseButtonDown(1) == true)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
        }

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

    void SetRotation(float horizontal)
    {
        playerRotationY += horizontal * rotationSpeed;
        
        transform.localRotation = Quaternion.Euler(0f, playerRotationY, 0f);
    }

    public void NextPart(Define.DefaultPart partType, bool isNext)
    {
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

    void ChangePart(List<GameObject> partList, ref int currentIndex, bool isNext)
    {
        partList[currentIndex].SetActive(false);

        if (isNext == true)
        {
            currentIndex++;

            // 다음버튼 눌렀을 때 현재 인덱스가 마지막이라면 처음으로 이동
            if (currentIndex >= partList.Count)
                currentIndex = 0;
        }
        else
        {
            currentIndex--;

            // 뒤로버튼 눌렀을 때 현재 인덱스가 처음이라면 마지막으로 이동
            if (currentIndex < 0)
                currentIndex = partList.Count - 1;
        }

        partList[currentIndex].SetActive(true);
    }

    public void SaveCustom()
    {
        Managers.Game.DefaultPart = new Dictionary<Define.DefaultPart, SkinnedData>();

        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hair, SetSkinned(hairList[currentHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Head, SetSkinned(headList[currentHeadIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Eyebrows, SetSkinned(eyebrowsList[currentEyebrowsIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.FacialHair, SetSkinned(facialHairList[currentFacialHairIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Torso, SetSkinned(torsoList[currentTorsoIndex]));
        Managers.Game.DefaultPart.Add(Define.DefaultPart.Hips, SetSkinned(hipsList[currentHipsIndex]));
    }

    SkinnedData SetSkinned(GameObject skinnedObject)
    {
        SkinnedMeshRenderer skinnedMesh = skinnedObject.GetComponent<SkinnedMeshRenderer>();

        SkinnedData skinned = new SkinnedData(){
            sharedMeshName = skinnedMesh.name,
            bounds = skinnedMesh.localBounds,
            rootBoneName = skinnedMesh.rootBone.name,
        };

        skinned.bones = new List<string>();
        foreach(Transform child in skinnedMesh.bones)
            skinned.bones.Add(child.name);
        
        return skinned;
    }
}
