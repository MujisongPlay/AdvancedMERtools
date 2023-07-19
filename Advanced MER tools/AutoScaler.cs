using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaler : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (!Activate) return;
        Activate = false;
        pairs.Clear();
        foreach (Transform game in this.transform.GetComponentsInChildren<Transform>())
        {
            if (game.transform.GetComponents<Component>().Length == 1) continue;
            pairs.Add(game, new Values(game.position, game.eulerAngles, game.lossyScale));
        }
        List<Transform> list = new List<Transform> { };
        list.AddRange(this.transform.GetComponentsInChildren<Transform>());
        list.FindAll(x => x.transform.GetComponents<Component>().Length == 1).ForEach(x => 
        {
            if (ResetPos) x.localPosition = Vector3.zero;
            if (ResetRot) x.localEulerAngles = Vector3.zero;
            if (ResetScl) x.localScale = Vector3.one;
            list.Remove(x);
        });
        list.ForEach(x =>
        {
            Values values = pairs[x];
            x.position = values.pos;
            x.eulerAngles = values.rot;
            x.localScale = values.scl;
            //x.localScale = operatorDivide(x.lossyScale, values.scl);
        });
    }

    private Vector3 operatorDivide(Vector3 vector, Vector3 vector1)
    {
        return new Vector3(vector.x / vector1.x, vector.y / vector1.y, vector.z / vector1.z);
    }

    public bool Activate;
    public bool ResetPos;
    public bool ResetRot;
    public bool ResetScl = true;

    public struct Values
    {
        public Values(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            pos = v1;
            rot = v2;
            scl = v3;
        }

        public Vector3 pos;
        public Vector3 rot;
        public Vector3 scl;
    }

    Dictionary<Transform, Values> pairs = new Dictionary<Transform, Values> { };
}
