using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float speed = 5f;

    void Update()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            x = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            x = 1f;

        if (Input.GetKey(KeyCode.UpArrow))
            y = 1f;
        else if (Input.GetKey(KeyCode.DownArrow))
            y = -1f;

        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bulletGo = Instantiate(bulletPrefab);
            bulletGo.transform.position = transform.position;
        }
    }
}
