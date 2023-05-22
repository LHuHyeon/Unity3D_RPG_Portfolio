using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    // 컴포넌트 타입 별로 담아줄 딕셔너리(자료구조)
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected bool _init = false;

	public virtual bool Init()
	{
		if (_init)
			return false;

		return _init = true;
	}

	private void Start()
	{
		Init();
	}

    // where T : 부모 클래스의 자식 클래스만 가능
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        // C++과 다르게 C#은 enum안에 있는 내용을 읽을 수 있다!
        string[] names = Enum.GetNames(type);
        
        // enum의 개수만큼 배열 생성 후 _objects에 추가
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for(int i = 0; i < names.Length; i++){
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    protected void BindObject(Type type) { Bind<GameObject>(type);  }
	protected void BindImage(Type type) { Bind<Image>(type);  }
	protected void BindText(Type type) { Bind<TextMeshProUGUI>(type);  }
	protected void BindButton(Type type) { Bind<Button>(type);  }

    // 사용 메소드
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        // Dictionary의 Value를 받을 변수 생성
        UnityEngine.Object[] objects = null;

        // 해당 Key 컴포넌트에 Value가 존재하는지 확인
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    // 자주 사용하는 컴포넌트는 사용하기 좋게 메소드 생성
    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }

    // Event 핸들러에 관한 메소드
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        // 객체에 컴포넌트 추가 및 읽어오기
        // EventSystem 관련 클래스이기 때문에 스크립트를 추가하면 클릭 드래그에 관한 메소드를 바로 사용 가능하다.
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        // UI_EventHandler 안에 action을 받을 Action 델리게이트가 있음!
        switch (type){
            case Define.UIEvent.Enter:
                evt.OnEnterHandler -= action;
                evt.OnEnterHandler += action;
                break;
            case Define.UIEvent.Exit:
                evt.OnExitHandler -= action;
                evt.OnExitHandler += action;
                break;
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
            case Define.UIEvent.BeginDrag:
                evt.OnBeginDragHandler -= action;
                evt.OnBeginDragHandler += action;
                break;
            case Define.UIEvent.EndDrag:
                evt.OnEndDragHandler -= action;
                evt.OnEndDragHandler += action;
                break;
            case Define.UIEvent.Drop:
                evt.OnDropHandler -= action;
                evt.OnDropHandler += action;
                break;
        }
    }
}
