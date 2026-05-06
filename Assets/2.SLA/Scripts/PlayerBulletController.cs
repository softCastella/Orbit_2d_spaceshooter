using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 5;

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);

        // 화면 위로 벗어나면 삭제하지 않고 풀로 반납한다
        if (transform.position.y > 5.5f)
            ObjectPoolManager.instance.ReleaseObject(gameObject);
    }
}
