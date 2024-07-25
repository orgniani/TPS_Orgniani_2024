using System.Collections;
using UnityEngine;

public class PickUpItems : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private ItemType itemType;

    [Header("Meds Parameters")]
    [SerializeField] private float restoredHP = 10f;

    [Header("Arrow animation parameters")]
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float posYPosition = 1f;

    private Vector3 initialPosition;

    private void Start()
    {
        if(arrow == null) return;

        initialPosition = arrow.transform.position;
        StartCoroutine(Float());
    }

    public ItemType GetItemType()
    {
        return itemType;
    }

    public float GetRestoredHP()
    {
        return restoredHP;
    }


    private IEnumerator Float()
    {
        while (true)
        {
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                float t = elapsedTime / animationDuration;
                float curveValue = animationCurve.Evaluate(t);

                Vector3 targetPosition = initialPosition + Vector3.up * posYPosition * curveValue;

                arrow.transform.position = Vector3.Lerp(arrow.transform.position, targetPosition, Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            posYPosition = -posYPosition;
        }

    }
}
