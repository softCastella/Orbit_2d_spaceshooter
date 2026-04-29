using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 파워 단계별 총알 프리팹 배열 (인덱스 0 = 파워1, 1 = 파워2, 2 = 파워3)
    // Inspector에서 PlayerBullet0, PlayerBullet1, PlayerBullet2 순서로 연결
    public GameObject[] bulletPrefabs = new GameObject[3];

    // 총구 위치 오브젝트 (Player 자식의 FirePoint를 연결)
    public Transform firePoint;

    // 플레이어 이동 속도
    public float speed = 5f;

    void Update()
    {
        // 키보드 입력 객체 가져오기 (New Input System)
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 좌우(x), 상하(y) 입력값을 -1 / 0 / 1 로 읽기
        float x = (keyboard.rightArrowKey.isPressed ? 1f : 0f) - (keyboard.leftArrowKey.isPressed ? 1f : 0f);
        float y = (keyboard.upArrowKey.isPressed ? 1f : 0f) - (keyboard.downArrowKey.isPressed ? 1f : 0f);

        // 이동 방향 * 속도 * 프레임 보정으로 위치 이동
        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);

        // 스페이스바를 누른 순간(1프레임)에만 총알 1발 발사
        if (keyboard.spaceKey.wasPressedThisFrame)
            Fire();
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
