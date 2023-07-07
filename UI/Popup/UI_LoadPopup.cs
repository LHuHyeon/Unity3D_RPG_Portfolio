using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_LoadPopup : UI_Popup
{
    [SerializeField]
    Slider loadSlider;

    public override bool Init() { return base.Init(); }

    public void SetInfo(Define.Scene type, int plusTime = 0)
    {
        if (Managers.Data.isData == false)
            OnDataRequest();

        loadSlider.value = 0;
        loadSlider.minValue = 0;
        loadSlider.maxValue = plusTime;

        Managers.Game.StopPlayer();
        StartCoroutine(LoadAsynSceneCoroutine(type, plusTime));
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
