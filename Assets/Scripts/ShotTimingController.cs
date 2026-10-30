
using UnityEngine;
using TMPro;

public class ShotTimingController : MonoBehaviour
{
    [SerializeField] private RectTransform shotTimingCursor;
    [SerializeField] private TMP_Text shotResultText;
    [SerializeField] private TMP_Text matchScoreText;
    [SerializeField] private float speed = 280f;
    [SerializeField] private float travelDistance = 235f;

    private bool shotTaken = false;
    private float shotTimer = 0f;
    private int bramwickGoals = 0;
    private int opponentGoals = 0;

    private void Start()
    {
        if (shotResultText != null)
            shotResultText.text = "";
        UpdateScoreboard();
    }
        [ContextMenu("Test Bramwick Goal")]
        public void RegisterBramwickGoal()
    {
        bramwickGoals++;

        UpdateScoreboard();

        Debug.Log("GOAL! Bramwick Town scores!");
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

        shotTaken = true;

        float distanceFromCentre =
            Mathf.Abs(shotTimingCursor.anchoredPosition.x);


        if (distanceFromCentre <= 45f)
        {
            Debug.Log("PERFECT SHOT!");

            if (Random.value < 0.65f)
            {
                RegisterBramwickGoal();
            }

            if (shotResultText != null)
                shotResultText.text = "PERFECT SHOT!";
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
        shotTaken = false;
        shotTimer = 0f;

        if (shotTimingCursor != null)
        {
            shotTimingCursor.anchoredPosition =
                new Vector2(-travelDistance, 0f);
        }

        if (shotResultText != null)
            shotResultText.text = "";
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
