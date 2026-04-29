using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 파워 단계별 총알 프리팹 배열 (인덱스 0 = 파워1, 1 = 파워2, 2 = 파워3)
    // Inspector에서 PlayerBullet0, PlayerBullet1, PlayerBullet2 순서로 연결
    public GameObject[] bulletPrefabs = new GameObject[3];

    // 총알이 발사될 위치 오브젝트 (Player의 자식 오브젝트 FirePoint를 연결)
    // 이 오브젝트의 위치를 기준으로 총알이 생성되므로 총구 위치가 항상 일정하게 유지됨
    public Transform firePoint;

    // 플레이어 이동 속도
    public float speed = 5f;

    void Update()
    {
        // 현재 키보드 입력 상태를 가져옴 (New Input System)
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float x = 0f;
        float y = 0f;

        // 좌우 이동 입력 처리
        if (keyboard.leftArrowKey.isPressed)
            x = -1f;
        else if (keyboard.rightArrowKey.isPressed)
            x = 1f;

        // 상하 이동 입력 처리
        if (keyboard.upArrowKey.isPressed)
            y = 1f;
        else if (keyboard.downArrowKey.isPressed)
            y = -1f;

        // 이동 방향 * 속도 * 프레임 보정으로 위치 이동
        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);

        // 스페이스바를 누른 프레임에만 총알 발사
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            // PlayerStats에서 현재 파워 값을 읽어 배열 인덱스로 변환 (파워1→0, 파워2→1, 파워3→2)
            int power = PlayerStats.Instance != null ? PlayerStats.Instance.Power : 1;
            int index = Mathf.Clamp(power - 1, 0, bulletPrefabs.Length - 1);

            // 해당 인덱스의 총알 프리팹이 연결되어 있는지 확인
            if (bulletPrefabs[index] == null)
            {
                Debug.LogWarning($"[PlayerController] bulletPrefabs[{index}]가 비어 있습니다. Inspector에서 연결해 주세요.");
                return;
            }

            // FirePoint가 연결되어 있으면 그 위치에, 없으면 플레이어 위치에 총알 생성
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            GameObject bulletGo = Instantiate(bulletPrefabs[index]);
            bulletGo.transform.position = spawnPos;
        }
    }
}
