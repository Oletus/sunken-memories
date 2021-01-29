using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class FishingLine : MonoBehaviour
{
    [SerializeField] private Rigidbody2D RopeSectionPrefab;

    [SerializeField] private float RopeSectionLength = 1.0f;

    [SerializeField] private int MaxSectionCount = 20;

    private bool FishingLineVerticalButton;

    private List<Rigidbody2D> RopeSections; // Top section is last in the list.

    private float FishingLineSpeedDown;

    private void Awake ()
    {
        RopeSections = new List<Rigidbody2D>();
        FishingLineSpeedDown = 0.1f;
    }

    private void AddRopeTopSection()
    {
        Rigidbody2D newTopSection = Instantiate(RopeSectionPrefab, transform.position, transform.rotation);
        newTopSection.isKinematic = true;

        if ( RopeSections.Count > 0 )
        {
            Rigidbody2D prevTopSection = RopeSections[RopeSections.Count - 1];
            prevTopSection.isKinematic = false;
            newTopSection.position = prevTopSection.position + Vector2.up * RopeSectionLength;
            HingeJoint2D joint = newTopSection.gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = prevTopSection;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector2.down * RopeSectionLength * 0.5f;
            joint.connectedAnchor = Vector2.up * RopeSectionLength * 0.5f;
            joint.enabled = true;
        }

        RopeSections.Add(newTopSection);
    }

    private void RemoveRopeTopSection()
    {
        if ( RopeSections.Count == 0 )
        {
            return;
        }
        Rigidbody2D last = RopeSections[RopeSections.Count - 1];
        Destroy(last.gameObject);
        RopeSections.RemoveAt(RopeSections.Count - 1);
        if ( RopeSections.Count > 0 )
        {
            RopeSections[RopeSections.Count - 1].isKinematic = true;
        }
    }

    private void Update ()
    {
        FishingLineVerticalButton = Input.GetButton("FishingLineVertical");
    }

    private void FixedUpdate()
    {
        if ( FishingLineVerticalButton )
        {
            FishingLineSpeedDown = Mathf.MoveTowards(FishingLineSpeedDown, -0.1f, Time.fixedDeltaTime);
        }
        else
        {
            FishingLineSpeedDown = Mathf.MoveTowards(FishingLineSpeedDown, 0.1f, Time.fixedDeltaTime);
        }
        if ( RopeSections.Count == 0 )
        {
            AddRopeTopSection();
        }
        else if ( RopeSections.Count > 0 )
        {
            Rigidbody2D lastSection = RopeSections[RopeSections.Count - 1];
            float useSpeedDown = FishingLineSpeedDown;
            if ( RopeSections.Count >= MaxSectionCount )
            {
                useSpeedDown = Mathf.Min(FishingLineSpeedDown, 0.0f);
            }
            if ( RopeSections.Count == 1 )
            {
                useSpeedDown = Mathf.Max(FishingLineSpeedDown, 0.0f);
            }
            lastSection.MovePosition(lastSection.position + Vector2.down * useSpeedDown);
            if (lastSection.position.y < transform.position.y - RopeSectionLength * 0.5f)
            {
                AddRopeTopSection();
            }
            if ( lastSection.position.y > transform.position.y + RopeSectionLength * 0.6f )
            {
                RemoveRopeTopSection();
            }
        }
    }
}
