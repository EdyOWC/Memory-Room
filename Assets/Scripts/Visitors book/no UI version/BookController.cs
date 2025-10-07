using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // for XR controller inputs

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

    [Header("UI Control")]
    public GameObject bookUI; // 🔹 assign your UI GO here (Canvas or any GO)

    private bool coverOpen = false;
    private bool flippingPage = false;
    private int currentPageIndex = 0;

    private InputAction flipAction;

    void OnEnable()
    {
        // Bind A button (primary button on Oculus/Quest right controller)
        flipAction = new InputAction("FlipPage", binding: "<XRController>{RightHand}/primaryButton");
        flipAction.performed += ctx => TryFlipPage();
        flipAction.Enable();
    }

    void OnDisable()
    {
        flipAction?.Disable();
    }

    // 🟢 Called when the book is grabbed
    public void OnGrabbed()
    {
        if (bookUI != null)
            bookUI.SetActive(true);

        if (!coverOpen)
            StartCoroutine(OpenCover());
    }

    // 🔵 Called when the book is released
    public void OnReleased()
    {
        if (bookUI != null)
            bookUI.SetActive(false);

        if (coverOpen)
            StartCoroutine(CloseAndResetBook());
    }

    private void TryFlipPage()
    {
        if (coverOpen && !flippingPage)
            StartCoroutine(FlipPage());
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

        yield return new WaitForSeconds(0.5f);

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

        // Reset all pages
        for (int i = 0; i < pagesParent.childCount; i++)
        {
            Transform page = pagesParent.GetChild(i);
            page.localRotation = Quaternion.identity;
            page.localPosition = Vector3.zero;
        }

        if (pagePivot != null)
            pagePivot.localRotation = Quaternion.identity;

        currentPageIndex = 0;
        flippingPage = false;
    }

    // 📄 Flip the next page
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
