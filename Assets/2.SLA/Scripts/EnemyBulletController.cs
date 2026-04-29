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

            //총알 제거
            Destroy(gameObject);
        }
    }
}
