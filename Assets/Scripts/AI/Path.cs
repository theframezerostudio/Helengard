using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    [SerializeField, ReadOnly] private Vector3[] path = null;

    private Color[] colors = null;

    [ContextMenu("Create Points From Children")]
    private void CreatePoints()
    {
        points = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            points[i] = transform.GetChild(i);
        }
    }

    [ContextMenu("Bake Path")]
    private void BakePath()
    {
        if (points.Length == 0)
            return;

        path = new Vector3[points.Length];

        colors = new[] { Color.red, Color.aliceBlue, Color.rebeccaPurple, Color.plum, Color.powderBlue,
                         Color.peru, Color.paleVioletRed, Color.paleGoldenRod, Color.paleTurquoise, Color.peachPuff};

        for (int i = 0; i < points.Length; i++)
        {
            path[i] = points[i].position;
        }
    }

    [ContextMenu("Reset Points")]
    public void ResetPoints()
    {
        points = null;
    }

    [ContextMenu("Reset Path")]
    private void ResetPath()
    {
        path = null;
        colors = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (path == null || path.Length == 0)
            return;

        Gizmos.color = colors[0];
        Gizmos.DrawSphere(path[0], 0.1f);

        for (int i = 1; i < path.Length; i++)
        {
            Gizmos.color = colors[i % colors.Length];

            Gizmos.DrawSphere(path[i], 0.1f);
            Gizmos.DrawLine(path[i], path[i-1]);
        }

        Gizmos.DrawLine(path[path.Length - 1], path[0]);
    }
}
