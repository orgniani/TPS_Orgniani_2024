using TMPro;
using UnityEngine;

public class UITimeCounter : MonoBehaviour
{
    private float startTime;
    private bool isCounting = false;

    [Header("Text")]
    [SerializeField] private TMP_Text timeText;

    [Header("Stars parameters")]
    [SerializeField] private float threeStarsTime = 60f;
    [SerializeField] private float twoStarsTime = 120f;
    [SerializeField] private float oneStarTime = 180f;

    private float elapsedTime = 0;

    private void Start()
    {
        startTime = Time.time;
        isCounting = true;
    }

    private void Update()
    {
        if (isCounting)
        {
            elapsedTime = Time.time - startTime;

            int hours = (int)(elapsedTime / 3600);
            int minutes = (int)((elapsedTime % 3600) / 60);
            int seconds = (int)(elapsedTime % 60);

            timeText.text = string.Format("TIME • {0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    public TMP_Text TimeText()
    {
        return timeText;
    }


    public void StopCounting()
    {
        isCounting = false;
    }

    public int CalculateStars()
    {
        if (elapsedTime <= threeStarsTime) return 3;

        else if (elapsedTime <= twoStarsTime) return 2;

        else if (elapsedTime <= oneStarTime) return 1;

        else return 0;
    }
}
