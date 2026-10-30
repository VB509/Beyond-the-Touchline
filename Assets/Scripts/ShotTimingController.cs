
using UnityEngine;
using TMPro;

public class ShotTimingController : MonoBehaviour
{
    [SerializeField] private RectTransform shotTimingCursor;
    [SerializeField] private TMP_Text shotResultText;
    [SerializeField] private TMP_Text matchScoreText;
    [SerializeField] private TMP_Text matchPhaseText;
    [SerializeField] private TMP_Text matchActionButtonText;
    [SerializeField] private TMP_Text matchClockText;
    [SerializeField] private float speed = 280f;
    [SerializeField] private float travelDistance = 235f;

    private bool shotTaken = false;
    private float shotTimer = 0f;
    private bool isAttacking = true;

    private int bramwickGoals = 0;
    private int opponentGoals = 0;
    private int actionsCompleted = 0;
    private const int totalActions = 8;

    private void Start()
    {
        if (shotResultText != null)
            shotResultText.text = "";

        UpdateScoreboard();

        if (matchPhaseText != null)
        {
            matchPhaseText.text = isAttacking
                ? "BRAMWICK ATTACKING"
                : "BRAMWICK DEFENDING";

        }
        UpdateMatchClock();
    }
private void UpdateMatchClock()
    {
        if (matchClockText == null)
            return;

        int minute = Mathf.RoundToInt(
            actionsCompleted * 90f / totalActions
        );

        matchClockText.text = $"{minute:00}'";
    }



[ContextMenu("Test Switch Match Phase")]
    public void SwitchMatchPhase()
    {
        isAttacking = !isAttacking;

        if (matchPhaseText != null)
        {
            matchPhaseText.text = isAttacking
                ? "BRAMWICK ATTACKING"
                : "BRAMWICK DEFENDING";
        }

        if (matchActionButtonText != null)
        {
            matchActionButtonText.text = isAttacking
                ? "SHOOT!"
                : "TACKLE!";
        }
    }



    private void Update()
    {
        if (shotTimingCursor == null || shotTaken)
            return;

        shotTimer += Time.deltaTime;

        float x = Mathf.PingPong(
            shotTimer * speed,
            travelDistance * 2f
        ) - travelDistance;

        shotTimingCursor.anchoredPosition = new Vector2(x, 0f);
    }

    public void Shoot()
    {
        if (shotTimingCursor == null || shotTaken)
            return;


        if (!isAttacking)
        {
            shotTaken = true;

            float tackleDistance =
                Mathf.Abs(shotTimingCursor.anchoredPosition.x);

            if (tackleDistance <= 45f)
            {
                if (shotResultText != null)
                    shotResultText.text = "TACKLE SUCCESS!";

                Debug.Log("Successful tackle!");
            }

            else
            {
                Debug.Log("Missed tackle!");

                if (Random.value < 0.60f)
                {
                    RegisterOpponentGoal();

                    if (shotResultText != null)
                        shotResultText.text = "OPPOSITION GOAL!";
                }
                else
                {
                    if (shotResultText != null)
                        shotResultText.text = "DEFENCE RECOVERS!";
                }
            }
            return;
        }



        shotTaken = true;

        float distanceFromCentre =
            Mathf.Abs(shotTimingCursor.anchoredPosition.x);

        if (distanceFromCentre <= 45f)
        {
            if (Random.value < 0.65f)
            {
                RegisterBramwickGoal();

                if (shotResultText != null)
                    shotResultText.text = "GOAL!";
            }
            else
            {
                Debug.Log("Shot saved by the goalkeeper!");

                if (shotResultText != null)
                    shotResultText.text = "SAVED!";
            }
        }
        else
        {
            Debug.Log("Shot outside the perfect zone.");

            if (shotResultText != null)
                shotResultText.text = "NOT PERFECT!";
        }
    }



public void ResetShot()
    {
        if (!shotTaken || actionsCompleted >= totalActions)
            return;

        actionsCompleted++;
        UpdateMatchClock();

        // The match ends after eight completed actions.
        if (actionsCompleted >= totalActions)
        {
            if (matchPhaseText != null)
                matchPhaseText.text = "FULL TIME!";

            if (matchActionButtonText != null)
                matchActionButtonText.text = "MATCH OVER";

            return;
        }

        shotTaken = false;
        shotTimer = 0f;

        if (shotTimingCursor != null)
        {
            shotTimingCursor.anchoredPosition =
                new Vector2(-travelDistance, 0f);
        }

        if (shotResultText != null)
            shotResultText.text = "";

        SwitchMatchPhase();
    }


[ContextMenu("Test Bramwick Goal")]
    public void RegisterBramwickGoal()
    {
        bramwickGoals++;

        UpdateScoreboard();

        Debug.Log("GOAL! Bramwick Town scores!");
    }

    public void RegisterOpponentGoal()
    {
        opponentGoals++;

        UpdateScoreboard();

        Debug.Log("GOAL! The opposition scores!");
    }

    private void UpdateScoreboard()
    {
        if (matchScoreText != null)
        {
            matchScoreText.text =
                $"BRAMWICK TOWN {bramwickGoals} - {opponentGoals} OPPOSITION";
        }
    }
}
