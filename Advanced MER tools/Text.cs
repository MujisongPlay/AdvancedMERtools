using System.Collections.Generic;
using UnityEngine;

public class Text : MonoBehaviour
{
    [Tooltip("The input string to display.")]
    public string input;

    [Tooltip("The distance between each character.")]
    public Vector3 distanceBetweenChars;

    [Tooltip("The length of the spacebar.")]
    public Vector3 spacebarLength;

    [Tooltip("Insert 26 letters to this. Ensure that the object name matches the character. For example, 'A' should match the GameObject named 'A'.")]
    public List<GameObject> chars = new List<GameObject>();

    private List<GameObject> duplicatedChars = new List<GameObject>();

    private string previousInput;
    private Vector3 previousDistanceBetweenChars;
    private Vector3 previousSpacebarLength;

    public void AddLettersFromFolder()
    {
#if UNITY_EDITOR
        string path = "Assets/Blocks/Letters/European";

        if (System.IO.Directory.Exists(path))
        {
            foreach (string file in System.IO.Directory.GetFiles(path, "*.prefab"))
            {
                GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(file);
                if (prefab != null && !chars.Contains(prefab))
                {
                    chars.Add(prefab);
                }
            }

            Debug.Log("Letters added from Assets/Blocks/Letters/European");
        }
        else
        {
            Debug.LogWarning("Folder Assets/Blocks/Letters/European not found. Please add letters manually to the chars list.");
        }
#endif
    }

    public void AddNumbersFromFolder()
    {
#if UNITY_EDITOR
        string path = "Assets/Blocks/Letters/Numbers";

        if (System.IO.Directory.Exists(path))
        {
            foreach (string file in System.IO.Directory.GetFiles(path, "*.prefab"))
            {
                GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(file);
                if (prefab != null && !chars.Contains(prefab))
                {
                    chars.Add(prefab);
                }
            }

            Debug.Log("Numbers added from Assets/Blocks/Letters/Numbers");
        }
        else
        {
            Debug.LogWarning("Folder Assets/Blocks/Letters/Numbers not found. Please add numbers manually to the chars list.");
        }
#endif
    }

    private void OnDrawGizmos()
    {
        if (string.IsNullOrEmpty(input) || (input == previousInput && distanceBetweenChars == previousDistanceBetweenChars && spacebarLength == previousSpacebarLength))
        {
            return;
        }

        foreach (var charObject in duplicatedChars)
        {
            DestroyImmediate(charObject);
        }
        duplicatedChars.Clear();

        Vector3 currentPosition = Vector3.zero;

        foreach (char character in input)
        {
            GameObject charPrefab = chars.Find(x => x.name.Equals(character.ToString(), System.StringComparison.OrdinalIgnoreCase));

            if (charPrefab == null)
            {
                currentPosition += spacebarLength;
                continue;
            }

            GameObject instantiatedChar = Instantiate(charPrefab, this.transform);
            instantiatedChar.transform.localEulerAngles = Vector3.zero;
            instantiatedChar.transform.localScale = Vector3.one;
            instantiatedChar.transform.localPosition = currentPosition;

            currentPosition += distanceBetweenChars;
            duplicatedChars.Add(instantiatedChar);
        }

        previousInput = input;
        previousDistanceBetweenChars = distanceBetweenChars;
        previousSpacebarLength = spacebarLength;
    }
}
