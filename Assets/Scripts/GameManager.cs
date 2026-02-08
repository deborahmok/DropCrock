using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Progression")]
    public int maxLevel = 3;
    public int currentLevel = 1;

    [Header("Randomized safe lane (human)")]
    public LaneId safeLane;

    [Header("Refs")]
    public CrocController croc;
    
    [Header("Lane refs")]
    public LaneTarget laneLeft;
    public LaneTarget laneMid;
    public LaneTarget laneRight;

    private void Start()
    {
        StartLevel(currentLevel);
    }

    public void StartLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        RandomizeSafeLane();
        croc.BeginLevel(currentLevel);
    }

    public void RandomizeSafeLane()
    {
        safeLane = (LaneId)Random.Range(0, 3);

        laneLeft.SetTarget(TargetType.Shark);
        laneMid.SetTarget(TargetType.Shark);
        laneRight.SetTarget(TargetType.Shark);

        GetLaneTarget(safeLane).SetTarget(TargetType.Human);

        Debug.Log($"[GM] Level {currentLevel} safe lane = {safeLane}");
    }

    private LaneTarget GetLaneTarget(LaneId id)
    {
        return id switch
        {
            LaneId.Left => laneLeft,
            LaneId.Mid => laneMid,
            _ => laneRight,
        };
    }
    
    public void OnCrocLanded(LaneId landedLane)
    {
        bool win = (landedLane == safeLane);
        StartCoroutine(ResolveAndContinue(win));
    }
    
    private System.Collections.IEnumerator ResolveAndContinue(bool win)
    {
        if (win)
        {
            Debug.Log("[GM] PASS!");
            // later: show “Fed!” text
        }
        else
        {
            Debug.Log("[GM] GAME OVER! Restarting current level.");
            // later: show “Game Over” text
        }

        yield return new WaitForSeconds(0.6f);

        if (win)
        {
            if (currentLevel >= maxLevel)
            {
                Debug.Log("[GM] YOU WIN!");
                // later: stop input + show win screen
            }
            else
            {
                StartLevel(currentLevel + 1);
            }
        }
        else
        {
            StartLevel(currentLevel); // restart same level
        }
    }
}