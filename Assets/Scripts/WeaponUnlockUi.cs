using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponUnlockUi : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text titleText;
    public TMP_Text bodyText;

    [Header("Lifetime")]
    public float ttl = 2f;
    public bool destroyAfterTtl = true;

    private Coroutine destroyCoroutine;

    public void Show(GameObject unlockedWeapon)
    {
        if (titleText != null)
            titleText.text = "New Weapon Added";

        if (bodyText != null)
        {
            if (unlockedWeapon != null)
                bodyText.text = unlockedWeapon.name + " is now active.";
            else
                bodyText.text = "No more weapons to unlock.";
        }

        if (destroyAfterTtl)
        {
            if (destroyCoroutine != null)
                StopCoroutine(destroyCoroutine);

            destroyCoroutine = StartCoroutine(DestroyAfterTtl());
        }
    }

    private IEnumerator DestroyAfterTtl()
    {
        yield return new WaitForSecondsRealtime(ttl);

        Destroy(gameObject);
    }
}
