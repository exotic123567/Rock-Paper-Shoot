using UnityEngine;
using UnityEngine.Serialization;

public class WallBoundaryBuilder : MonoBehaviour
{
    [FormerlySerializedAs("wallBlockPrefab")] public GameObject tophorizontalWallBlockPrefab; // Top boundary wall
    [SerializeField] public GameObject bottomhorizontalWallBlockPrefab; // Bottom boundary wall
    [SerializeField] public GameObject verticalleftWallBlockPrefab; // Left boundary wall
    [SerializeField] public GameObject verticalrightWallBlockPrefab; // Right boundary wall

    public float boundaryWidth = 20f;  // Width of the boundary area
    public float boundaryHeight = 20f; // Height of the boundary area
    public float blockSpacing = 0.1f;  // Optional spacing between blocks

    private void Start()
    {
        CreateBoundary();
    }

    private void CreateBoundary()
    {
        // Calculate block sizes and counts based on prefab dimensions
        float blockWidth = tophorizontalWallBlockPrefab.GetComponent<SpriteRenderer>().bounds.size.x + blockSpacing;
        float blockHeight = tophorizontalWallBlockPrefab.GetComponent<SpriteRenderer>().bounds.size.y + blockSpacing;

        int horizontalBlockCount = Mathf.CeilToInt(boundaryWidth / blockWidth);
        int verticalBlockCount = Mathf.CeilToInt(boundaryHeight / blockHeight);

        // Top boundary with top horizontal blocks
        for (int i = 0; i < horizontalBlockCount; i++)
        {
            Vector2 position = new Vector2(-boundaryWidth / 2 + i * blockWidth, boundaryHeight / 2);
            Instantiate(tophorizontalWallBlockPrefab, position, Quaternion.identity, transform);
        }

        // Bottom boundary with bottom horizontal blocks
        for (int i = 0; i < horizontalBlockCount; i++)
        {
            Vector2 position = new Vector2(-boundaryWidth / 2 + i * blockWidth, -boundaryHeight / 2);
            Instantiate(bottomhorizontalWallBlockPrefab, position, Quaternion.identity, transform);
        }

        // Left boundary with left vertical blocks
        for (int i = 0; i < verticalBlockCount; i++)
        {
            Vector2 position = new Vector2(-boundaryWidth / 2, -boundaryHeight / 2 + i * blockHeight);
            Instantiate(verticalleftWallBlockPrefab, position, Quaternion.identity, transform);
        }

        // Right boundary with right vertical blocks
        for (int i = 0; i < verticalBlockCount; i++)
        {
            Vector2 position = new Vector2(boundaryWidth / 2, -boundaryHeight / 2 + i * blockHeight);
            Instantiate(verticalrightWallBlockPrefab, position, Quaternion.identity, transform);
        }
    }
}
