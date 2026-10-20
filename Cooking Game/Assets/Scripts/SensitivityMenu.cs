using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Put on the Sensitivity slider object inside your pause menu.
public class SensitivityMenu : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueText;           // shows e.g. "100%"
    [SerializeField] private FirstPersonController player;

    void Start()
    {
        slider.minValue = 0.2f;
        slider.maxValue = 3f;
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(FirstPersonController.SensitivityKey, 1f));
        slider.onValueChanged.AddListener(OnSliderChanged);
        UpdateLabel(slider.value);
    }

    private void OnSliderChanged(float value)
    {
        player.SetSensitivityMultiplier(value);
        UpdateLabel(value);
    }

    private void UpdateLabel(float value)
    {
        if (valueText != null) valueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
