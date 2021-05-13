using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minX = -60f;
    public float maxX = 60;
    public float minY = -360;
    public float maxY = 360;

    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    public Camera cam;

    float rotX = 0f;
    float rotY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        rotX += Input.GetAxis("Mouse X") * sensitivityX;
        rotY += Input.GetAxis("Mouse Y") * sensitivityY;

        rotX = Mathf.Clamp(rotX, minX, maxX);

        transform.localEulerAngles = new Vector3(0, rotY, 0);
        cam.transform.localEulerAngles = new Vector3(-rotX, rotY, 0);

    }
}
