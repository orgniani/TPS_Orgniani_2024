using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnvironmentInteraction playerEnvironmentInteraction;

    [Header("Animation")]
    [SerializeField] private AnimationCurve curveColor;
    [SerializeField] private Material sharedMaterial;

    [Header("Color")]
    [SerializeField] private Color originalColor;
    [SerializeField] private Color pulseColor;

    [Header("Text")]
    [SerializeField] private GameObject instructionsCanvas;

    private void Awake()
    {
        sharedMaterial = GetComponent<Renderer>().material;
        originalColor = sharedMaterial.color;
    }

    private void Update()
    {
        StartCoroutine(Pulse());

        if (playerEnvironmentInteraction.IsAtDropSpot)
        {
            instructionsCanvas.SetActive(true);
        }

        else
        {
            instructionsCanvas.SetActive(false);
        }
    }

    private IEnumerator Pulse()
    {
        float tt = 0;

        if (!playerEnvironmentInteraction.IsDraggingEnemy())
        {
            sharedMaterial.color = originalColor;
            yield break;
        }

        while (playerEnvironmentInteraction.IsDraggingEnemy())
        {
            tt += Time.deltaTime;
            float valorDe0a1 = tt / 1f;

            float colorVal = curveColor.Evaluate(valorDe0a1);

            sharedMaterial.color = Color.Lerp(originalColor, pulseColor, colorVal);

            yield return null;
        }
    }
}
