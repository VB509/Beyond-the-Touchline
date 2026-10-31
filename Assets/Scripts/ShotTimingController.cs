
using UnityEngine;
using TMPro;

public class ShotTimingController : MonoBehaviour
{
    [SerializeField] private RectTransform shotTimingCursor;
    [SerializeField] private RectTransform perfectShotZone;
    [SerializeField] private RectTransform goalZone;
    [SerializeField] private TMP_Text shotResultText;
    [SerializeField] private TMP_Text matchScoreText;
    [SerializeField] private TMP_Text matchPhaseText;
    [SerializeField] private TMP_Text matchActionButtonText;
    [SerializeField] private TMP_Text resetButtonText;
    [SerializeField] private TMP_Text matchClockText;
    [SerializeField] private TMP_Text matchEventText;
    [SerializeField] private Fungus.Flowchart storyFlowchart;
    [SerializeField] private Canvas matchCanvas;
    [SerializeField] private float speed = 280f;
    [SerializeField] private float travelDistance = 235f;

    private bool shotTaken = false;
    private float shotTimer = 0f;
    private bool isAttacking = true;

    private int bramwickGoals = 0;
    private int opponentGoals = 0;
    private int actionsCompleted = 0;
    private const int totalActions = 9;

    private readonly string[] matchEvents =
    {
    "Lukas finds space behind the defence!",
    "Daniel tracks a dangerous counterattack!",
    "Bramwick break forward through midfield!",
    "The opposition striker slips into the box!",
    "Lukas chases a loose ball before half-time!",
    "Daniel spots an unmarked opposition player!",
    "The crowd roars as Bramwick push forward!",
    "Bramwick are caught on the break!",
    "90th minute! One final chance for Bramwick Town!"
};

    private int teamAttack;
    private int teamDefense;
    private int teamFitness;
    private int matchTactic;
    private int dressingRoomTrust;
    private int fanTrust;

    private void Start()
    {


        if (storyFlowchart != null)
        {

            teamAttack = storyFlowchart.GetIntegerVariable("TeamAttack");
            teamDefense = storyFlowchart.GetIntegerVariable("TeamDefense");
            teamFitness = storyFlowchart.GetIntegerVariable("TeamFitness");


            Debug.Log(
                "MATCH STATS - Attack: " + teamAttack +
                " | Defense: " + teamDefense +
                " | Fitness: " + teamFitness
            );
        }


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
        UpdateTimingZone();
        if (matchCanvas != null)
        {
            matchCanvas.enabled = false;
        }
    }
private void UpdateMatchClock()
    {
        if (matchClockText == null)
            return;


        int minute = Mathf.Clamp(
            Mathf.RoundToInt(actionsCompleted * 90f / (totalActions - 1)),
            0,
            90
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
        UpdateTimingZone();
        UpdateMatchEvent();

    }



    private void Update()
    {
        if (shotTimingCursor == null || shotTaken)
            return;


        shotTimer += Time.deltaTime;

        float matchProgress = (float)actionsCompleted / totalActions;
        float fitnessFactor = Mathf.Clamp01((100f - teamFitness) / 100f);
        float fatigueMultiplier = 1f + matchProgress * fitnessFactor * 2.5f;
        float crowdInfluence = Mathf.Clamp01(
    (matchProgress - 0.5f) * 2f
);

        float fanSpeedMultiplier = Mathf.Clamp(
            1f - (fanTrust - 50f) * 0.005f * crowdInfluence,
            0.8f,
            1.2f
        );

        float x = Mathf.PingPong(
            shotTimer * (speed * fatigueMultiplier + 30f * (1f - matchProgress))
    * fanSpeedMultiplier,
            travelDistance * 2f
        ) - travelDistance;

        shotTimingCursor.anchoredPosition = new Vector2(x, 0f);
    }


    public void Shoot()
    {
        if (shotTimingCursor == null || perfectShotZone == null || shotTaken)
            return;

        shotTaken = true;

        float distanceFromCentre =
            Mathf.Abs(shotTimingCursor.anchoredPosition.x);

        // DEFENSIVE ACTION
        if (!isAttacking)
        {
            if (distanceFromCentre <= perfectShotZone.rect.width / 2f)
            {
                if (shotResultText != null)
                    shotResultText.text = "TACKLE SUCCESS!";

                if (matchEventText != null)
                    matchEventText.text =
                        "Daniel reads the play and wins the ball!";

                Debug.Log("Successful tackle!");
            }
            else
            {
                RegisterOpponentGoal();

                if (shotResultText != null)
                    shotResultText.text = "OPPOSITION GOAL!";

                if (matchEventText != null)
                    matchEventText.text =
                        "The opposition striker finds the net!";

                Debug.Log("Missed tackle!");
            }

            return;
        }

        // ATTACKING ACTION
        if (distanceFromCentre <= perfectShotZone.rect.width / 2f)
        {
            // Perfect timing: GOAL
            if (distanceFromCentre <= perfectShotZone.rect.width * 0.20f)
            {
                RegisterBramwickGoal();

                if (shotResultText != null)
                    shotResultText.text = "GOAL!";

                if (matchEventText != null)
                    matchEventText.text =
                        "The Bramwick players race towards the corner flag!";
            }
            // Good timing, but not perfect: SAVED
            else
            {
                if (shotResultText != null)
                    shotResultText.text = "SAVED!";

                if (matchEventText != null)
                    matchEventText.text =
                        "The goalkeeper dives and pushes the ball away!";

                Debug.Log("Shot saved by the goalkeeper!");
            }
        }
        // Poor timing: MISS
        else
        {
            if (shotResultText != null)
                shotResultText.text = "NOT PERFECT!";

            if (matchEventText != null)
                matchEventText.text =
                    "The shot flies wide of the post!";

            Debug.Log("Shot outside the perfect zone.");
        }
    }



    public void ResetShot()
    {

        if (actionsCompleted >= totalActions)
        {
            ContinueToPostMatch();
            return;
        }

        if (!shotTaken)
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
            if (resetButtonText != null)
                resetButtonText.text = "CONTINUE";

            if (matchEventText != null)
            {
                if (bramwickGoals > opponentGoals)
                    matchEventText.text = "Bramwick Town win! What a performance!";
                else if (bramwickGoals < opponentGoals)
                    matchEventText.text = "A disappointing defeat for Bramwick Town.";
                else
                    matchEventText.text = "Full time! The points are shared.";
            }


            int finalResult = bramwickGoals > opponentGoals ? 1
                            : bramwickGoals == opponentGoals ? 2
                            : 3;

            if (storyFlowchart != null)
                storyFlowchart.SetIntegerVariable("MatchResult", finalResult);

            Debug.Log("MATCH RESULT SAVED - " + finalResult);

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

    public void StartMatch()
    {
        RefreshTeamStats();
        UpdateMatchEvent();

        if (matchCanvas != null)
        {
            matchCanvas.enabled = true;
        }
    }

    public void RefreshTeamStats()
    {
        if (storyFlowchart == null)
            return;

        teamAttack = storyFlowchart.GetIntegerVariable("TeamAttack");
        teamDefense = storyFlowchart.GetIntegerVariable("TeamDefense");
        teamFitness = storyFlowchart.GetIntegerVariable("TeamFitness");
        matchTactic = storyFlowchart.GetIntegerVariable("MatchTactic");
        dressingRoomTrust = storyFlowchart.GetIntegerVariable("DressingRoomTrust");
        fanTrust = storyFlowchart.GetIntegerVariable("FanTrust");

        UpdateTimingZone();

        Debug.Log(
            $"MATCH STATS UPDATED - Attack: {teamAttack} | Defense: {teamDefense} | Fitness: {teamFitness}"
        );
        Debug.Log("MATCH TACTIC - " + matchTactic);
        Debug.Log(
    "MATCH TRUST - Dressing Room: " + dressingRoomTrust +
    " | Fans: " + fanTrust
);
    }

    private void UpdateMatchEvent()
    {
        if (matchEventText == null)
            return;


        if (actionsCompleted < matchEvents.Length)
        {
            matchEventText.text = matchEvents[actionsCompleted];
        }

    }

    private void UpdateTimingZone()
    {
        if (perfectShotZone == null)
            return;

        int skill = isAttacking ? teamAttack : teamDefense;

        if (matchTactic == 1)
            skill += isAttacking ? 10 : -10;
        else if (matchTactic == 2)
            skill += isAttacking ? -10 : 10;
        int trustBonus = Mathf.RoundToInt(
    (dressingRoomTrust - 50) * 0.2f
);

        skill += trustBonus;


        float zoneWidth = Mathf.Clamp(
            90f + (skill - 50) * 2f,
            50f,
            150f
        );

        perfectShotZone.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            zoneWidth
        );

        if (goalZone != null)
        {
            goalZone.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                zoneWidth * 0.40f
            );
            goalZone.gameObject.SetActive(isAttacking);
        }

    }
    public void ContinueToPostMatch()
    {
        if (actionsCompleted < totalActions || storyFlowchart == null)
            return;

        if (matchCanvas != null)
            matchCanvas.enabled = false;

        storyFlowchart.ExecuteBlock("Day07_PostMatch");
    }


}
