using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Arrange : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (Count <= 0 /*|| (UseCurve && BezierCurve == null)*/) return;
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
        float Time = 0;
        for (int i = 0; i < Count; i++)
        {
            if (!UseCurve)
            {
                pos += ConstantOffset + Vector3.Scale(RelativeOffset, pos == Vector3.zero ? Vector3.one : pos);
                rot += ConstantRotation + Vector3.Scale(RelativeRotation, rot);
            }
            else
            {
                //Vector3 v1 = BezierCurve.GetPointAt(Time);
                //Vector3 v2 = BezierCurve.GetPointAt(Time + (Time > 0 ? -CurveIntegrationAccuracity : CurveIntegrationAccuracity));
                //Quaternion q = Quaternion.LookRotation(v1 - v2, Vector3.up);
                //pos = BezierCurve.GetPointAt(Time) + ConstantOffset + q * RelativeOffset;
                //rot = q.eulerAngles + ConstantRotation + Vector3.Scale(q.eulerAngles, RelativeRotation);
                //Time += 1f / (float)Count;
            }
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
    public bool UseCurve;
    //public BezierCurve BezierCurve;
    public float CurveIntegrationAccuracity = 0.01f;

    List<GameObject> Duplicated = new List<GameObject> { };
    List<Arrange> arranges = new List<Arrange> { };
}
