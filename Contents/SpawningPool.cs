using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * File :   SpawningPool.cs
 * Desc :   몬스터 생성
 *
 & Functions
 &  [Public]
 &  : AddMonsterCount()     - 몬스터 수 증가
 &  : SetKeepMonsterCount() - 최대 몬스터 지정
 &
 &  [Private]
 &  : ReserveSpawn()  - 몬스터 스폰 코루틴
 *
 */

public class SpawningPool : MonoBehaviour
{
    public GameObject   _spawnMonsterNumber;    // 몬스터 Prefab

    [SerializeField]
    private Vector3     _spawnPos;              // 스폰 위치

    [SerializeField]
    private float       _spawnRedius = 5f;      // 스폰 최대 거리

    [SerializeField]
    private float       _spawnTime = 5f;        // 스폰 최대 시간

    [SerializeField]
    private int         _monsterCount = 0;      // 현재 몬스터 수
    private int         _reserveCount = 0;      // 임시 변수 (에러 방지)

    [SerializeField]
    private int         _keepMonsterCount = 0;  // 최대 몬스터 수

    // 몬스터 수 증가
    public void AddMonsterCount(Transform parent, int value)
    {
        // 스포너 부모 체크
        if (transform == parent) 
            this._monsterCount += value;
    }
    // 최대 몬스터 지정
    public void SetKeepMonsterCount(int count) { this._keepMonsterCount = count; }

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        // 최대 몬스터 수 만큼 생성
        while((_reserveCount + _monsterCount) < _keepMonsterCount)
            StartCoroutine("ReserveSpawn");
    }

    // 몬스터 스폰 설정
    private IEnumerator ReserveSpawn()
    {
        // 코루틴이 시작하자마자 시간을 기다리므로 _monsterCount가 증가하지 못해 Update의 while에서 Error가 발생할 수 있다.
        // 그러므로 _reserveCount를 사용해 while을 끝내도록 한다.
        _reserveCount++;

        yield return new WaitForSeconds(Random.Range(1, _spawnTime));

        // 몬스터 생성
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, _spawnMonsterNumber, transform);
        NavMeshAgent nav = obj.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;

        // 소환 가능한 위치를 찾을 때까지 루프
        while(true)
        {
            Vector3 randDir = Random.insideUnitSphere * _spawnRedius;   // 원 형태 랜덤 벡터 지정
            randDir.y = 0;
            randPos = _spawnPos + randDir;

            NavMeshPath path = new NavMeshPath();
            if (nav.CalculatePath(randPos, path))   // randPos 위치에 소환 가능 여부 확인
            {
                obj.transform.position = randPos;
                break;
            }
        }

        // 위치 설정
        nav.nextPosition = randPos;
        obj.GetComponent<MonsterController>().spawnPos = randPos;

        // while의 에러 예방 목적인 변수이므로 코루틴이 끝날땐 --를 해준다.
        _reserveCount--;
    }
}
