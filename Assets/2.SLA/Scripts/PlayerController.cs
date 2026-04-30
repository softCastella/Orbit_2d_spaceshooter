using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //anim이라는 이름의 Animator를 쓸 거라는 변수 선언
    Animation anim;
    // 파워 단계별 총알 프리팹 배열 (인덱스 0 = 파워1, 1 = 파워2, 2 = 파워3)
    // Inspector에서 PlayerBullet0, PlayerBullet1, PlayerBullet2 순서로 연결
    public GameObject[] bulletPrefabs = new GameObject[3];

    // 총구 위치 오브젝트 (Player 자식의 FirePoint를 연결)
    public Transform firePoint;

    // 플레이어 이동 속도
    public float speed = 2f;

    // 발사 쿨다운: 마지막 발사 후 최소 대기 시간
    public float cooldown = 0.15f;
    // 선입력 버퍼: 쿨다운 중 눌러도 이 시간 이내면 자동 발사
    public float graceTime = 0.08f;

    float lastFireTime = -999f;  // 마지막 발사 시각
    float bufferedFireTime = -999f; // 선입력이 들어온 시각

    // 현재 파워 단계 (1~3), W 키로 전환
    int power = 1;

    // BombBoom 프리팩 (인스펙터에서 연결)
    public GameObject bombPrefab;
    // 폭탄 지속 시간 (초) — Inspector에서 조정
    public float bombDuration = 3f;

    void Start()
    {
        //게임 오브젝트에 붙어있는 Animator 컴포넌트를 찾아서 anim에 넣어주기
        anim = GetComponent<Animation>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal"); // WASD + 방향키 모두 인식
        float y = Input.GetAxisRaw("Vertical");    // WASD + 방향키 모두 인식

        // 대각선 입력 시 속도가 √2배 빨라지는 것을 방지
        Vector3 dir = new Vector3(x, y, 0);
        if (dir.magnitude > 1f) dir.Normalize();
        transform.Translate(dir * speed * Time.deltaTime);

        // 화면 경계 계산 후 플레이어 위치 제한 (스프라이트 크기 고려)
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        float hw = bounds.extents.x; // 스프라이트 가로 절반
        float hh = bounds.extents.y; // 스프라이트 세로 절반
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, min.x + hw, max.x - hw),
            Mathf.Clamp(transform.position.y, min.y + hh, max.y - hh),
            transform.position.z
        );

        // 마우스 왼쪽 클릭 시 선입력 버퍼에 시각 기록
        if (Input.GetMouseButtonDown(0))
            bufferedFireTime = Time.time;

        // cooldown이 끝났고, 버퍼 입력이 graceTime 이내이면 발사
        bool cooldownDone = Time.time >= lastFireTime + cooldown;
        bool hasBuffer    = Time.time <= bufferedFireTime + graceTime;
        if (cooldownDone && hasBuffer)
        {
            Fire();
            lastFireTime = Time.time;
            bufferedFireTime = -999f; // 버퍼 사용 후 초기화
        }

        // [추가] 마우스 오른쪽 클릭으로 폭탄 발사
        if (Input.GetMouseButtonDown(1))
        {
            Bomb();
        }

        // W 키로 파워 단계 전환 (1 → 2 → 3 → 1 반복)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            power = power % 3 + 1;
            Debug.Log($"파워 단계: {power}");
        }

        //Player 프리팹 메카님 : Idle, Left, Right 전환
        // -1(왼쪽), 0(정지), 1(오른쪽)
        float h = Input.GetAxisRaw("Horizontal"); // WASD + 방향키 모두 인식

        if (anim == null)
        {
            Debug.LogError("[PlayerController] Animation 컴포넌트가 null입니다. Player에 Animation 컴포넌트가 있는지 확인하세요.");
        }
        else
        {
            if (h < 0)       anim.Play("Player_Left");
            else if (h > 0)  anim.Play("Player_Right");
            else             anim.Play("Player_Idle");
        }
    }

    void Fire()
    {
        // power(1~3) → bulletPrefabs 인덱스(0~2)로 변환
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

    void Bomb()
    {
        if (bombPrefab == null) return;
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        // 스프라이트 절반 높이만큼 위로 올려서 이미지 하단이 firePoint에 맞도록
        SpriteRenderer bombSr = bombPrefab.GetComponentInChildren<SpriteRenderer>();
        if (bombSr != null)
            spawnPos.y += bombSr.bounds.extents.y;

        GameObject obj = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        // 지속 시간은 bombDuration (Inspector에서 조정), 그동안 애니메이션 반복 재생
        Destroy(obj, bombDuration);
        Animator bombAnim = obj.GetComponentInChildren<Animator>();
        if (bombAnim != null)
            StartCoroutine(LoopBombAnim(bombAnim, bombDuration));

        // 폭탄 지속 시간 동안 매 프레임 영역 내 적 소멸
        SpriteRenderer bombAreaSr = obj.GetComponentInChildren<SpriteRenderer>();
        if (bombAreaSr != null)
            StartCoroutine(BombKillArea(bombAreaSr, bombDuration));
    }

    IEnumerator BombKillArea(SpriteRenderer bombAreaSr, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && bombAreaSr != null)
        {
            Bounds b = bombAreaSr.bounds;
            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);
            foreach (Enemy enemy in enemies)
            {
                if (enemy == null) continue;
                // 적의 콜라이더 bounds와 겹치면 소멸
                Collider2D col = enemy.GetComponent<Collider2D>();
                bool inZone = col != null ? b.Intersects(col.bounds) : b.Contains(enemy.transform.position);
                if (inZone)
                    Destroy(enemy.gameObject);
            }
            // 폭탄 영역 안에 있는 적 총알만 소멸
            EnemyBulletController[] bullets = FindObjectsByType<EnemyBulletController>(FindObjectsInactive.Exclude);
            foreach (EnemyBulletController bullet in bullets)
            {
                if (bullet == null) continue;
                Collider2D bCol = bullet.GetComponent<Collider2D>();
                bool bulletInZone = bCol != null ? b.Intersects(bCol.bounds) : b.Contains(bullet.transform.position);
                if (bulletInZone) Destroy(bullet.gameObject);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LoopBombAnim(Animator bombAnim, float duration)
    {
        yield return null; // 애니메이터 초기화 대기 (1프레임)
        if (bombAnim == null) yield break;

        AnimatorStateInfo stateInfo = bombAnim.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length > 0f ? stateInfo.length : 0.1f;

        float elapsed = 0f;
        while (elapsed < duration && bombAnim != null)
        {
            bombAnim.Play(stateInfo.shortNameHash, 0, 0f); // 처음부터 다시 재생
            yield return new WaitForSeconds(clipLength);
            elapsed += clipLength;
        }
    }
}