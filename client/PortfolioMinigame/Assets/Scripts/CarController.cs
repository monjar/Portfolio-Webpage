using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Range(0.1F, 1000F)] public float forwardSpeed;
    [Range(0.1F, 1000F)] public float reverseSpeed;
    [Range(0.1F, 1000F)] public float turnSpeed;
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
    public Transform frontTransform;
    public Transform backTransform;
    public Animator animator;
    public Joystick rightJoystick;
    public Joystick leftJoystick;
    private float moveInput;
    private float turnInput;
    private bool isGrounded;
    private bool isRearGrounded;
    private bool isFrontGrounded;
    private bool isCollided;
    private Vector3 motorOffset;
    private float touchVertAxis;
    private bool isTouchMovingVert = false;
    private float touchHoriAxis;
    private bool isTouchMovingHori = false;

    private void Start()
    {
        motorRigidbody.transform.parent = null;
        isCollided = false;
    }
    
    private void Update()
    {
        
        var vertAxis = leftJoystick.isActiveAndEnabled ? leftJoystick.Vertical : Input.GetAxisRaw("Vertical");
        var horiAxis = rightJoystick.isActiveAndEnabled ? rightJoystick.Horizontal : Input.GetAxisRaw("Horizontal");
        moveInput = vertAxis;
        turnInput = horiAxis;
        moveInput *= moveInput > 0 ? forwardSpeed : reverseSpeed;
        turningWheels.ForEach(w =>
        {
            w.localRotation = Quaternion.Slerp(w.localRotation,
                Quaternion.Euler(w.localRotation.x, turnInput * wheelTurnAngle,
                    w.localRotation.z),
                Time.deltaTime * wheelTurnRoughness);

        });
        animator.SetFloat("WheelSpeed",
            motorRigidbody.velocity.magnitude / 2 * Vector3.Dot(motorRigidbody.velocity, transform.forward));
        body.localRotation = Quaternion.Lerp(body.localRotation,
            Quaternion.Euler(body.localRotation.x, body.localRotation.y,
                turnInput * bodyTurnAngle * vertAxis),
            Time.deltaTime * bodyTurnRoughness);
 
        transform.position = motorRigidbody.transform.position;
        var newRotation = turnInput * turnSpeed * Time.deltaTime * vertAxis;

        transform.Rotate(0, newRotation, 0, Space.World);
        // RaycastHit hitFront;
        // isFrontGrounded = Physics.Raycast(frontTransform.position, -frontTransform.up, out hitFront,
        //     raycastGroundDistance,
        //     maskGroundLayer);
        //
        // RaycastHit hitRear;
        // isRearGrounded = Physics.Raycast(backTransform.position, -backTransform.up, out hitRear, raycastGroundDistance,
        //     maskGroundLayer);
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, raycastGroundDistance,
            maskGroundLayer);
        // isGrounded = isFrontGrounded || isRearGrounded;
        // motorRigidbody.drag = isFrontGrounded && isRearGrounded ? groundDrag : airDrag;
        motorRigidbody.drag = isGrounded ? groundDrag : airDrag;
        // var targetRotation = Quaternion.FromToRotation(transform.up, hitFront.point.y > hitRear.point.y? hitFront.normal : hitRear.normal) * transform.rotation;
        var targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;


        transform.rotation =
            Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * groundRotationRoughness);
        motorRigidbody.transform.rotation = targetRotation;
    }

    private void FixedUpdate()
    {
        if (isGrounded)
            motorRigidbody.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        else
        {
            motorRigidbody.AddForce(Vector3.up * -30f, ForceMode.Acceleration);
        }
    }
}