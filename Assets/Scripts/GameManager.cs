using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    
    [Header("UI")]
    public TMP_Text resultText;
    public TMP_Text levelText;

    [Header("Timing")]
    public float resolveDelay = 0.8f;

    public GameObject playAgainButton;
    private bool gameWon = false;
    
    private void Start()
    {
        StartLevel(currentLevel);
    }

    public void StartLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        gameWon = false;
        
        if (levelText) levelText.text = $"Level {currentLevel}";
        if (resultText) resultText.text = "";
        if (playAgainButton) playAgainButton.SetActive(false);

        croc.enabled = false;

        StopAllCoroutines();
        StartCoroutine(ShuffleThenAssignAndBegin());
    }
    
    public void PlayAgain()
    {
        Debug.Log("[GM] PlayAgain clicked");
        if (resultText) resultText.text = "";
        if (playAgainButton) playAgainButton.SetActive(false);

        // croc.enabled = true;
        if (gameWon)
        {
            // If they finished level 3, restart from level 1
            StartLevel(1);
        }
        else
        {
            // If they lost, retry the same level
            StartLevel(currentLevel);
        }
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
    
    private IEnumerator ShuffleThenAssign()
    {
        // Ensure targets are visible at round start
        SetLaneTargetsVisible(true);

        laneLeft.Pop(); laneMid.Pop(); laneRight.Pop();
        yield return new WaitForSeconds(0.2f);

        laneLeft.ResetScale(); laneMid.ResetScale(); laneRight.ResetScale();

        RandomizeSafeLane();
    }

    private IEnumerator ShuffleThenAssignAndBegin()
    {
        Debug.Log("[GM] ShuffleThenAssignAndBegin");

        SetLaneTargetsVisible(true);

        laneLeft.Wiggle(-0.5f);
        laneMid.Wiggle(0.5f);
        laneRight.Wiggle(-0.5f);

        yield return new WaitForSeconds(0.5f); // LONG and obvious

        laneLeft.ResetPosition();
        laneMid.ResetPosition();
        laneRight.ResetPosition();

        RandomizeSafeLane();

        croc.enabled = true;
        croc.BeginLevel(currentLevel);
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
    
    private void SetLaneTargetsVisible(bool visible)
    {
        laneLeft.SetVisible(visible);
        laneMid.SetVisible(visible);
        laneRight.SetVisible(visible);
    }
    
    public void OnCrocLanded(LaneId landedLane)
    {
        bool win = (landedLane == safeLane);
        StartCoroutine(ResolveAndContinue(win));
    }

    private IEnumerator ResolveAndContinue(bool win)
    {
        if (resultText)
            resultText.text = win ? "FED!" : "GAME OVER";

        yield return new WaitForSeconds(resolveDelay);

        if (win)
        {
            if (currentLevel >= maxLevel)
            {
                gameWon = true;
                if (resultText) resultText.text = "YOU WIN!";
                croc.enabled = false;
                if (playAgainButton) playAgainButton.SetActive(true);
                yield break;
            }
            else
            {
                StartLevel(currentLevel + 1);
            }
        }
        else
        {
            // GAME OVER screen instead of auto-restart
            croc.enabled = false;
            SetLaneTargetsVisible(false);
            if (playAgainButton) playAgainButton.SetActive(true);
            yield break;
        }
    }
    
    public void ResetGame()
    {
        // Full reset to Level 1
        gameWon = false;

        if (resultText) resultText.text = "";
        if (playAgainButton) playAgainButton.SetActive(false);

        SetLaneTargetsVisible(true);

        // Make sure croc can move again
        croc.enabled = true;

        StartLevel(1);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops play mode in editor
#else
    Application.Quit(); // quits a built game
#endif
    }
    
    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.rKey.wasPressedThisFrame)
        {
            ResetGame();
        }

        if (kb.escapeKey.wasPressedThisFrame)
        {
            ExitGame();
        }
    }
}