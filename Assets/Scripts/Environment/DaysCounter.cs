using UnityEngine;
using TMPro;   // TextMeshPro namespace

public class DaysCounter : MonoBehaviour
{
    [Header("TextMeshPro Reference")]
    public TextMeshPro textField;   // use TextMeshPro for 3D world text

    [Header("Reference Date")]
    public int year = 2023;
    public int month = 10;  // October
    public int day = 7;

    void Start()
    {
        UpdateDaysCount();
    }

    void UpdateDaysCount()
    {
        // Reference date
        System.DateTime referenceDate = new System.DateTime(year, month, day);

        // Current date (UTC for consistency)
        System.DateTime today = System.DateTime.UtcNow.Date;

        // Days difference
        int daysPassed = (today - referenceDate).Days;

        // Update text
        if (textField != null)
        {
            textField.text = daysPassed.ToString() + " DAYS";
        }
    }
}
