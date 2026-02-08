using UnityEngine;

public enum TargetType { Shark, Human }

public class LaneTarget : MonoBehaviour
{
    public TargetType targetType;
    private Vector3 basePos;

    [Header("Visuals")]
    public SpriteRenderer sr;
    public Sprite humanSprite;
    public Sprite sharkSprite;

    private Vector3 baseScale;

    private void Awake()
    {
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            baseScale = sr.transform.localScale;
            basePos = sr.transform.localPosition;
        }
    }

    public void SetTarget(TargetType type)
    {
        targetType = type;
        if (sr == null) return;

        // Sprite driven (partner-friendly)
        sr.sprite = (type == TargetType.Human) ? humanSprite : sharkSprite;

        // Fallback debug colors if sprites not assigned
        if (sr.sprite == null)
        {
            sr.color = (type == TargetType.Human) ? Color.green : Color.red;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    public void SetVisible(bool visible)
    {
        if (sr != null) sr.enabled = visible;
    }

    public void SetAlpha(float a)
    {
        if (sr == null) return;
        var c = sr.color;
        c.a = a;
        sr.color = c;
    }

    public void Wiggle(float amount = 0.5f)
    {
        if (sr == null) return;
        sr.transform.localPosition = basePos + Vector3.right * amount;
    }
    
    public void Pop(float multiplier = 1.8f)
    {
        if (sr == null) return;
        sr.enabled = true;
        sr.transform.localScale = baseScale * multiplier;
    }

    public void ResetScale()
    {
        if (sr == null) return;
        sr.transform.localScale = baseScale;
    }
    public void ResetPosition()
    {
        if (sr == null) return;
        sr.transform.localPosition = basePos;
    }
}