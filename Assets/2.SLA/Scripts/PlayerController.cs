using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //anim이라는 이름의 Animator를 쓸 거라는 변수 선언
    Animator anim;
    // 파워 단계별 총알 프리팹 배열 (인덱스 0 = 파워1, 1 = 파워2, 2 = 파워3)
    // Inspector에서 PlayerBullet0, PlayerBullet1, PlayerBullet2 순서로 연결
    public GameObject[] bulletPrefabs = new GameObject[3];

    // 총구 위치 오브젝트 (Player 자식의 FirePoint를 연결)
    public Transform firePoint;

    // 플레이어 이동 속도
    public float speed = 5f;

    // 발사 간격(초) : 0.3이면 1초에 약 3발
    public float fireRate = 0.3f;

    // 다음 발사가 가능한 시각을 기억해 두는 변수
    float nextFireTime = 0f;
    
    void Start()
    {
        //게임 오브젝트에 붙어있는 Animator 컴포넌트를 찾아서 anim에 넣어주기
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);

        // GetKey : 키를 누르고 있는 동안 매 프레임 true
        // Time.time >= nextFireTime : 마지막 발사 후 fireRate 초가 지났을 때만 발사
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Fire();
            // 다음 발사 가능 시각을 현재 시각 + 간격으로 갱신
            nextFireTime = Time.time + fireRate;
        }
        
        //Player 프리팹 메카님 : Idle, Left, Right 전환
        // -1(왼쪽), 0(정지), 1(오른쪽)
        float h = Input.GetAxisRaw("Horizontal"); 

        if (h < 0) // 왼쪽 누름
            anim.SetInteger("State", 1);
        else if (h > 0) // 오른쪽 누름
            anim.SetInteger("State", 2);
        else // 아무것도 안 누름
            anim.SetInteger("State", 0);
    }

    void Fire()
    {
        // PlayerStats의 파워(1~3)를 배열 인덱스(0~2)로 변환
        int power = PlayerStats.Instance != null ? PlayerStats.Instance.Power : 1;
        int index = Mathf.Clamp(power - 1, 0, bulletPrefabs.Length - 1);

        // 해당 슬롯이 비어있으면 경고 후 중단
        if (bulletPrefabs[index] == null)
        {
            Debug.LogWarning($"[PlayerController] bulletPrefabs[{index}]가 비어 있습니다. Inspector에서 연결해 주세요.");
            return;
        }

        // FirePoint 위치에 총알 생성 (FirePoint 없으면 플레이어 위치 사용)
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Instantiate(bulletPrefabs[index], spawnPos, Quaternion.identity);
    }
}
