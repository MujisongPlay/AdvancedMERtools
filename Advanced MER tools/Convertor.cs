using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class Convertor : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (Activate)
        {
            Activate = false;
            List<string> tags = new List<string> { };
            tags.AddRange(InternalEditorUtility.tags);
            foreach (MeshFilter mesh in GetComponentsInChildren<MeshFilter>())
            {
                if (!mesh.TryGetComponent<PrimitiveComponent>(out _) && mesh.CompareTag("Untagged") && tags.Contains(mesh.sharedMesh.name))
                {
                    mesh.gameObject.tag = mesh.sharedMesh.name;
                    MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
                    Color color = renderer.sharedMaterial.color;
                    Color _color = new Color(color.r, color.g, color.b, color.a);
                    PrimitiveComponent primitive = mesh.gameObject.AddComponent<PrimitiveComponent>();
                    primitive.Collidable = true;
                    primitive.Color = _color;
                    Material material = new Material((Material)Resources.Load(_color.a >= 1f ? "Materials/Regular" : "Materials/Transparent"));
                    renderer.sharedMaterial = material;
                    renderer.sharedMaterial.color = _color;
                }
            }
        }
    }

    public bool Activate = false;
}
