using UnityEngine;

public class Enemy : MonoBehaviour
{   
    //에너미 타입
    public enum EnemyType { A, B, C }
    public EnemyType enemyType;

    //적 스탯
    public int hp;
    public float power;
    public float speed;
    public int exp;
    public int damage;  
    public float shootInterval;

    //적의 이동 방향
    public Vector3 moveDirection = Vector3.down;

    //스프라이트
    private SpriteRenderer sr;
    //적의 사망 여부
    private bool isDead = false;
    
    //플레이어 위치 정보
    private Transform playerTransform;
    //플레이어 콜라이더 정보
    private Collider2D playerCollider;
    //적 총알 정보0
    public GameObject enemyBulletPrefab0;
    //적 총알 정보1 (타입C용)
    public GameObject enemyBulletPrefab1;
    //스프라이트 배열
    public Sprite[] sprites; // 0: 기본, 1: 피격
    //적 총알 발사구 정보
    private Transform firePoint0;
    private Transform firePoint1;


    //발사 타이머
    private float shootTimer;

    void Start()
    {
        //적 타입에 따라 스탯 개별 설정
        switch (enemyType)
        {
            case EnemyType.A:
                hp = 80; power = 1f; speed = 1f; exp = 10; shootInterval = 2f; damage = 10; //적당히 빠른애
                break; 
            case EnemyType.B:
                hp = 100; power = 1.5f; speed = 1.5f; exp = 15; shootInterval = 1.5f; damage = 15; //가장 빠른애
                break;
            case EnemyType.C:
                hp = 200; power = 3f; speed = 0.5f; exp = 20; shootInterval = 3f; damage = 30;  //가장 느린애
                break;
        }

        // FirePoint 자식 오브젝트 수집
        firePoint0 = transform.Find("FirePoint_0");
        firePoint1 = transform.Find("FirePoint_1");

        if (firePoint0 == null) Debug.LogWarning($"[Enemy] FirePoint_0을 찾지 못했습니다. ({gameObject.name})");
        if (enemyBulletPrefab0 == null) Debug.LogWarning($"[Enemy] enemyBulletPrefab0이 비어있습니다. ({gameObject.name})");

        shootTimer = shootInterval;
    }

    // Update is called once per frame
    void Update()
    {   
        //적 이동
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 화면 밖으로 나가면 제거
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        if (vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f)
        {
            Destroy(gameObject);
            return;
        }

        // 발사 타이머
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    void Shoot()
    {
        if (firePoint0 != null && enemyBulletPrefab0 != null)
            Instantiate(enemyBulletPrefab0, firePoint0.position, firePoint0.rotation);

        if (enemyType == EnemyType.C && firePoint1 != null && enemyBulletPrefab1 != null)
            Instantiate(enemyBulletPrefab1, firePoint1.position, firePoint1.rotation);
    }
}
