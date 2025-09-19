using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MultiGrabSceneTrigger : MonoBehaviour
{
    [Header("Notes to watch")]
    public List<XRGrabInteractable> notesToWatch;

    [Header("Scenery to toggle")]
    public List<GameObject> objectsToEnable;
    public List<GameObject> objectsToDisable;

    [Header("Skybox Settings")]
    public Material skyboxToSet;   // assign ONE material in Inspector

    [Header("Settings")]
    public int requiredUniqueGrabs = 3;

    private HashSet<XRGrabInteractable> uniqueGrabs = new HashSet<XRGrabInteractable>();
    private bool sceneryChanged = false;

    private void Awake()
    {
        foreach (var note in notesToWatch)
        {
            if (note == null) continue;
            note.selectEntered.AddListener(OnGrab);
        }
    }

    private void OnDestroy()
    {
        foreach (var note in notesToWatch)
        {
            if (note == null) continue;
            note.selectEntered.RemoveListener(OnGrab);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (sceneryChanged) return;

        var note = args.interactableObject as XRGrabInteractable;
        if (note == null) return;

        // Add this note to the set of unique grabs
        uniqueGrabs.Add(note);

        // If we’ve reached the threshold → commit the scenery change
        if (uniqueGrabs.Count >= requiredUniqueGrabs)
        {
            ChangeScenery();
            sceneryChanged = true;
        }
    }

    private void ChangeScenery()
    {
        // Toggle objects
        foreach (var go in objectsToEnable)
            if (go != null) go.SetActive(true);

        foreach (var go in objectsToDisable)
            if (go != null) go.SetActive(false);

        // Change skybox once
        if (skyboxToSet != null)
        {
            RenderSettings.skybox = skyboxToSet;
            DynamicGI.UpdateEnvironment();
            Debug.Log("Skybox switched to: " + skyboxToSet.name);
        }
    }
}
