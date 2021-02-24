using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Range(0.1F, 100F)] public float forwardSpeed;
    [Range(0.1F, 100F)] public float reverseSpeed;
    [Range(0.1F, 100F)] public float turnSpeed;
    [Range(0, 10)] public float airDrag;
    [Range(0F, 10)] public float groundDrag;
    public LayerMask maskGroundLayer;
    public Rigidbody motorRigidbody;
    private float moveInput;
    private float turnInput;
    private bool isGrounded;

    private void Start()
    {
        motorRigidbody.transform.parent = null;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
        moveInput *= moveInput > 0 ? forwardSpeed : reverseSpeed;

        transform.position = motorRigidbody.transform.position;
        var newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        ;
        transform.Rotate(0, newRotation, 0, Space.World);

        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, maskGroundLayer);
        motorRigidbody.drag = isGrounded ? groundDrag : airDrag;

        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
    }

    private void FixedUpdate()
    {
        if (isGrounded)
            motorRigidbody.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        else
        {
            motorRigidbody.AddForce(transform.up * -30f);
        }
    }
}