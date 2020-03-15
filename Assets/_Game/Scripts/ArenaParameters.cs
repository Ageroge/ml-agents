using UnityEngine;


[CreateAssetMenu]
public class ArenaParameters : ScriptableObject
{
    public Vector2 Size = new Vector2(16, 16);
    [Range(0, 1), SerializeField] float GeneratedPositionBounds = 0.9f;

    public bool IsInside(Vector3 position)
    {
        return -Size.x / 2 <= position.x && position.x <= Size.x / 2 && -Size.y / 2 <= position.y && position.y <= Size.y / 2;
    }

    public Vector3 GeneratePosition()
    {
        float xPosition = Random.value;
        float yPosition = Random.value;
        if (Random.value <= 0.5f)
        {
            xPosition = xPosition <= 0.5f ? 0.0f : 1.0f;
        }
        else
        {
            yPosition = yPosition <= 0.5f ? 0.0f : 1.0f;
        }

        // Set opposite position to archer (target) (Xorboo: random values are not related to an archer in any way)
        return new Vector3(Size.x * (xPosition - 0.5f), 0.5f, Size.y * (yPosition - 0.5f)) * GeneratedPositionBounds;
    }
}
