using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField]
    private List<GameObject> lockedWeapons = new List<GameObject>();

    [Header("Popup")]
    [SerializeField]
    private GameObject weaponUnlockPopupPrefab;

    [SerializeField]
    private Transform popupParent;

    [SerializeField]
    private float popupDuration = 2f;

    private GameObject activePopup;

    public GameObject UnlockRandomWeapon()
    {
        List<GameObject> availableWeapons = new List<GameObject>();

        for (int i = 0; i < lockedWeapons.Count; i++)
        {
            GameObject weapon = lockedWeapons[i];

            if (weapon == null)
                continue;

            if (!weapon.activeSelf)
                availableWeapons.Add(weapon);
        }

        if (availableWeapons.Count <= 0)
            return null;

        GameObject selectedWeapon = availableWeapons[
            UnityEngine.Random.Range(0, availableWeapons.Count)
        ];

        selectedWeapon.SetActive(true);
        ShowPopup(selectedWeapon);

        return selectedWeapon;
    }

    void ShowPopup(GameObject unlockedWeapon)
    {
        if (weaponUnlockPopupPrefab == null)
            return;

        if (activePopup != null)
            Destroy(activePopup);

        activePopup = Instantiate(
            weaponUnlockPopupPrefab,
            popupParent
        );

        WeaponUnlockUi popupUI =
            activePopup.GetComponent<WeaponUnlockUi>();

        if (popupUI != null)
            popupUI.Show(unlockedWeapon);

        Destroy(activePopup, popupDuration);
    }
}

