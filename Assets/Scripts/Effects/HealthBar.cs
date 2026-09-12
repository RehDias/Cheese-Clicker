using UnityEngine;
using UnityEngine.UI;
  using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (slider == null)
            return;

        slider.minValue = 0;
        slider.maxValue = maxHealth;
        slider.SetValueWithoutNotify(currentHealth);

        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }
}
