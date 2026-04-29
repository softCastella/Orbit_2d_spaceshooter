using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { A, B, C }
    public EnemyType enemyType;

    public int hp;
    public float power;
    public float speed;
    public int exp;
    public float shootInterval;
    public int damage;  
    public Vector3 moveDirection = Vector3.down;



    void Start()
    {
        //적 타입에 따라 스탯 설정
        switch (enemyType)
        {
            case EnemyType.A:
                hp = 80; power = 1f; speed = 1f; exp = 10; shootInterval = 2f; damage = 10; //적당히 빠른애
                break; 
            case EnemyType.B:
                hp = 100; power = 1.5f; speed = 6f; exp = 15; shootInterval = 1.5f; damage = 15; //가장 빠른애
                break;
            case EnemyType.C:
                hp = 200; power = 3f; speed = 0.5f; exp = 20; shootInterval = 3f; damage = 30;  //가장 느린애
                break;
        }
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
        
    }
}
