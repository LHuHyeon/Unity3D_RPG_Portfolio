using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    // 컴포넌트 찾은 후 추가하기
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();

        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    // GameObjec는 컴포넌트가 아니므로 Transform을 통해 찾음
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        
        return transform.gameObject;
    }

    // 자식 객체 컴포넌트 찾기
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        // recursive : 자기 자신의 자식 객체를 가져올지 판단
        if (recursive == false){
            // go의 자식객체 수 만큼
            for(int i=0; i<go.transform.childCount; i++){
                // 지정된 자식객체를 transform에 반환
                Transform transform = go.transform.GetChild(i);

                // string.IsNullOrEmpty = 빈문자열이면 true (null 또는 "")
                if (string.IsNullOrEmpty(name) || transform.name == name){
                    // 해당 T(Button, Text, ...) 컴포넌트 반환
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else{
            // true일 경우 자식의 자식까지 다 가져온다.
            foreach(T component in go.GetComponentsInChildren<T>()){
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}
