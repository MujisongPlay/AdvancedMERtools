using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;

public class MirrorObject : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (ApplyMirror)
        {
            Duplicated.Clear();
            x = false;
            y = false;
            z = false;
            ApplyMirror = false;
        }
        if (!x & !y & !z) return;
        mirrors.AddRange(this.GetComponentsInChildren<MirrorObject>());
        List<GameObject> Registed = new List<GameObject> { };
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject game = this.transform.GetChild(i).gameObject;
            if (mirrors.TrueForAll(x => !x.Duplicated.Contains(game)))
            {
                Registed.Add(game);
            }
        }
        Vector3 vector = Vector3.one;
        Duplicated.ForEach(x => DestroyImmediate(x));
        Duplicated.Clear();
        if (x) vector.x = -1;
        if (y) vector.y = -1;
        if (z) vector.z = -1;
        foreach (GameObject game in Registed)
        {
            GameObject copy = GameObject.Instantiate(game, this.transform);
            Vector3 v = copy.transform.localPosition;
            copy.transform.localPosition = new Vector3(v.x * vector.x, v.y * vector.y, v.z * vector.z);
            v = copy.transform.localEulerAngles;
            copy.transform.localEulerAngles = new Vector3(v.x * vector.z * vector.y, v.y * vector.z * vector.x, v.z * vector.x * vector.y);
            copy.transform.localScale = game.transform.localScale;
            Duplicated.Add(copy);
        }
    }

    public bool x;
    public bool y;
    public bool z;
    public bool ApplyMirror;

    List<GameObject> Duplicated = new List<GameObject> { };
    List<MirrorObject> mirrors = new List<MirrorObject> { };
}
