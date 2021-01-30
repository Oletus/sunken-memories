using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class BoatController : MonoBehaviour
{
    private float HorizontalInput;

    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private HingeJoint2D FishingLineJoint;
    [SerializeField] private Rigidbody2D FishingLine;

    [SerializeField] private float HorizontalAcceleration = 1.0f;
    [SerializeField] private float HorizontalMaxSpeed = 5.0f;

    [SerializeField] private float TiltAcceleration = 1.0f;
    [SerializeField] private float TiltSpringConstant = 1.0f;

    private float AngularVelocity;
    private float TiltAngle;

    private float HorizontalSpeed;

    private void Awake()
    {
        FishingLineJoint.anchor = RB.transform.worldToLocalMatrix.MultiplyPoint(FishingLine.transform.position);
    }

    private void Update()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        AngularVelocity = 0.0f;
        TiltAngle = 0.0f;
    }

    private void FixedUpdate()
    {
        AngularVelocity += (HorizontalInput + Mathf.Sin(Time.fixedTime) * 0.3f) * TiltAcceleration * Time.fixedDeltaTime;
        AngularVelocity -= TiltAngle * TiltSpringConstant * Time.fixedDeltaTime;
        AngularVelocity = AngularVelocity * 0.99f;
        TiltAngle += AngularVelocity * Time.fixedDeltaTime;

        HorizontalSpeed += HorizontalInput * HorizontalAcceleration * Time.fixedDeltaTime;
        HorizontalSpeed *= 0.99f;
        HorizontalSpeed = Mathf.Clamp(HorizontalSpeed, -HorizontalMaxSpeed, HorizontalMaxSpeed);
        RB.MovePosition(RB.position + HorizontalSpeed * Vector2.right * Time.fixedDeltaTime);
        RB.MoveRotation(TiltAngle);
    }
}
