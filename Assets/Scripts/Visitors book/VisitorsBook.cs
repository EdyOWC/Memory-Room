using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;

public class VisitorsBook : MonoBehaviour
{
    [Header("Sheet API")]
    [SerializeField]
    private string apiUrl =
        "https://api.sheetbest.com/sheets/3089755f-e327-4b08-b132-ab2cac79be66";

    [Header("Prefabs & Parents")]
    [SerializeField] private GameObject reviewPrefab; // prefab with NameText + ReviewText + BG
    [SerializeField] private Transform pageParent;    // parent transform for all pages

    [Header("Layout")]
    [SerializeField] private float pageSpacing = 0.05f;   // distance between pages (z stacking)
    [SerializeField] private Vector2 reviewTextOffset = new Vector2(0, 50f); // offset for review
    [SerializeField] private Vector2 nameTextOffset = new Vector2(0, -200f); // offset for name

    void Start()
    {
        StartCoroutine(FetchReviews());
    }

    IEnumerator FetchReviews()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching reviews: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                Debug.Log("Raw JSON: " + json);

                var reviews = JSON.Parse(json);
                if (reviews == null || !reviews.IsArray)
                {
                    Debug.LogError("Invalid JSON received!");
                    yield break;
                }

                // Clear old children
                foreach (Transform child in pageParent)
                {
                    Destroy(child.gameObject);
                }

                int i = 0;
                foreach (var r in reviews.AsArray)
                {
                    string name = r.Value["שם המבקרים"];
                    string review = r.Value["כתבו כאן"];

                    Debug.Log($"Review loaded: {name} - {review}");

                    // Spawn one page per review
                    GameObject page = Instantiate(reviewPrefab, pageParent);

                    // Position as stacked pages
                    page.transform.localPosition = new Vector3(0, 0, -i * pageSpacing);

                    // Find child TMPs
                    TMP_Text[] texts = page.GetComponentsInChildren<TMP_Text>(true);
                    foreach (TMP_Text t in texts)
                    {
                        if (t.name == "NameText")
                        {
                            t.text = name;
                            t.rectTransform.anchoredPosition = nameTextOffset;
                        }
                        if (t.name == "ReviewText")
                        {
                            t.text = review;
                            t.rectTransform.anchoredPosition = reviewTextOffset;
                        }
                    }

                    i++;
                }

                Debug.Log($"Loaded {reviews.Count} reviews into book");
            }
        }
    }
}
