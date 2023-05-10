using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _order = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    // 프리팹 오브젝트 부모 (하이라이커 깔끔하게 정리하려고 사용)
    public GameObject Root
    {
        get{
            GameObject root = GameObject.Find("@UI_Root");// 오브젝트 찾기

            if (root == null)
                root = new GameObject{name = "@UI_Root"}; // 오브젝트 이름 설정

            return root;
        }
    }

    // 오브젝트에 Canvas를 추가하고 order을 설정
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort){
            canvas.sortingOrder = _order;
            _order++;
        }
        else{
            canvas.sortingOrder = 0;
        }
    }

    // 3D 안에 있는 WorldSpace에서 UI 생성 (캐릭터 체력 UI ...)
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name)){
            name = typeof(T).Name;
        }

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>(); // or -> go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return go.GetOrAddComponent<T>();
    }

    // 인벤토리의 슬롯 및 아이템 생성
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name)){
            name = typeof(T).Name;
        }

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        return go.GetOrAddComponent<T>();
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        // name = null 경우
        if (string.IsNullOrEmpty(name)){
            name = typeof(T).Name;
        }

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    // UI에 만들어질 프리팹을 stack에 넣어 order을 관리
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name)){
            name = typeof(T).Name;
        }

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }

    // 스택의 마지막 위치에 popup이 있나 확인 후 삭제
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;
        
        if (_popupStack.Peek() != popup){
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    // Stack pop 진행
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;
        
        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;

        _order--;
    }

    // Stack 안에 있는 전체 pop
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0){
            ClosePopupUI();
        }
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
