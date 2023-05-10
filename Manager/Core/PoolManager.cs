using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ResourceManager의 보조역할 느낌
public class PoolManager
{
    class Pool
    {
        public GameObject Original { get; private set; }    // Pool을 진행할 대표 변수
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        // Pool 초기 설정
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;      // Pool을 담을 Root Object    
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)         // count 만큼 pool Object 생성 후 Stack에 push
                Push(Create());
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);   // Original 복제
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();    // Poolable 컴포넌트 생성
        }

        // 객체 생성 메소드
        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        // 객체 반환 메소드
        public Poolable Pop(Transform parent = null)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();
            
            poolable.gameObject.SetActive(true);

            // DontDestroyOnLoad 해제 용도 (SceneManager 객체를 이용)
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            
            poolable.transform.parent = parent;
            poolable.IsUsing = true;

            return poolable;
        }
    }

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();    // pool 객체 저장
    Transform _root;    // 오브젝트 생성 경로

    public void Init()
    {
        // Pool Object를 담을 부모 객체(_root) 경로 설정
        if (_root == null){
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    // 새로운 pool 생성 후 저장
    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);     // Pool 생성
        pool.Root.parent = _root;       // _root(@Pool_Root)를 부모 객체로 설정

        _pool.Add(original.name, pool);
    }

    // 기존 pool 저장 메소드
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;

        if (_pool.ContainsKey(name) == false){
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }

    // pool 반환 메소드
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        // Poolable 컴포넌트가 붙은 객체인데 저장된 Key가 없을 경우 생성
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);

        return _pool[original.name].Pop(parent);        // Pop 진행
    }

    // pool 객체 읽기 메소드
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }

    // mmorpg에서 지역마다 쓰는 객체다 다를 수도 있기 때문에 일단 구현!
    public void Clear()
    {
        // @Pool_Root 안에 있는 객체 모두 제거
        foreach(Transform child in _root){
            GameObject.Destroy(child.gameObject);
        }
        _pool.Clear();  // Pool 초기화
    }
}
