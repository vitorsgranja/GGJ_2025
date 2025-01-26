using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class LineCollision2 : MonoBehaviour
{
    public MapController mapController;

    public PolygonCollider2D polygonCollider2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    //void LateUpdate()
    public void UpdateCollision()
    {
        Vector3[] positions = mapController.GetPositions();
        if (positions.Length >= 2)
        {
            int numberOfLines = positions.Length - 1;
            polygonCollider2D.pathCount = numberOfLines;

            for (int i = 0; i < numberOfLines; i++)
            {
                List<Vector2> currentPositions = new List<Vector2>
                {
                    positions[i],
                    positions[i+1]
                };

                List<Vector2> currentColliderPoints = CalculateColliderPoints(currentPositions);
                polygonCollider2D.SetPath(i, currentColliderPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));
            }
        }
        else
        {
            polygonCollider2D.pathCount = 0;
        }
    }

    private List<Vector2> CalculateColliderPoints(List<Vector2> positions)
    {
        float width = mapController.GetWidth();

        float m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);
        float deltaX = (width / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
        float deltaY = (width / 2f) * (1 / Mathf.Pow(1 + m * m, 0.5f));

        Vector2[] offsets = new Vector2[2];
        offsets[0] = new Vector2(-deltaX, deltaY);
        offsets[0] = new Vector2(deltaX, -deltaY);

        List<Vector2> colliderPositions = new List<Vector2>
        {
            positions[0] + offsets[0],
            positions[1] + offsets[0],
            positions[1] + offsets[1],
            positions[0] + offsets[1],
        };

        return colliderPositions;
    }
}
