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
        [Header("Money")]
        [SerializeField] private Text moneyTextField;

        [Header("Health")]
        [SerializeField] private Image health0;
        [SerializeField] private Image health1;
        [SerializeField] private Image health2;
        [SerializeField] private Image health3;
        [SerializeField] private Image health4;
        [SerializeField] private Sprite healthFull;
        [SerializeField] private Sprite healthEmpty;

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
            Player.OnDeath += PlayerDied;
            Player.OnUpdateMoney += UpdateMoney;
            Player.OnHealthChanged += UpdateHealth;
        }

        private void OnDestroy()
        {
            WeaponBelt.WeaponChanged -= UpdateWeapon;
            WeaponBelt.WeaponUpdate -= UpdateAmmo;
            Player.OnDeath -= PlayerDied;
            Player.OnUpdateMoney -= UpdateMoney;
            Player.OnHealthChanged -= UpdateHealth;
        }

        private void UpdateMoney(int amount)
        {
            if (moneyTextField == null)
            {
                Debug.LogError("Money Text Field Reference is Missing");
                return;
            }
            moneyTextField.text = amount.ToString("00000000");
        }

        private void UpdateHealth(float currenthealth, float maxHealth)
        {
            if (health0 == null || health1 == null || health2 == null || health3 == null | health4 == null) return;
            if (healthFull == null || healthEmpty == null) return;

            float p = currenthealth / maxHealth;

            switch (p)
            {
                case > 0.9f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthFull;
                    health3.sprite = healthFull;
                    health4.sprite = healthFull;
              
                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = Vector3.one;
                    health2.rectTransform.localScale = Vector3.one;
                    health3.rectTransform.localScale = Vector3.one;
                    health4.rectTransform.localScale = Vector3.one;
                    break;

                case > 0.8f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthFull;
                    health3.sprite = healthFull;
                    health4.sprite = healthFull;

                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = Vector3.one;
                    health2.rectTransform.localScale = Vector3.one;
                    health3.rectTransform.localScale = Vector3.one;
                    health4.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
                    break;

                case > 0.7f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthFull;
                    health3.sprite = healthFull;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = Vector3.one;
                    health2.rectTransform.localScale = Vector3.one;
                    health3.rectTransform.localScale = Vector3.one;
                    break;

                case > 0.6f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthFull;
                    health3.sprite = healthFull;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = Vector3.one;
                    health2.rectTransform.localScale = Vector3.one;
                    health3.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
                    break;

                case > 0.5f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthFull;
                    health3.sprite = healthEmpty;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = Vector3.one;
                    health2.rectTransform.localScale = Vector3.one;
                    break;

                case > 0.4f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthFull;
                    health3.sprite = healthEmpty;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = Vector3.one;
                    health2.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
                    break;

                case > 0.3f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthEmpty;
                    health3.sprite = healthEmpty;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = Vector3.one;
                    break;

                case > 0.2f:
                    health0.sprite = healthFull;
                    health1.sprite = healthFull;
                    health2.sprite = healthEmpty;
                    health3.sprite = healthEmpty;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = Vector3.one;
                    health1.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
                    break;

                case > 0.1f:
                    health0.sprite = healthFull;
                    health1.sprite = healthEmpty;
                    health2.sprite = healthEmpty;
                    health3.sprite = healthEmpty;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = Vector3.one;
                    break;

                case > 0f:
                    health0.sprite = healthFull;
                    health1.sprite = healthEmpty;
                    health2.sprite = healthEmpty;
                    health3.sprite = healthEmpty;
                    health4.sprite = healthEmpty;

                    health0.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
                    break;

                default:
                    break;
            }
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
            if (ammoTextField == null)
            {
                Debug.LogError("Ammo Text Field Reference is Missing");
                return;              
            }



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
            if (infoTextField == null)
            {
                Debug.LogError("Text Field Reference is Missing");
                return;
            }
            infoTextField.text = text.ToUpper();

            if (showInfoTextPlayable == null)
            {
                Debug.LogError("Playable Reference is Missing");
                return;
            }
            playableDirector.Play(showInfoTextPlayable);
            var audioEventName = text.Split(' ')[0];
            audioEvents.PlayAudioEventOneShot(audioEventName);
        }

        public void ShowInfoText(int index)
        {
            if (infoTextField == null)
            {
                Debug.LogError("Text Field Reference is Missing");
                return;
            }
            infoTextField.text = infoTexts[index].ToUpper();

            if (showInfoTextPlayable == null)
            {
                Debug.LogError("Playable Reference is Missing");
                return;
            }
            playableDirector.Play(showInfoTextPlayable);
            var audioEventName = infoTexts[index].Split(' ')[0];
            audioEvents.PlayAudioEventOneShot(audioEventName);
        }

        private void PlayerDied(DamageType damageType, string playerName)
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
                    text = infoTexts[0];
                    break;

                case DamageType.Water:
                    text = infoTexts[0];
                    break;

                case DamageType.Electro:
                    break;

                case DamageType.Car:
                    break;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(text)) return;           
            ShowInfoText(text);
        }

    }
}

