using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectGTA2_Unity;

public class Hud : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI ammoTextField;


    private void Awake()
    {
        WeaponBelt.WeaponChanged += UpdateWeapon;
        WeaponBelt.WeaponUpdate += UpdateAmmo;
    }

    private void OnDestroy()
    {
        WeaponBelt.WeaponChanged -= UpdateWeapon;
        WeaponBelt.WeaponUpdate -= UpdateAmmo;
    }


    private void UpdateWeapon(Sprite sprite, int ammo)
    {
        if(weaponImage == null) return;

        if (sprite == null)
        {
            weaponImage.gameObject.SetActive(false);
        }
        else
        {
            weaponImage.gameObject.SetActive(true);
            weaponImage.sprite = sprite;
        }

        UpdateAmmo(ammo);
    }

    private void UpdateAmmo(int ammo)
    {
        if (ammoTextField == null) return;

        if (ammo < 0)
        {
            ammoTextField.gameObject.SetActive(false);
        }
        else
        {
            ammoTextField.gameObject.SetActive(true);
            ammoTextField.text = ammo.ToString();
        }
    }
}
