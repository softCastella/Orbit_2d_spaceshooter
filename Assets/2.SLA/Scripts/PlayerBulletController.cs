using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 5;

    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);

        if (transform.position.y > 5.5f)
            Destroy(gameObject);
    }
}
