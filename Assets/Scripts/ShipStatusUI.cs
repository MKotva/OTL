using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleShipStatusUI : MonoBehaviour
{
    [Header("Ship")]
    public Damageable ship;

    [Header("Shields")]
    public ShieldSector upShield;
    public ShieldSector downShield;
    public ShieldSector leftShield;
    public ShieldSector rightShield;

    [Header("Ship UI")]
    public Image shipImage;
    public TMP_Text shipText;

    [Header("Shield UI")]
    public Image upImage;
    public TMP_Text upText;

    public Image downImage;
    public TMP_Text downText;

    public Image leftImage;
    public TMP_Text leftText;

    public Image rightImage;
    public TMP_Text rightText;

    void Update()
    {
        if (ship != null)
            UpdateItem(shipImage, shipText, ship.health, ship.maxHealth);

        if (upShield != null)
            UpdateItem(upImage, upText, upShield.energy, upShield.maxEnergy);

        if (downShield != null)
            UpdateItem(downImage, downText, downShield.energy, downShield.maxEnergy);

        if (leftShield != null)
            UpdateItem(leftImage, leftText, leftShield.energy, leftShield.maxEnergy);

        if (rightShield != null)
            UpdateItem(rightImage, rightText, rightShield.energy, rightShield.maxEnergy);
    }

    void UpdateItem(Image image, TMP_Text text, float current, float max)
    {
        if (max <= 0f)
            return;

        float percent = Mathf.Clamp01(current / max);

        // 100% = visible, 0% = invisible
        if (image != null)
        {
            Color color = image.color;
            color.a = percent;
            image.color = color;
        }

        if (text != null)
        {
            text.text = Mathf.RoundToInt(percent * 100f) + "%";
        }
    }
}