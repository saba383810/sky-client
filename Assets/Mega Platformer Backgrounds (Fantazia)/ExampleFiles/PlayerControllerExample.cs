using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerExample : MonoBehaviour
{
    //Controller for the moving object and the camera

    public float Speed;
    public float CameraSpeed;

    public Vector2 Camera_MinXY;
    public Vector2 Camera_MaxXY;

    Rigidbody2D rb;

    public GameObject WASDText;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
    }


    //Smooth movement of the camera
    void MoveCamera()
    {
        Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, transform.position, CameraSpeed);
        Vector3 CameraPos = Camera.main.transform.position;
        if (CameraPos.x > Camera_MaxXY.x)
        {
            CameraPos.x = Camera_MaxXY.x;
        }
        else if (CameraPos.x < Camera_MinXY.x)
        {
            CameraPos.x = Camera_MinXY.x;
        }

        if (CameraPos.y > Camera_MaxXY.y)
        {
            CameraPos.y = Camera_MaxXY.y;
        }
        else if (CameraPos.y < Camera_MinXY.y)
        {
            CameraPos.y = Camera_MinXY.y;
        }
        CameraPos.z = -10;


        Camera.main.transform.position = CameraPos;

    }


    //Receiver for the input
    void Update()
    {
        int XInput = (int)Input.GetAxisRaw("Horizontal");
        int YInput = (int)Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(XInput, YInput) * Speed;

        if (WASDText.activeInHierarchy && (XInput > 0 || YInput > 0))
        {
            WASDText.SetActive(false);
        }

        MoveCamera();
    }
}
