using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(VisionDetector))]
public class VisionConeRenderer : MonoBehaviour
{
    public int segments = 20;
    public Material visionMaterial;

    private VisionDetector detector;

    void Start()
    {
        detector = GetComponent<VisionDetector>();
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // Center point
        vertices[0] = Vector3.zero;

        // Cone edges
        for (int i = 0; i <= segments; i++)
        {
            float angle = -detector.viewAngle / 2 + (detector.viewAngle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            vertices[i + 1] = dir * detector.viewRadius;

            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        if (visionMaterial != null)
        {
            mr.material = visionMaterial;
        }
    }

}
