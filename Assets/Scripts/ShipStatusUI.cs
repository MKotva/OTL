using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatusUI : MonoBehaviour
{
    [System.Serializable]
    public class StatusUIEntry
    {
        public string name;

        [Header("UI")]
        public Image damageImage;
        public TMP_Text percentageText;

        [Header("Opacity")]
        [Range(0f, 1f)]
        public float alphaAtFull = 0f;

        [Range(0f, 1f)]
        public float alphaAtEmpty = 1f;

        public void UpdateUI(float currentValue, float maxValue)
        {
            float percent = 0f;

            if (maxValue > 0f)
                percent = Mathf.Clamp01(currentValue / maxValue);

            float damagePercent = 1f - percent;

            if (damageImage != null)
            {
                Color color = damageImage.color;
                color.a = Mathf.Lerp(alphaAtFull, alphaAtEmpty, damagePercent);
                damageImage.color = color;
            }

            if (percentageText != null)
            {
                percentageText.text = Mathf.RoundToInt(percent * 100f) + "%";
            }
        }
    }

    [Header("Ship")]
    public Damageable shipHealth;

    [Header("Shields")]
    public ShieldSector upShield;
    public ShieldSector downShield;
    public ShieldSector leftShield;
    public ShieldSector rightShield;

    [Header("Ship UI")]
    public StatusUIEntry shipUI;

    [Header("Shield UI")]
    public StatusUIEntry upShieldUI;
    public StatusUIEntry downShieldUI;
    public StatusUIEntry leftShieldUI;
    public StatusUIEntry rightShieldUI;

    void Update()
    {
        UpdateShipUI();
        UpdateShieldUI();
    }

    void UpdateShipUI()
    {
        if (shipHealth == null || shipUI == null)
            return;

        shipUI.UpdateUI(
            shipHealth.health,
            shipHealth.maxHealth
        );
    }

    void UpdateShieldUI()
    {
        if (upShield != null && upShieldUI != null)
        {
            upShieldUI.UpdateUI(
                upShield.energy,
                upShield.maxEnergy
            );
        }

        if (downShield != null && downShieldUI != null)
        {
            downShieldUI.UpdateUI(
                downShield.energy,
                downShield.maxEnergy
            );
        }

        if (leftShield != null && leftShieldUI != null)
        {
            leftShieldUI.UpdateUI(
                leftShield.energy,
                leftShield.maxEnergy
            );
        }

        if (rightShield != null && rightShieldUI != null)
        {
            rightShieldUI.UpdateUI(
                rightShield.energy,
                rightShield.maxEnergy
            );
        }
    }
}
