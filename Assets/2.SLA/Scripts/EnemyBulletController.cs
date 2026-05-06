using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    //총알 이동 속도
    public float speed = 8f;
    //총알 데미지
    public int damage = 10;
    //총알 이동 방향
    public Vector3 moveDirection = Vector3.down;

    //총알 이동 월드좌표계    
    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 화면 밖으로 나가면 삭제하지 않고 풀로 반납한다
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        if (vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f)
        {
            ObjectPoolManager.instance.ReleaseObject(gameObject);
        }
    }

    //플레이어와 충돌 시 데미지 전달 후 총알 제거
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            //플레이어에게 데미지 전달
            if (player != null) 
            {
                // player.TakeDamage(damage);
                // Debug.Log("Player HP: " + player.hp);
            }

            //총알 제거하지 않고 풀로 반납한다
            ObjectPoolManager.instance.ReleaseObject(gameObject);
        }
    }
}
