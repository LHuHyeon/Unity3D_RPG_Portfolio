using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유니티에서 제공하는 명령어를 더 쉽게 사용하기 위한 매니저
public class ResourceManager
{
    // 객체 읽기
    public T Load<T>(string path) where T : Object
    {
        // 찾으려는 타입이 GameObject일 경우 Pool에서 찾음.
        if (typeof(T) == typeof(GameObject)){
            string name = path;
            int index = name.LastIndexOf('/');  // '/' 문자까지의 문자열 개수 반환
            if (index >= 0)
                name.Substring(index + 1);      // name을 index+1 문자열 위치에서 반환

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go.IsNull() == false)
                return go as T;
                // return go.GetComponent<T>();
        }

        // Pool에서 찾지 못하면 Resources 경로에서 가져옴.
        // [유니티에서 제공하는 Resources 폴더 안에 해당 경로의 프리팹을 읽어오는 클래스]
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(GameObject obj, Transform parent = null)
    {
        if (obj.IsNull() == true){
            Debug.Log("객체가 존재하지 않습니다.");
            return null;
        }

        // 풀링이 적용된 객체인지 확인
        if (obj.GetComponent<Poolable>().IsNull() == false)
            return Managers.Pool.Pop(obj, parent).gameObject;

        // 해당 original 프리팹을 parent의 자식 객체로 생성하기
        GameObject go = Object.Instantiate(obj, parent);
        go.name = obj.name;    // (Clone) 이름을 없애기 위한 코드

        return go;
    }

    // 프리팹 생성
    public GameObject Instantiate(string path, Transform parent = null)
    {
        // original 프리팹 객체 읽어오기.
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original.IsNull() == true){
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // 풀링이 적용된 객체인지 확인
        if (original.GetComponent<Poolable>().IsNull() == false)
        {
            // UI 팝업이면 하나만 생성
            if (original.GetComponent<UI_Popup>().IsNull() == false)
                Managers.Pool.CreatePool(original, 1); 

            return Managers.Pool.Pop(original, parent).gameObject;
        }

        // 해당 original 프리팹을 parent의 자식 객체로 생성하기
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;    // (Clone) 이름을 없애기 위한 코드

        return go;
    }

    // 오브젝트 삭제
    public void Destroy(GameObject go)
    {
        if (go.IsNull() == true)
            return;
        
        // 만약에 풀링이 필요한 아이라면 PoolManager한테 위탁
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable.IsNull() == false){
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
