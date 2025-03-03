using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            Vector3 v1 = game.position;
            Vector3 v2 = game.eulerAngles;
            Vector3 v3 = game.lossyScale;
            pairs.Add(game, new Values(IsEmpty(game.gameObject) && ResetPos ? Vector3.zero : new Vector3(v1.x, v1.y, v1.z),
                IsEmpty(game.gameObject) && ResetRot ? Vector3.zero : new Vector3(v2.x, v2.y, v2.z), 
                IsEmpty(game.gameObject) && ResetScl ? Vector3.one : new Vector3(v3.x, v3.y, v3.z)));
        }
        List<Transform> list = new List<Transform> { };
        list.AddRange(this.transform.GetComponentsInChildren<Transform>());
        list.ForEach(x =>
        {
            Values values = pairs[x];
            x.position = values.pos;
            x.eulerAngles = values.rot;
            x.localScale = values.scl;
            //x.localScale = operatorDivide(x.lossyScale, values.scl);
        });
        //wait(list);
    }

    //static async Task wait(List<Transform> transforms)
    //{
    //    ForceStop = true;
    //    await Task.Delay(1000);
    //    transforms.ForEach(x =>
    //    {
    //        Values values = pairs[x];
    //        x.position = values.pos;
    //        x.eulerAngles = values.rot;
    //        //x.localScale = operatorDivide(x.lossyScale, values.scl);
    //    });
    //    ForceStop = false;
    //}

    bool IsEmpty(GameObject game)
    {
        if (game == gameObject || game.GetComponents<Component>().Length == 1)
            return true;
        if (!game.TryGetComponent<MeshFilter>(out _) && !game.TryGetComponent<Light>(out _) && !game.TryGetComponent<PickupComponent>(out _))
            return true;
        return false;
    }

    private Vector3 operatorDivide(Vector3 vector, Vector3 vector1)
    {
        return new Vector3(vector.x / vector1.x, vector.y / vector1.y, vector.z / vector1.z);
    }

    //static bool ForceStop = false;

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
