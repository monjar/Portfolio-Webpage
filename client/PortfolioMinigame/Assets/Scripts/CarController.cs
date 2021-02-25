using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Range(0.1F, 100F)] public float forwardSpeed;
    [Range(0.1F, 100F)] public float reverseSpeed;
    [Range(0.1F, 100F)] public float turnSpeed;
    [Range(0, 90)] public float wheelTurnAngle;
    [Range(0, 10)] public float bodyTurnAngle;
    [Range(0, 1)] public float raycastGroundDistance;
    [Range(0, 90)] public float wheelTurnRoughness;
    [Range(0, 90)] public float bodyTurnRoughness;
    [Range(0, 10)] public float airDrag;
    [Range(0F, 10)] public float groundDrag;
    [Range(0F, 100)] public float groundRotationRoughness;
    public LayerMask maskGroundLayer;
    public Rigidbody motorRigidbody;
    public List<Transform> wheels;
    public List<Transform> turningWheels;
    public Transform body;
    public Transform motorCenter;
    public Animator animator;
    private float moveInput;
    private float turnInput;
    private bool isGrounded;
    private Vector3 motorOffset;
    private void Start()
    {
        motorRigidbody.transform.parent = null;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
        moveInput *= moveInput > 0 ? forwardSpeed : reverseSpeed;
        turningWheels.ForEach(w =>
        {
            w.localRotation = Quaternion.Slerp(w.localRotation,
                Quaternion.Euler(w.localRotation.x, turnInput * wheelTurnAngle,
                    w.localRotation.z),
                Time.deltaTime * wheelTurnRoughness);

            // w.rotation.r


            // w.localRotation = Quaternion.Euler(w.localRotation.x , turnInput * wheelTurnAngle,
            //     w.localRotation.z);
        });
        animator.SetFloat("WheelSpeed", -moveInput * motorRigidbody.velocity.magnitude);
        body.localRotation = Quaternion.Lerp(body.localRotation,
            Quaternion.Euler(body.localRotation.x , body.localRotation.y,
                turnInput * bodyTurnAngle * Input.GetAxis("Vertical")),
            Time.deltaTime * bodyTurnRoughness);
        // body.localRotation= Quaternion.Euler(body.localRotation.x , body.localRotation.y,
        //     turnInput * bodyTurnAngle);
        // wheels.ForEach(w => { w.Rotate(5, 0, 0); });
        transform.position = motorRigidbody.transform.position;
        var newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxis("Vertical");

        transform.Rotate(0, newRotation, 0, Space.World);
        // transform.RotateAround(motorRigidbody.transform.position, Vector3.up, newRotation);
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, raycastGroundDistance, maskGroundLayer);
      
        motorRigidbody.drag = isGrounded ? groundDrag : airDrag;
        var targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        transform.rotation =
            Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * groundRotationRoughness);
    }

    private void FixedUpdate()
    {
        if (isGrounded)
            motorRigidbody.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        else
        {
            motorRigidbody.AddForce(transform.up * -30f, ForceMode.Acceleration);
            print(isGrounded);
        }
    }
}