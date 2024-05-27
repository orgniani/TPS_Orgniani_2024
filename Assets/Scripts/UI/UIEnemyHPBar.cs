using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHPBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Image healthBar;
    [SerializeField] private HealthController enemyHP;

    [SerializeField] private bool matchYaxis;

    private void OnEnable()
    {
        enemyHP.onHurt += HandleHPBar;
    }

    private void OnDisable()
    {
        enemyHP.onHurt -= HandleHPBar;
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        FaceCamera();
    }

    private void FaceCamera()
    {
        Vector3 pos = cam.transform.position;
        if (matchYaxis)
            pos.y = transform.position.y;

        transform.LookAt(pos, Vector3.up);
    }

    private void HandleHPBar()
    {
        if (!healthBar) return;

        healthBar.fillAmount = 1.0f * enemyHP.Health / enemyHP.MaxHealth;
    }
}
