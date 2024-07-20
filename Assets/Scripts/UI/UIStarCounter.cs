using UnityEngine;

public class UIStarCounter : MonoBehaviour
{
    [SerializeField] private GameObject[] starFills;

    public void SetStars(int stars)
    {
        for (int i = 0; i < starFills.Length; i++)
        {
            starFills[i].SetActive(i < stars);
        }
    }
}