using UnityEngine;
using UnityEngine.InputSystem;

public class CrocController : MonoBehaviour
{
    private enum State { Moving, Dropping, Resolved }
    private State state = State.Moving;

    [Header("Movement")]
    public float baseMoveSpeed = 5f;
    public float moveSpeedPerLevel = 2f;
    private float currentMoveSpeed;

    [Header("Dropping")]
    public float dropGravity = 6f;

    [Header("Refs")]
    public Rigidbody2D rb;
    public Transform spawnPoint;
    public GameManager gm;

    private int moveDir = 1; // 1 = right, -1 = left
    private bool hasLanded = false;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (state == State.Moving)
        {
            // constant horizontal movement
            rb.linearVelocity = new Vector2(moveDir * currentMoveSpeed, 0f);

            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartDrop();
            }
        }
    }

    public void BeginLevel(int level)
    {
        // Reset croc to spawn and start moving
        state = State.Moving;
        hasLanded = false;

        currentMoveSpeed = baseMoveSpeed + (level - 1) * moveSpeedPerLevel;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        if (spawnPoint)
            transform.position = spawnPoint.position;

        moveDir = 1; // reset direction each level (optional)
    }

    private void StartDrop()
    {
        state = State.Dropping;
        rb.linearVelocity = new Vector2(0f, 0f);
        rb.gravityScale = dropGravity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"TRIGGER with: {other.name} tag={other.tag} state={state}");

        if (state == State.Moving && other.CompareTag("Wall"))
        {
            moveDir *= -1;

            // Snap slightly away from the wall so we don't keep re-triggering / drift through
            float pushIn = 0.2f;
            transform.position += new Vector3(moveDir * pushIn, 0f, 0f);

            // Immediately apply new velocity this frame
            rb.linearVelocity = new Vector2(moveDir * currentMoveSpeed, 0f);
            return;
        }

        if (state == State.Dropping && !hasLanded && other.CompareTag("Lane"))
        {
            var lane = other.GetComponent<LaneTrigger>();
            if (lane != null)
            {
                hasLanded = true;
                state = State.Resolved;
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                gm.OnCrocLanded(lane.laneId);
            }
        }
    } //debug
    
    private void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
    }
}