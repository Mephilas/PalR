using MathSelf;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControllerTest : MonoBehaviour
{
    public float Height = 10;
    void Start()
    {
        Camera.main.transform.position = new(0, Height, 0);
        Camera.main.transform.eulerAngles = new(90, 0, 0);
    }

    Vector3 MoveDir = new();
    float MoveSpeed = 5;
    void Update()
    {
        MoveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            MoveDir.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveDir.z -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveDir.x += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveDir.x -= 1;
        }
        Camera.main.transform.position += MoveDir.normalized * MoveSpeed * Time.deltaTime;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 0.5f, 3);
    }
}
