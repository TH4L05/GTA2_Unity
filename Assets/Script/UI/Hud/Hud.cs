/// <author>Thoams Krahl</author>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using ProjectGTA2_Unity.Characters;

namespace ProjectGTA2_Unity.UI
{
    public class Hud : MonoBehaviour
    {
        [Header("Weapon")]
        [SerializeField] private Text moneyTextField;

        [Header("Weapon")]
        [SerializeField] private Image weaponImage;
        [SerializeField] private Text ammoTextField;
        [SerializeField] private Text ammoTextShadowField;

        [Header("Life & Multiplier")]
        [SerializeField] private Text lifesTextField;
        [SerializeField] private Text multiplierTextField;

        [Header("InfoText")]
        [SerializeField] private string[] infoTexts;
        [SerializeField] private Text infoTextField;
        [SerializeField] private PlayableAsset showInfoTextPlayable;

        [Header("Settings")]
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private AudioEventList audioEvents;

        private void Awake()
        {
            WeaponBelt.WeaponChanged += UpdateWeapon;
            WeaponBelt.WeaponUpdate += UpdateAmmo;
            Player.PlayerDied += PlayerDied;
            Player.UpdatePlayerMoney += UpdateMoney;
        }

        private void OnDestroy()
        {
            WeaponBelt.WeaponChanged -= UpdateWeapon;
            WeaponBelt.WeaponUpdate -= UpdateAmmo;
            Player.PlayerDied -= PlayerDied;
            Player.UpdatePlayerMoney -= UpdateMoney;
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

            if (ammo <= 0)
            {
                ammoTextField.gameObject.SetActive(false);
                ammoTextShadowField.gameObject.SetActive(false);
            }
            else
            {
                ammoTextField.gameObject.SetActive(true);
                ammoTextShadowField.gameObject.SetActive(true);
                ammoTextField.text = ammo.ToString();
                ammoTextShadowField.text = ammo.ToString();
            }
        }

        public void ShowInfoText(string text)
        {
            infoTextField.text = text;
            playableDirector.Play(showInfoTextPlayable);
        }

        public void ShowInfoText(int index)
        {
            infoTextField.text = infoTexts[index];
            playableDirector.Play(showInfoTextPlayable);
        }

        private void PlayerDied(DamageType damageType)
        {
            string text = string.Empty;

            switch (damageType)
            {
                case DamageType.Invalid:
                    break;

                case DamageType.Normal:
                    text = infoTexts[0];
                    break;

                case DamageType.Fire:
                    text = infoTexts[2];
                    break;

                case DamageType.Water:
                    text = infoTexts[0];
                    break;

                case DamageType.Electro:
                    break;

                case DamageType.Car:
                    break;

                case DamageType.CopNormal:
                    text = infoTexts[1];
                    break;

                case DamageType.CopGun:
                    break;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(text)) return;
            audioEvents.PlayAudioEventOneShot(text);
            ShowInfoText(text);
        }

        private void UpdateMoney(int amount)
        {
            moneyTextField.text = amount.ToString("00000000");
        }
    }
}

