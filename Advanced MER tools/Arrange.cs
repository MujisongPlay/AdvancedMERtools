using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Arrange : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (Count <= 0) return;
        arranges.AddRange(this.GetComponentsInChildren<Arrange>());
        List<GameObject> Registed = new List<GameObject> { };
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject game = this.transform.GetChild(i).gameObject;
            if (arranges.TrueForAll(x => !x.Duplicated.Contains(game)))
            {
                Registed.Add(game);
            }
        }
        Duplicated.ForEach(x => DestroyImmediate(x));
        Duplicated.Clear();
        Vector3 pos = Vector3.zero;
        Vector3 rot = Vector3.zero;
        Vector3 scl = Vector3.one;
        for (int i = 0; i < Count; i++)
        {
            pos += ConstantOffset + Vector3.Scale(RelativeOffset, pos == Vector3.zero ? Vector3.one : pos);
            rot += ConstantRotation + Vector3.Scale(RelativeRotation, rot);
            scl += ConstantScale + Vector3.Scale(RelativeScale, scl);
            GameObject copy = GameObject.Instantiate(this.gameObject);
            DestroyImmediate(copy.GetComponent<Arrange>());
            if (this.transform.root != this.transform) copy.transform.parent = this.transform.parent;
            copy.transform.localPosition = this.transform.localPosition + pos;
            copy.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles + rot);
            copy.transform.localScale = Vector3.Scale(this.transform.localScale, scl);
            Duplicated.Add(copy);
        }
    }

    public Vector3 RelativeOffset;
    public Vector3 ConstantOffset;
    public Vector3 RelativeRotation;
    public Vector3 ConstantRotation;
    public Vector3 RelativeScale;
    public Vector3 ConstantScale;
    public int Count;

    List<GameObject> Duplicated = new List<GameObject> { };
    List<Arrange> arranges = new List<Arrange> { };
}
