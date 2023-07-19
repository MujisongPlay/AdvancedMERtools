using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (Count <= 1 || Angle == Vector3.zero) return;
        rotators.AddRange(this.GetComponentsInChildren<Rotator>());
        List<GameObject> Registed = new List<GameObject> { };
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject game = this.transform.GetChild(i).gameObject;
            if (rotators.TrueForAll(x => !x.Duplicated.Contains(game)))
            {
                Registed.Add(game);
            }
        }
        Duplicated.ForEach(x => DestroyImmediate(x));
        Duplicated.Clear();
        Vector3 offset = Angle / Count;
        for (int i = 1; i < Count; i++)
        {
            GameObject copy = GameObject.Instantiate(this.gameObject);
            DestroyImmediate(copy.GetComponent<Rotator>());
            if (this.transform.root != this.transform) copy.transform.parent = this.transform.parent;
            copy.transform.localPosition = this.transform.localPosition;
            copy.transform.localEulerAngles = this.transform.localEulerAngles + offset;
            copy.transform.localScale = this.transform.localScale;
            offset += Angle / Count;
            Duplicated.Add(copy);
        }
    }

    public Vector3 Angle;
    public int Count;

    List<GameObject> Duplicated = new List<GameObject> { };
    List<Rotator> rotators = new List<Rotator> { };
}
