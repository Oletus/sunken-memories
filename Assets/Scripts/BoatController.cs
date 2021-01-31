using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using LPUnityUtils;

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

    [SerializeField] private Transform CameraReference;

    [SerializeField] private SoundVariants MoveSound;
    [SerializeField] private AudioSourcePlayer MoveSoundPlayer;
    [SerializeField] private float BoatMoveSoundVolume = 0.5f;

    private float AngularVelocity;
    private float TiltAngle;

    private float HorizontalSpeed;

    private float BoatSoundCurrentStrength;

    private AudioSourcePlayer.PlayingSound PlayingBoatSound;

    private void Awake()
    {
        FishingLineJoint.anchor = RB.transform.worldToLocalMatrix.MultiplyPoint(FishingLine.transform.position);
        if ( MoveSoundPlayer != null )
        {
            PlayingBoatSound = MoveSoundPlayer.Play(MoveSound);
        }
    }

    private void Update()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        AngularVelocity = 0.0f;
        TiltAngle = 0.0f;

        if ( CameraReference != null )
        {
            CameraReference.position = RB.position;
        }

        if ( PlayingBoatSound != null && PlayingBoatSound.GetAudioSource() != null )
        {
            AudioSource boatSoundSource = PlayingBoatSound.GetAudioSource();
            BoatSoundCurrentStrength = Mathf.MoveTowards(BoatSoundCurrentStrength, Mathf.Abs(HorizontalInput), 0.5f * Time.deltaTime);
            boatSoundSource.pitch = BoatSoundCurrentStrength * 0.5f + 0.5f;
            boatSoundSource.volume = BoatSoundCurrentStrength * BoatMoveSoundVolume;
        }
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
        RB.rotation = TiltAngle;
    }
}
