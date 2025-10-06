using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BookController : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Book Parts")]
    public Transform frontCover;
    public Transform coverPivot;
    public Transform pagesParent;
    public Transform pagePivot; // shared hinge for page flips

    [Header("Cover Motion")]
    public Axis coverAxis = Axis.Z;
    public float coverOpenAngle = 140f;
    public bool invertCoverDirection = false;
    public float coverOpenSpeed = 1f;

    [Header("Page Motion")]
    public Axis pageAxis = Axis.Z;
    public float flipAngle = 160f;
    public float pageFlipSpeed = 0.5f;
    public bool invertPageDirection = false;

    [Header("Debug / Desktop")]
    public bool debugMode = true;
    public KeyCode grabTestKey = KeyCode.G;
    public KeyCode flipTestKey = KeyCode.A;
    public KeyCode releaseTestKey = KeyCode.R; // 🔹 press R to simulate release
    public float holdToFlipSeconds = 0.5f;

    private bool coverOpen = false;
    private bool flippingPage = false;
    private int currentPageIndex = 0;
    private float holdTimer = 0f;

    void Update()
    {
        if (debugMode)
        {
            // Simulate Grab (G)
            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
                OnGrabbed();

            // Simulate Page Flip (A)
            bool aHeld = Keyboard.current != null && Keyboard.current.aKey.isPressed;
            if (aHeld)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdToFlipSeconds && !flippingPage)
                {
                    StartCoroutine(FlipPage());
                    holdTimer = 0f;
                }
            }
            else holdTimer = 0f;

            // Simulate Release (R)
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                OnReleased();
        }
    }

    // 🟢 Called when the book is grabbed
    public void OnGrabbed()
    {
        if (!coverOpen)
            StartCoroutine(OpenCover());
    }

    // 🔵 Called when the book is released
    public void OnReleased()
    {
        if (coverOpen)
            StartCoroutine(CloseAndResetBook());
    }

    // 📖 Open the front cover
    private IEnumerator OpenCover()
    {
        coverOpen = true;

        Transform pivot = coverPivot != null ? coverPivot : frontCover;
        Quaternion start = pivot.localRotation;
        Quaternion end = start * Quaternion.AngleAxis(
            (invertCoverDirection ? -1f : 1f) * coverOpenAngle,
            AxisVector(coverAxis)
        );

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / coverOpenSpeed;
            pivot.localRotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }
        pivot.localRotation = end;
    }

    // 📕 Close cover & reset all pages completely
    private IEnumerator CloseAndResetBook()
    {
        coverOpen = false;

        // Wait a short delay before closing
        yield return new WaitForSeconds(0.5f);

        // 1️⃣ Smoothly close the cover
        Transform pivot = coverPivot != null ? coverPivot : frontCover;
        Quaternion start = pivot.localRotation;
        Quaternion end = Quaternion.identity;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / coverOpenSpeed;
            pivot.localRotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }
        pivot.localRotation = end;

        // 2️⃣ Reset all pages to flat
        for (int i = 0; i < pagesParent.childCount; i++)
        {
            Transform page = pagesParent.GetChild(i);
            page.localRotation = Quaternion.identity;
            page.localPosition = Vector3.zero;
        }

        // 3️⃣ Reset page pivot rotation
        if (pagePivot != null)
            pagePivot.localRotation = Quaternion.identity;

        // 4️⃣ Reset page index so we start from beginning again
        currentPageIndex = 0;

        flippingPage = false;
    }

    // 📄 Flip the next page (starts from lowest in hierarchy)
    private IEnumerator FlipPage()
    {
        if (pagesParent == null || pagesParent.childCount == 0 || pagePivot == null)
            yield break;
        if (currentPageIndex >= pagesParent.childCount)
            yield break;

        flippingPage = true;

        int index = pagesParent.childCount - 1 - currentPageIndex;
        Transform page = pagesParent.GetChild(index);

        float rotated = 0f;
        float totalAngle = Mathf.Abs(flipAngle);
        Vector3 axis = AxisVector(pageAxis);
        float direction = invertPageDirection ? -1f : 1f;

        while (rotated < totalAngle)
        {
            float step = (Time.deltaTime / pageFlipSpeed) * totalAngle;
            if (rotated + step > totalAngle)
                step = totalAngle - rotated;

            page.RotateAround(pagePivot.position, pagePivot.TransformDirection(axis), direction * step);

            rotated += step;
            yield return null;
        }

        page.SetSiblingIndex(pagesParent.childCount - 1);
        currentPageIndex++;

        flippingPage = false;
    }

    private static Vector3 AxisVector(Axis a)
    {
        switch (a)
        {
            case Axis.X: return Vector3.right;
            case Axis.Y: return Vector3.up;
            default: return Vector3.forward;
        }
    }
}
