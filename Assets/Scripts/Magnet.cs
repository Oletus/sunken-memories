using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class Magnet : MonoBehaviour
{
    private HingeJoint2D MagnetJoint;

    private void FixedUpdate ()
    {
        // TODO: Add magnetic force
    }

    public void RemoveMagnetic(Magnetic magnetic)
    {
        Destroy(magnetic.gameObject);
        Destroy(MagnetJoint);
        MagnetJoint = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Magnetic magnetic = collision.gameObject.GetComponent<Magnetic>();
        if ( magnetic && magnetic.IsMagnetic )
        {
            Attach(collision, magnetic);
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
}
