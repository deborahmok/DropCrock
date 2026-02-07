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
        safeLane = (LaneId)Random.Range(0, 3); // Left/Mid/Right
        Debug.Log($"[GM] Level {currentLevel} safe lane = {safeLane}");
    }

    public void OnCrocLanded(LaneId landedLane)
    {
        if (landedLane == safeLane)
        {
            // PASS level
            if (currentLevel >= maxLevel)
            {
                Debug.Log("[GM] YOU WIN!");
                // later: show win UI
            }
            else
            {
                Debug.Log("[GM] Passed! Next level.");
                StartLevel(currentLevel + 1);
            }
        }
        else
        {
            // LOSE -> restart current level
            Debug.Log("[GM] GAME OVER! Restarting current level.");
            StartLevel(currentLevel);
        }
    }
}