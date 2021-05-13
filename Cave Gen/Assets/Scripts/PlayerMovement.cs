using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed;

    Rigidbody rb;
    Vector3 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDir = (horizontal * transform.right + vertical * transform.forward).normalized;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = moveDir * walkSpeed * Time.deltaTime;
        rb.velocity += yVelFix;
    }
}
