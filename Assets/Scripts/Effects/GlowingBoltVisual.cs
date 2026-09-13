using UnityEngine;

public class GlowingBoltVisual : MonoBehaviour
{
    [SerializeField, Min(0.05f)] private float length = 0.55f;

    private static Material lineMaterial;
    private Vector3 lastPosition;

    private void Awake()
    {
        lastPosition = transform.position;
        CreateLine("Outer Glow", 0.24f, new Color(1f, 0f, 0f, 0.16f));
        CreateLine("Inner Glow", 0.12f, new Color(1f, 0.05f, 0.02f, 0.55f));
        CreateLine("Bright Core", 0.045f, new Color(1f, 0.78f, 0.7f, 1f));
    }

    private void LateUpdate()
    {
        Vector3 movement = transform.position - lastPosition;
        if (movement.sqrMagnitude > 0.000001f)
            transform.right = movement.normalized;

        lastPosition = transform.position;
    }

    private void CreateLine(string lineName, float width, Color color)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(transform, false);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(-length * 0.5f, 0f, 0f));
        line.SetPosition(1, new Vector3(length * 0.5f, 0f, 0f));
        line.startWidth = width;
        line.endWidth = width;
        line.startColor = color;
        line.endColor = color;
        line.numCapVertices = 4;
        line.sortingLayerID = -43522115;
        line.sortingOrder = 2;
        line.sharedMaterial = GetLineMaterial();
    }

    private static Material GetLineMaterial()
    {
        if (lineMaterial != null)
            return lineMaterial;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogError("No compatible sprite shader was found for the Mage bolt.");
            return null;
        }

        lineMaterial = new Material(shader)
        {
            name = "Mage Bolt Glow Material",
            hideFlags = HideFlags.HideAndDontSave
        };
        return lineMaterial;
    }
}
