/// <author>Thoams Krahl</author>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectGTA2_Unity.UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image weaponImage;
        //[SerializeField] private TextMeshProUGUI ammoTextField;
        [SerializeField] private Text ammoTextField;

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
            if (weaponImage == null) return;

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
}

