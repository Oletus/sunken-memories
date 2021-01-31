using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using LPUnityUtils;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class Magnet : MonoBehaviour
{
    [SerializeField] private float AttractionRadius = 2.0f;
    [SerializeField] private float AttractionForce = 1.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float ForceLinearity = 0.5f;
    [SerializeField] public ParticleSystem MagnetParticle;

    [SerializeField] private SoundVariants MagnetPingSound;
    [SerializeField] private SoundVariants MagnetAttachSound;
    [SerializeField] private float PingSoundInterval = 1.0f;

    private float NearestDistance;

    private float NextPingTime;
    private bool MagnetAttachSoundScheduled;

    private HingeJoint2D MagnetJoint;

    private Rigidbody2D RB;

    private bool MagnetEnabled;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        NearestDistance = 1.0f;
    }

    private void Update()
    {
        bool wasMagnetEnabled = MagnetEnabled;
        MagnetEnabled = Input.GetButton("Submit");
        if ( !wasMagnetEnabled && MagnetEnabled )
        {
            AudioSourcePlayer.GlobalPlayer.Play(MagnetPingSound);
            NextPingTime = Time.time + PingSoundInterval;
        }
        /*if ( MagnetEnabled && Time.time > NextPingTime)
        {
            NextPingTime = Mathf.Max(Time.time - Time.deltaTime, NextPingTime) + PingSoundInterval;
            Debug.Log("Play ping " + (1.0f - NearestDistance));
            AudioSourcePlayer.GlobalPlayer.Play(MagnetPingSound, -1, 1.0f - NearestDistance);
        }*/

        if (MagnetAttachSoundScheduled)
        {
            AudioSourcePlayer.GlobalPlayer.Play(MagnetAttachSound);
            MagnetAttachSoundScheduled = false;
        }
    }

    private void FixedUpdate ()
    {
        if ( MagnetEnabled )
        {
            var em = MagnetParticle.emission;
            em.enabled = true;
            if ( MagnetJoint == null )
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(RB.position, AttractionRadius);
                foreach (Collider2D collider in colliders)
                {
                    Magnetic magnetic = collider.GetComponent<Magnetic>();
                    if (magnetic != null && collider.attachedRigidbody != null)
                    {
                        Vector2 magnetBottomCenter = transform.localToWorldMatrix.MultiplyPoint(Vector2.down * 0.3f);
                        Vector2 colliderPos = collider.ClosestPoint(magnetBottomCenter);
                        Vector2 towardsMagnet = (magnetBottomCenter - colliderPos);
                        float distance = towardsMagnet.magnitude;
                        float distanceMult = Mathf.Lerp(1.0f / ((distance / AttractionRadius) + 0.01f), (1.0f - distance / AttractionRadius), ForceLinearity);
                        collider.attachedRigidbody.AddForce(AttractionForce * towardsMagnet * distanceMult, ForceMode2D.Force);
                        RB.AddForceAtPosition(AttractionForce * -towardsMagnet * distanceMult, magnetBottomCenter);
                        NearestDistance = Mathf.Min(NearestDistance, distance / AttractionRadius);
                    }
                }
            }
            else
            {
                NearestDistance = 1.0f;
            }
        }
        else
        {
            var em = MagnetParticle.emission;
            em.enabled = false;
            Detach();
        }
    }

    public void RemoveMagnetic(Magnetic magnetic)
    {
        if ( Detach(magnetic.gameObject) )
        {
            Destroy(magnetic.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( MagnetEnabled )
        {
            Magnetic magnetic = collision.gameObject.GetComponent<Magnetic>();
            if (magnetic && magnetic.IsMagnetic)
            {
                Attach(collision, magnetic);
            }
        }
    }

    private void Attach(Collision2D collision, Magnetic magnetic)
    {
        if ( MagnetJoint != null )
        {
            return;
        }
        MagnetAttachSoundScheduled = true;
        MagnetJoint = gameObject.AddComponent<HingeJoint2D>();
        ContactPoint2D contact = collision.GetContact(0);
        MagnetJoint.enableCollision = true;
        MagnetJoint.connectedBody = collision.rigidbody;
        MagnetJoint.anchor = transform.worldToLocalMatrix.MultiplyPoint(contact.point);
        MagnetJoint.connectedAnchor = collision.transform.worldToLocalMatrix.MultiplyPoint(contact.point);
    }

    private void Detach()
    {
        if ( MagnetJoint != null && MagnetJoint.connectedBody != null )
        {
            Detach(MagnetJoint.connectedBody.gameObject);
        }
    }

    private bool Detach(GameObject magnetic)
    {
        if ( MagnetJoint.connectedBody.gameObject != magnetic )
        {
            return false;
        }
        Destroy(MagnetJoint);
        MagnetJoint = null;
        return true;
    }
}
