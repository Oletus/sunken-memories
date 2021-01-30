using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class BoatController : MonoBehaviour
{
    private float HorizontalInput;

    [SerializeField] private Rigidbody RB;
    [SerializeField] private float HorizontalAcceleration = 1.0f;
    [SerializeField] private float HorizontalMaxSpeed = 5.0f;

    private float HorizontalSpeed;

    private void Update ()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        HorizontalSpeed += HorizontalInput * HorizontalAcceleration * Time.fixedDeltaTime;
        HorizontalSpeed *= 0.99f;
        HorizontalSpeed = Mathf.Clamp(HorizontalSpeed, -HorizontalMaxSpeed, HorizontalMaxSpeed);
        RB.MovePosition(RB.position + HorizontalSpeed * Vector3.right * Time.fixedDeltaTime);
    }
}
