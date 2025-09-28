using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawerMissionGate : MonoBehaviour
{
    [Header("Drawer Setup")]
    public XRGrabInteractable drawerGrab;
    public bool drawerLockedAtStart = true;
    public Rigidbody drawerRb;

    [Header("Will Setup")]
    public XRGrabInteractable willGrabObject;

    [Header("Relock Setup")]
    public Collider relockTrigger;
    public Collider drawerBackMarker;

    [Header("Mission Setup")]
    public List<string> missions = new List<string> { "A", "B", "C" };
    private HashSet<string> completedMissions = new HashSet<string>();

    [Header("Feedback")]
    public AudioSource sfxSource;
    public AudioClip stuckDrawerSFX;
    public AudioSource dialogSource;
    public AudioClip stuckDialogClip;

    [Header("Music Control")]
    public AudioSource musicSource;
    [Range(0f, 1f)] public float dimmedVolume = 0.3f;
    public float fadeSpeed = 2f;

    [Header("Peek Movement")]
    public Transform moveTarget;
    public Vector3 startPos;
    public Vector3 endPos;
    public float moveDuration = 1.5f;

    [Header("Animation Setup")]
    public Animator animator;
    public string relockTriggerName = "Dark";

    [Header("Fog Setup")]
    public bool changeFogOnRelock = true;
    public bool fogEnabledOnRelock = false;
    public bool revertFogOnUnlock = false;
    private bool originalFogState;

    private bool dialogPlayed = false;
    private bool isUnlocked = false;
    private bool isRelocked = false;
    private bool isMoving = false;
    private float moveTimer = 0f;

    private float musicTargetVolume;
    private float musicOriginalVolume;

    void Start()
    {
        if (drawerGrab == null)
            drawerGrab = GetComponent<XRGrabInteractable>();

        if (drawerRb == null)
            drawerRb = GetComponent<Rigidbody>();

        if (drawerLockedAtStart)
            LockDrawer();

        if (drawerGrab != null)
            drawerGrab.selectEntered.AddListener(_ => OnGrabAttempt());

        if (willGrabObject != null)
            willGrabObject.enabled = false;

        if (moveTarget != null)
            moveTarget.localPosition = startPos;

        originalFogState = RenderSettings.fog;

        if (musicSource != null)
        {
            musicOriginalVolume = musicSource.volume;
            musicTargetVolume = musicOriginalVolume;
        }
    }

    private void OnGrabAttempt()
    {
        if (!isUnlocked)
        {
            if (sfxSource != null && stuckDrawerSFX != null)
                sfxSource.PlayOneShot(stuckDrawerSFX);

            if (!dialogPlayed && stuckDialogClip != null)
            {
                if (dialogSource != null && !dialogSource.isPlaying)
                {
                    dialogSource.PlayOneShot(stuckDialogClip);
                    dialogPlayed = true;
                }
                else
                {
                    Debug.Log("Stuck dialog skipped (source busy).");
                }
            }
        }
    }

    private void LockDrawer()
    {
        if (drawerRb != null)
            drawerRb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void UnlockDrawer()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        isRelocked = false;

        if (drawerRb != null)
        {
            drawerRb.constraints = RigidbodyConstraints.FreezePositionX |
                                   RigidbodyConstraints.FreezePositionY |
                                   RigidbodyConstraints.FreezeRotation;
        }

        Debug.Log("✅ All missions completed — Drawer unlocked!");
        StartPeekMove();

        if (revertFogOnUnlock)
            RenderSettings.fog = originalFogState;
    }

    private void RelockDrawer()
    {
        if (!isUnlocked) return;

        LockDrawer();
        isRelocked = true;

        if (willGrabObject != null)
            willGrabObject.enabled = true;

        if (animator != null && !string.IsNullOrEmpty(relockTriggerName))
            animator.SetTrigger(relockTriggerName);

        if (changeFogOnRelock)
            RenderSettings.fog = fogEnabledOnRelock;

        // Dim music on relock
        if (musicSource != null)
            musicTargetVolume = dimmedVolume;

        Debug.Log("🔒 Drawer re-locked — DARKER animation + Fog updated, Will enabled + Music dimmed!");
    }

    private void StartPeekMove()
    {
        if (moveTarget != null)
        {
            moveTimer = 0f;
            isMoving = true;
        }
    }

    public void CompleteMission(string missionName)
    {
        if (!missions.Contains(missionName))
        {
            Debug.LogWarning($"Mission {missionName} is not part of this drawer’s requirements.");
            return;
        }

        if (completedMissions.Add(missionName))
        {
            Debug.Log($"Mission {missionName} completed!");
            if (completedMissions.Count == missions.Count)
                UnlockDrawer();
        }
    }

    void Update()
    {
        // Peek movement
        if (isMoving && moveTarget != null)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);
            float easedT = Mathf.SmoothStep(0, 1, t);
            moveTarget.localPosition = Vector3.Lerp(startPos, endPos, easedT);

            if (t >= 1f)
                isMoving = false;
        }

        // Smooth music fade
        if (musicSource != null && !Mathf.Approximately(musicSource.volume, musicTargetVolume))
        {
            musicSource.volume = Mathf.MoveTowards(musicSource.volume, musicTargetVolume, fadeSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (drawerBackMarker != null && relockTrigger != null)
        {
            if (other == relockTrigger)
            {
                RelockDrawer();
            }
        }
    }
}
