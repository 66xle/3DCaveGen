using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CaveGen script;

    public float walkSpeed;
    public float jumpForce;

    Rigidbody rb;
    Vector3 moveDir;

    public Transform cam;

    LayerMask layerMask = 1 << 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDir = (horizontal * transform.right + vertical * new Vector3(transform.forward.x, 0, transform.forward.z)).normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = transform.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Raycast();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = moveDir * (walkSpeed * 100) * Time.deltaTime;
        rb.velocity += yVelFix;
    }

    void Raycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 100.0f, layerMask))
        {
            Vector3 point = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            point += (new Vector3(hit.normal.x, hit.normal.y, hit.normal.z)) * -0.5f;

            script.SetData(point, 0);
        }
    }
}
