using UnityEngine;
using TMPro;

public class PlayerSystemsManagement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float health;
    [SerializeField] private bool isShake;

    [SerializeField] private TextMeshProUGUI healthText; 

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void loseHealth(float damage) {
        if (health <= 0) {
            health = 0;
        } else {
            health -= damage;
            UpdateHealthUI();

            if (CameraShake.Instance != null) {

                CameraShake.Instance.ShakeCamera(1f,0.2f);
            }

            if (health <= 0) {
                endGame();
            }
        }
    }

    void UpdateHealthUI() {
        if (healthText != null) {
            healthText.text = "Health: " + health.ToString("F0");
        }
    }

    void Start()
    {
        health = 100.0f;
        UpdateHealthUI();
    }

    void endGame() {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            loseHealth(1.0f);
        }
        if (isShake) {
            loseHealth(1.0f);
            isShake = false;
        }
    }
}
