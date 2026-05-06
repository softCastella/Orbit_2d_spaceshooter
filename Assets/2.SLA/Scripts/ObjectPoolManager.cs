using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // 어디서든 접근 가능한 싱글톤 인스턴스
    public static ObjectPoolManager instance;

    [Header("--- Prefabs ---")]
    public GameObject playerBullet0Prefab;
    public GameObject playerBullet1Prefab;
    public GameObject enemyAPrefab;
    public GameObject enemyBPrefab;
    public GameObject enemyCPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;
    public GameObject enemyBullet0Prefab;

    // 각 오브젝트를 저장할 리스트들
    private List<GameObject> playerBullet0List = new List<GameObject>();
    private List<GameObject> playerBullet1List = new List<GameObject>();
    private List<GameObject> enemyAList = new List<GameObject>();
    private List<GameObject> enemyBList = new List<GameObject>();
    private List<GameObject> enemyCList = new List<GameObject>();
    private List<GameObject> itemCoinList = new List<GameObject>();
    private List<GameObject> itemPowerList = new List<GameObject>();
    private List<GameObject> itemBoomList = new List<GameObject>();
    private List<GameObject> enemyBullet0List = new List<GameObject>();

    private void Awake()
    {
        // 싱글톤 설정: 이미 인스턴스가 있다면 파괴, 없다면 나 자신을 할당
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 초기 생성 (기존과 동일하게 개수 설정 가능)
        PreGenerate(playerBullet0Prefab, playerBullet0List, 20);
        PreGenerate(playerBullet1Prefab, playerBullet1List, 20);
        PreGenerate(enemyAPrefab, enemyAList, 10);
        PreGenerate(enemyBPrefab, enemyBList, 10);
        PreGenerate(enemyCPrefab, enemyCList, 20);
        PreGenerate(itemCoinPrefab, itemCoinList, 20);
        PreGenerate(itemPowerPrefab, itemPowerList, 10);
        PreGenerate(itemBoomPrefab, itemBoomList, 10);
        PreGenerate(enemyBullet0Prefab, enemyBullet0List, 20);
    }

    // [개선 1] 반복되는 생성 로직을 하나의 메서드로 공통화
    private void PreGenerate(GameObject prefab, List<GameObject> list, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab); // 프리팹 생성
            obj.transform.SetParent(transform);   // 매니저 자식으로 설정
            obj.SetActive(false);                 // 비활성화
            list.Add(obj);                        // 리스트에 추가
        }
    }

    // [개선 2] 리스트에서 오브젝트를 찾는 공통 로직 (부족하면 새로 생성함)
    private GameObject GetFromPool(List<GameObject> list, GameObject prefab)
    {
        // 1. 리스트에서 비활성화된 오브젝트를 찾음
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].activeInHierarchy)
            {
                return list[i]; // 찾으면 즉시 반환
            }
        }

        // 2. 만약 다 사용 중이라서 못 찾았다면? 새로 하나 만들어서 리스트에 추가하고 반환! (중요)
        GameObject newObj = Instantiate(prefab);
        newObj.transform.SetParent(transform);
        list.Add(newObj);
        return newObj;
    }

    // --- 외부에서 호출하는 함수들 (매우 깔끔해짐) ---

    public GameObject GetPlayerBullet0() => GetFromPool(playerBullet0List, playerBullet0Prefab);
    public GameObject GetPlayerBullet1() => GetFromPool(playerBullet1List, playerBullet1Prefab);
    public GameObject GetEnemyBullet0() => GetFromPool(enemyBullet0List, enemyBullet0Prefab);
    public GameObject GetEnemyA() => GetFromPool(enemyAList, enemyAPrefab);
    public GameObject GetEnemyB() => GetFromPool(enemyBList, enemyBPrefab);
    public GameObject GetEnemyC() => GetFromPool(enemyCList, enemyCPrefab);

    // 아이템은 타입에 따라 가져오기
    public GameObject GetItem(ItemPickup.ItemType itemType)
    {
        switch (itemType)
        {
            case ItemPickup.ItemType.Coin:  return GetFromPool(itemCoinList, itemCoinPrefab);
            case ItemPickup.ItemType.Power: return GetFromPool(itemPowerList, itemPowerPrefab);
            case ItemPickup.ItemType.Boom:  return GetFromPool(itemBoomList, itemBoomPrefab);
            default: return null;
        }
    }

    // [공통 반납 함수] 총알, 적, 아이템 구분 없이 이 함수 하나로 반납 가능
    public void ReleaseObject(GameObject obj)
    {
        obj.SetActive(false);             // 비활성화
        obj.transform.position = Vector3.zero; // 위치 초기화
        // 필요하다면 리지드바디 속도 초기화 등을 여기서 수행할 수 있습니다.
    }
}