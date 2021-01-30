using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class FishingLine : MonoBehaviour
{
    [SerializeField] private Rigidbody RopeSectionPrefab;

    [SerializeField] private HingeJoint RopeTopJoint;

    [SerializeField] private HingeJoint RopeEnd;

    [SerializeField] private float RopeSectionLength = 1.0f;

    [SerializeField] private int MaxSectionCount = 20;

    private bool FishingLineVerticalButton;

    private List<Rigidbody> RopeSections; // Top section is last in the list.

    private float FishingLineSpeedDown;

    private float TopJointAnchor; // A value of -0.5 means that the section is fully retracted, a value of 0.5 means fully extended.

    private void Awake ()
    {
        RopeSections = new List<Rigidbody>();
        FishingLineSpeedDown = 0.1f;
    }

    private void AddRopeTopSection()
    {
        Rigidbody newTopSection = Instantiate(RopeSectionPrefab, transform.position, transform.rotation);

        if (RopeSections.Count > 0)
        {
            Rigidbody prevTopSection = RopeSections[RopeSections.Count - 1];
            newTopSection.position = prevTopSection.position + Vector3.up * RopeSectionLength;
            HingeJoint joint = newTopSection.gameObject.AddComponent<HingeJoint>();
            joint.axis = Vector3.forward;
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
                RopeEnd.connectedAnchor = Vector3.down * RopeSectionLength * 0.5f;
                RopeEnd.GetComponent<Rigidbody>().position = newTopSection.position + Vector3.down * RopeSectionLength * 0.5f - RopeEnd.transform.localToWorldMatrix.MultiplyPoint(RopeEnd.anchor);
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
        Rigidbody last = RopeSections[RopeSections.Count - 1];
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

    private void FixedUpdate()
    {
        if ( FishingLineVerticalButton )
        {
            FishingLineSpeedDown = Mathf.MoveTowards(FishingLineSpeedDown, -5.0f, 50.0f * Time.fixedDeltaTime);
        }
        else
        {
            FishingLineSpeedDown = Mathf.MoveTowards(FishingLineSpeedDown, 5.0f, 50.0f * Time.fixedDeltaTime);
        }
        if ( RopeSections.Count == 0 )
        {
            AddRopeTopSection();
        }
        else if ( RopeSections.Count > 0 )
        {
            Rigidbody lastSection = RopeSections[RopeSections.Count - 1];
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
