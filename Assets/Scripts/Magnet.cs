using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class Magnet : MonoBehaviour
{
    [SerializeField] private float AttractionRadius = 2.0f;
    [SerializeField] private float AttractionForce = 1.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float ForceLinearity = 0.5f;

    private HingeJoint2D MagnetJoint;

    private Rigidbody2D RB;

    private bool MagnetEnabled;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MagnetEnabled = Input.GetButton("Submit");
    }

    private void FixedUpdate ()
    {
        if ( MagnetEnabled )
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(RB.position, AttractionRadius);
            foreach ( Collider2D collider in colliders )
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
                }
            }
        } else
        {
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
        MagnetJoint = gameObject.AddComponent<HingeJoint2D>();
        ContactPoint2D contact = collision.GetContact(0);
        MagnetJoint.enableCollision = true;
        MagnetJoint.connectedBody = collision.rigidbody;
        MagnetJoint.anchor = transform.worldToLocalMatrix.MultiplyPoint(contact.point);
        MagnetJoint.connectedAnchor = collision.transform.worldToLocalMatrix.MultiplyPoint(contact.point);
    }

    private void Detach()
    {
        if ( MagnetJoint != null )
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
