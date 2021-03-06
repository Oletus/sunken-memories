using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using LPUnityUtils;

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

    [SerializeField] private SoundVariants MoveSound;
    [SerializeField] private SoundVariants MoveSound2;
    [SerializeField] private AudioSourcePlayer MoveSoundPlayer;
    [SerializeField] private float MoveSoundVolume = 0.5f;

    private float SoundCurrentStrength;

    private AudioSourcePlayer.PlayingSound PlayingSound1;
    private AudioSourcePlayer.PlayingSound PlayingSound2;

    private float FishingLineVertical;

    private List<Rigidbody2D> RopeSections; // Top section is last in the list.

    private float FishingLineSpeedDown;

    private float TopJointAnchor; // A value of -0.5 means that the section is fully retracted, a value of 0.5 means fully extended.

    private float LowerUntilTime;

    private void Awake ()
    {
        RopeSections = new List<Rigidbody2D>();
        FishingLineSpeedDown = 0.1f;
    }

    private void Start()
    {
        if ( MoveSound != null && MoveSoundPlayer != null )
        {
            PlayingSound1 = MoveSoundPlayer.Play(MoveSound);
        }
        if ( MoveSound2 != null && MoveSoundPlayer != null)
        {
            PlayingSound2 = MoveSoundPlayer.Play(MoveSound2);
        }

        // Lower the fishing line for a few seconds at start.
        LowerUntilTime = Time.time + 1.8f;
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
                RopeEnd.GetComponent<Rigidbody2D>().position =
                    newTopSection.position + Vector2.down * RopeSectionLength * 0.5f -
                    (Vector2)RopeEnd.transform.localToWorldMatrix.MultiplyVector(RopeEnd.anchor);
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
        FishingLineVertical = Time.time < LowerUntilTime ? -1.5f : Input.GetAxis("Vertical");

        if (Time.time > LowerUntilTime + 0.1f)
        {
            SoundCurrentStrength = Mathf.MoveTowards(SoundCurrentStrength, FishingLineSpeedDown / FishingLineMaxSpeed, 1.5f * Time.deltaTime);
        }

        if ( PlayingSound1 != null && PlayingSound1.GetAudioSource() != null)
        {
            AudioSource lineOutSoundSource = PlayingSound1.GetAudioSource();
            lineOutSoundSource.pitch = Mathf.Abs(SoundCurrentStrength) * 0.5f + 0.5f;
            lineOutSoundSource.volume = Mathf.Max(SoundCurrentStrength, 0.0f) * MoveSoundVolume;
        }

        if ( PlayingSound2 != null && PlayingSound2.GetAudioSource() != null )
        {
            AudioSource lineInSoundSource = PlayingSound2.GetAudioSource();
            lineInSoundSource.pitch = Mathf.Abs(SoundCurrentStrength) * 0.5f + 0.5f;
            lineInSoundSource.volume = Mathf.Max(-SoundCurrentStrength, 0.0f) * MoveSoundVolume;
        }
    }

    private void LateUpdate()
    {
        if (LineRenderer == null)
        {
            return;
        }
        List<Vector3> positions = new List<Vector3>();

        positions.Add(RopeEnd.transform.localToWorldMatrix.MultiplyPoint(RopeEnd.anchor));
        for ( int i = 1; i < RopeSections.Count; ++i )
        {
            positions.Add(RopeSections[i].transform.localToWorldMatrix.MultiplyPoint(Vector3.down * RopeSectionLength * 0.5f));
        }
        positions.Add(transform.position);
        LineRenderer.positionCount = positions.Count;
        LineRenderer.SetPositions(positions.ToArray());
    }

    private void FixedUpdate()
    {
        float fishingLineTargetSpeed = -FishingLineMaxSpeed * FishingLineVertical;
        FishingLineSpeedDown = Mathf.MoveTowards(FishingLineSpeedDown, fishingLineTargetSpeed, 50.0f * Time.fixedDeltaTime);
        
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
