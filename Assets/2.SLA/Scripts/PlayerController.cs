using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float speed = 5f;

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float x = 0f;
        float y = 0f;

        if (keyboard.leftArrowKey.isPressed)
            x = -1f;
        else if (keyboard.rightArrowKey.isPressed)
            x = 1f;

        if (keyboard.upArrowKey.isPressed)
            y = 1f;
        else if (keyboard.downArrowKey.isPressed)
            y = -1f;

        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            GameObject bulletGo = Instantiate(bulletPrefab);
            bulletGo.transform.position = transform.position;
        }
    }
}
