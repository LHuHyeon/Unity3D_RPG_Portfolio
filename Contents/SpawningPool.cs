using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 몬스터 자동 생성
public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;      // 현재 몬스터 수
    int _reserveCount = 0;      // 임시 변수 (에러 방지)

    [SerializeField]
    int _keepMonsterCount = 0;  // 최대 몬스터 수

    [SerializeField]
    Vector3 _spawnPos;          // 스폰 위치

    [SerializeField]
    float _spawnRedius = 15f;   // 스폰 최대 거리

    [SerializeField]
    float _spawnTime = 5f;      // 스폰 최대 시간

    // 외부에서 값 접근을 위한 메소드
    public void AddMonsterCount(int value) { _monsterCount += value; }          // 몬스터 수 증가
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }   // 최대 몬스터 지정

    void Start()
    {
        _spawnPos = new Vector3(1, 0, -11);

        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        // 최대 몬스터 수 만큼 생성
        while((_reserveCount + _monsterCount) < _keepMonsterCount){
            StartCoroutine("ReserveSpawn");
        }
    }

    // 몬스터 스폰 설정
    IEnumerator ReserveSpawn()
    {
        // 코루틴이 시작하자마자 시간을 기다리므로 _monsterCount가 증가하지 못해 Update의 while에서 Error가 발생할 수 있다.
        // 그러므로 _reserveCount를 사용해 while을 끝내도록 한다.
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(1, _spawnTime));

        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        NavMeshAgent nav = obj.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;

        // 소환 가능한 위치를 찾을 때까지 루프
        while(true){
            Vector3 randDir = Random.insideUnitSphere * _spawnRedius;   // 원 형태 랜덤 벡터 지정
            randDir.y = 0;
            randPos = _spawnPos + randDir;

            NavMeshPath path = new NavMeshPath();
            if (nav.CalculatePath(randPos, path))   // randPos 위치에 소환 가능 여부 확인
                break;
        }

        obj.transform.position = randPos;

        // while의 에러 예방 목적인 변수이므로 코루틴이 끝날땐 --를 해준다.
        _reserveCount--;
    }
}
