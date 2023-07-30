using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/*
[ 로딩 팝업 스크립트 ]
1. Scene을 이동할 때 오래 걸리는 경우 사용된다.
2. 자주 호출되는 함수 : SetInfo()
*/

public class UI_LoadPopup : UI_Popup
{
    [SerializeField]
    Slider loadSlider;

    [SerializeField]
    TextMeshProUGUI tipText;

    int currentMessageNumber = 0;
    string[] loadMessges = new string[]{Define.LoadMessage1, Define.LoadMessage2, Define.LoadMessage3};

    public override bool Init() { return base.Init(); }

    public void SetInfo(Define.Scene type, int plusTime = 0)
    {
        // 구글 시트 데이터 가져오기
        if (Managers.Data.isData == false)
            OnDataRequest();

        loadSlider.value = 0;
        loadSlider.minValue = 0;
        loadSlider.maxValue = plusTime;

        currentMessageNumber = Random.Range(0,4);
        tipText.text = $"Tip : {loadMessges[currentMessageNumber]}";

        Managers.Game.StopPlayer();
        StartCoroutine(LoadAsynSceneCoroutine(type, plusTime));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentMessageNumber++;
            if (currentMessageNumber >= loadMessges.Length)
                currentMessageNumber = 0;

            tipText.text = $"Tip : {loadMessges[currentMessageNumber]}";
        }
    }
    
    // 비동기 로드
    private float loadTime = 0;
    public IEnumerator LoadAsynSceneCoroutine(Define.Scene type, int plusTime = 0)
    {
        yield return null;

        AsyncOperation operation = Managers.Scene.LoadAsynScene(type);

        while (operation.isDone == false)
        {
            loadTime += Time.deltaTime;

            loadSlider.value = loadTime;

            if (loadTime > plusTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void OnDataRequest()
    {
        StartCoroutine(Managers.Data.DataRequest(Define.StartNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.LevelNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.SkillNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.UseItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.WeaponItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.ArmorItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.DropItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.MonsterNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.ShopNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.TalkNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.QuestNumber));

        Managers.Data.isData = true;
    }
}
