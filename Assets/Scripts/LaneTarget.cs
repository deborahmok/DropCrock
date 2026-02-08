using UnityEngine;

public enum TargetType { Shark, Human }

public class LaneTarget : MonoBehaviour
{
    public TargetType targetType;

    [Header("Optional visuals")]
    public SpriteRenderer sr;

    public void SetTarget(TargetType type)
    {
        targetType = type;

        // Visual placeholder: different colors
        if (sr != null)
        {
            sr.color = (type == TargetType.Human) ? Color.green : Color.red;
        }
    }

    private void Reset()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }
}