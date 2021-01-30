using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class FishingLine : MonoBehaviour
{
    [SerializeField] private Rigidbody2D RopeSectionPrefab;

    [SerializeField] private HingeJoint2D RopeTopJoint;

    [SerializeField] private HingeJoint2D RopeEnd;

    [SerializeField] private float RopeSectionLength = 1.0f;

    [SerializeField] private int MaxSectionCount = 20;

    [SerializeField] private float FishingLineMaxSpeed = 3.0f;

    [SerializeField] private LineRenderer LineRenderer;

    private bool FishingLineVerticalButton;

    private List<Rigidbody2D> RopeSections; // Top section is last in the list.

    private float FishingLineSpeedDown;

    private float TopJointAnchor; // A value of -0.5 means that the section is fully retracted, a value of 0.5 means fully extended.

    private void Awake ()
    {
        RopeSections = new List<Rigidbody2D>();
        FishingLineSpeedDown = 0.1f;
    }

    private void AddRopeTopSection()
    {
        Rigidbody2D newTopSection = Instantiate(RopeSectionPrefab, transform.position, transform.rotation);

        if (RopeSections.Count > 0)
        {
            Rigidbody2D prevTopSection = RopeSections[RopeSections.Count - 1];
            newTopSection.position = prevTopSection.position + Vector2.up * RopeSectionLength;
            HingeJoint2D joint = newTopSection.gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = prevTopSection;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.down * RopeSectionLength * 0.5f;
            joint.connectedAnchor = Vector3.up * RopeSectionLength * 0.5f;
            TopJointAnchor -= 1.0f;
        }
        else
        {
            TopJointAnchor = -RopeSectionLength * 0.4999f;
            if (RopeEnd != null)
            {
                RopeEnd.connectedBody = newTopSection;
                RopeEnd.connectedAnchor = Vector2.down * RopeSectionLength * 0.5f;
                RopeEnd.GetComponent<Rigidbody2D>().position = newTopSection.position + Vector2.down * RopeSectionLength * 0.5f - (Vector2)RopeEnd.transform.localToWorldMatrix.MultiplyPoint(RopeEnd.anchor);
            }
        }
        RopeTopJoint.connectedBody = newTopSection;
        RopeTopJoint.connectedAnchor = Vector3.up * TopJointAnchor;

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

        TopJointAnchor += 1.0f;

        if (RopeSections.Count > 0)
        {
            RopeTopJoint.connectedBody = RopeSections[RopeSections.Count - 1];
            RopeTopJoint.connectedAnchor = Vector3.up * TopJointAnchor;
        }
    }

    private void Update ()
    {
        FishingLineVerticalButton = Input.GetButton("Vertical");
    }

    private void LateUpdate()
    {
        if (LineRenderer == null)
        {
            return;
        }
        List<Vector3> positions = new List<Vector3>();
        for ( int i = 0; i < RopeSections.Count; ++i )
        {
            positions.Add(RopeSections[i].transform.localToWorldMatrix.MultiplyPoint(Vector3.down * RopeSectionLength * 0.5f));
        }
        positions.Add(transform.position);
        LineRenderer.positionCount = positions.Count;
        LineRenderer.SetPositions(positions.ToArray());
    }

    private void FixedUpdate()
    {
        if ( FishingLineVerticalButton )
        {
            FishingLineSpeedDown = Mathf.MoveTowards(FishingLineSpeedDown, -FishingLineMaxSpeed, 50.0f * Time.fixedDeltaTime);
        }
        else
        {
            FishingLineSpeedDown = Mathf.MoveTowards(FishingLineSpeedDown, FishingLineMaxSpeed, 50.0f * Time.fixedDeltaTime);
        }
        if ( RopeSections.Count == 0 )
        {
            AddRopeTopSection();
        }
        else if ( RopeSections.Count > 0 )
        {
            Rigidbody2D lastSection = RopeSections[RopeSections.Count - 1];
            float useSpeedDown = FishingLineSpeedDown;
            TopJointAnchor += useSpeedDown * Time.fixedDeltaTime;
            if ( RopeSections.Count == 1 )
            {
                TopJointAnchor = Mathf.Max(TopJointAnchor, -0.5f);
            }
            if ( RopeSections.Count >= MaxSectionCount )
            {
                TopJointAnchor = Mathf.Min(TopJointAnchor, 0.5f);
            }
            if (TopJointAnchor > RopeSectionLength * 0.51f)
            {
                AddRopeTopSection();
            }
            else if (TopJointAnchor < -RopeSectionLength * 0.51f)
            {
                RemoveRopeTopSection();
            }
            RopeTopJoint.connectedAnchor = Vector3.up * TopJointAnchor;
        }
    }
}
