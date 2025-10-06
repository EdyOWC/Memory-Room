using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;

public class VisitorsBook2 : MonoBehaviour
{
    [Header("Sheet API")]
    [SerializeField]
    private string apiUrl =
        "https://api.sheetbest.com/sheets/3089755f-e327-4b08-b132-ab2cac79be66";

    [Header("Prefabs & Parents")]
    [SerializeField] private GameObject reviewPrefab; // prefab with NameText + ReviewText (3D TextMeshPro)
    [SerializeField] private Transform pageParent;    // where the pages will be instantiated

    [Header("Layout")]
    [SerializeField] private float pageSpacing = 0.05f;   // z distance between stacked pages
    [SerializeField] private bool addSpacesBetweenLetters = true; // enable or disable spacing fix

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
                yield break;
            }

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

                if (addSpacesBetweenLetters)
                {
                    name = AddSpacesBetweenCharacters(name);
                    review = AddSpacesBetweenCharacters(review);
                }

                Debug.Log($"Review loaded: {name} - {review}");

                // Spawn one page per review
                GameObject page = Instantiate(reviewPrefab, pageParent);

                // Position as stacked pages in world
                page.transform.localPosition = new Vector3(0, 0, -i * pageSpacing);

                // Find child TMPs (3D TextMeshPro, not UI!)
                TMP_Text[] texts = page.GetComponentsInChildren<TMP_Text>(true);
                foreach (TMP_Text t in texts)
                {
                    if (t.name == "NameText")
                        t.text = name;
                    else if (t.name == "ReviewText")
                        t.text = review;
                }

                i++;
            }

            Debug.Log($"Loaded {reviews.Count} reviews into book");
        }
    }

    // ✨ Adds a space after each character (except the last one)
    private string AddSpacesBetweenCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        System.Text.StringBuilder spaced = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            spaced.Append(input[i]);
            if (i < input.Length - 1)
                spaced.Append(' '); // add a space after each char
        }
        return spaced.ToString();
    }
}
