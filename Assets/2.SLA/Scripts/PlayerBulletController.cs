using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y > 5.5f)
            Destroy(gameObject);
    }
}
