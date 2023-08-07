using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text : MonoBehaviour
{
    public string Input;
    public Vector3 DistanceBetweenChar;
    public Vector3 SpacebarLength;

    [Tooltip("Insert 26 letters to this. And becareful: object name is calling. 'A' -> call gameobject that have A, next a, lastly ' '.")]
    public List<GameObject> Chars;

    private void OnDrawGizmos()
    {
        if (Input == "" || (Input == previous && DistanceBetweenChar == p1 && SpacebarLength == p2)) return;
        Vector3 vector = Vector3.zero;
        duplicated.ForEach(x => DestroyImmediate(x));
        for (int i = 0; i < Input.Length; i++)
        {
            GameObject game = Chars.Find(x => x.name.Equals(Input[i].ToString(), System.StringComparison.Ordinal));
            if (game == null)
            {
                game = Chars.Find(x => x.name.Equals(Input[i].ToString(), System.StringComparison.OrdinalIgnoreCase));
                if (game == null)
                {
                    vector += SpacebarLength;
                    continue;
                }
            }
            GameObject game1 = GameObject.Instantiate(game, this.transform);
            game1.transform.localEulerAngles = Vector3.zero;
            game1.transform.localScale = Vector3.one;
            game1.transform.localPosition = vector;
            vector += DistanceBetweenChar;
            duplicated.Add(game1);
        }
        previous = Input;
        p1 = DistanceBetweenChar;
        p2 = SpacebarLength;
    }

    string previous;
    Vector3 p1;
    Vector3 p2;

    List<GameObject> duplicated = new List<GameObject> { };
}
