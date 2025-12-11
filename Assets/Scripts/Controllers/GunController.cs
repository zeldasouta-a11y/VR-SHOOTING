using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRShooting.Data;
using VRShooting.Manager;

namespace VRShooting.Weapon
{
    /// <summary>
    /// 銃の基底クラス
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
    public class GunController : MonoBehaviour,IWeapon
    {
        [SerializeField] GunData gundata;


        // 弾管理
        private int bulletRemaining;
        private int reserveAmmo;

        // 状態
        /// <summary>
        /// トリガーが押され続けているか
        /// </summary>
        private bool isActivate = false;
        /// <summary>
        /// 打てる状態(リロード完了か)
        /// </summary>
        private bool isShootable = true;
        /// <summary>
        /// リロード中か
        /// </summary>
        private bool isReloading = false;
        /// <summary>
        /// フルオートモードか
        /// </summary>
        private bool isFullAuto = false;
        /// <summary>
        /// リロードとは別のクールタイム
        /// </summary>
        private float fireRate = 0;
        /// <summary>
        /// リロード時間の定数
        /// </summary>
        private float reloadConstant = 0;
        /// <summary>
        /// 残弾
        /// </summary>
        private int infiniteAmmo = -1;
        /// <summary>
        /// 自身のRigidbBody
        /// </summary>
        private Rigidbody thisRigidbody;

        // 低残弾の色設定
        [SerializeField] private int lowAmmoThreshold = 3;
        [SerializeField] private Color lowAmmoColor = Color.red;
        private Color normalAmmoColor = Color.white;

        protected virtual void Start()
        {
            bulletRemaining = gundata.MagazineCapacity;
            reserveAmmo = gundata.IsInfiniteAmmo ? infiniteAmmo : gundata.ReserveAmmo;

            // XRイベント
            var xrGrab = gundata.gunModelObject.GetComponent<XRGrabInteractable>();

            xrGrab.activated.AddListener(Activate);
            xrGrab.deactivated.AddListener(Deactivate);
            xrGrab.lastHoverExited.AddListener(HoverExited);
            thisRigidbody = GetComponent<Rigidbody>();
            // UI初期化
            if (gundata.RemainText) normalAmmoColor = gundata.RemainText.color;
            if (gundata.ReloadText) gundata.ReloadText.gameObject.SetActive(false);
            if (gundata.ReloadProgress)
            {
                gundata.ReloadProgress.fillAmount = 0f;
                gundata.ReloadProgress.gameObject.SetActive(false);
            }
            fireRate = gundata.FireRate;
            reloadConstant = gundata.ReloadConstant;
            UpdateUI();

        }
        protected virtual void OnEnable()
        {
            //イベント購読
            ManagerLocator.Instance.Game.OnFullAutoChanged += OnFullAutoHandle;
        }
        protected virtual void OnDisable()
        {
            if (ManagerLocator.Instance.Game != null)
            {
                //イベント解除
                ManagerLocator.Instance.Game.OnFullAutoChanged -= OnFullAutoHandle;
            }

        }

        protected virtual void Update()
        {
            // フルオート連射
            if (!(isFullAuto && isActivate)) return;

            if (bulletRemaining <= 0)
            {
                StartReload();
                UpdateUI();
                return;
            }
            if (isShootable)
            {
                WeaponShot();
                UpdateUI();
                return;
            }
        }
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="_data"></param>
        public void Init(GunData _data) { gundata = _data; }
        /// <summary>
        /// フルオートモードの変更
        /// </summary>
        /// <param name="mode"></param>
        private void OnFullAutoHandle(bool mode)
        {
            if (mode)
            {
                isFullAuto = true;
                fireRate = gundata.FullAutoFireRate;
                reloadConstant = gundata.FillAutoReloadConstant;
                gundata.ShootSound.volume = 0.3f;
                gundata.ReloadSound.volume = 0;
            }
            else
            {
                isFullAuto = false;
                fireRate = gundata.FireRate;
                reloadConstant = gundata.ReloadConstant;
                gundata.ShootSound.volume = 1.0f;
                gundata.ReloadSound.volume = 1.0f;
            }
        }

        public void Activate(ActivateEventArgs args)
        {
            isActivate = true;
            if (bulletRemaining <= 0)
            {
                StartReload();
                return;
            }
            if (isFullAuto) { return; }

            if (isShootable)
            {
                WeaponShot();
                UpdateUI();
            }
        }
        /// <summary>
        /// 銃を撃つ処理(仮想関数)
        /// </summary>
        public virtual void WeaponShot()
        {
            GunShotFire();
            StartCoroutine(FireRountine());   
        }

        public void Deactivate(DeactivateEventArgs args) { isActivate = false; }
        public void HoverExited(HoverExitEventArgs args)
        {

            isActivate = false;
        }
        public void GunRespawn()
        {
            thisRigidbody.linearVelocity = Vector3.zero;
            thisRigidbody.angularVelocity = Vector3.zero;
            this.transform.localPosition = gundata.GunRespawnPoint.localPosition;
        }
        private void GunShotFire()
        {
            if (bulletRemaining <= 0) { return; }
            bulletRemaining--;
            gundata.ShootSound?.Play();
            ShootAmmo();

        }
        private IEnumerator FireRountine()
        {
            isShootable = false;
            yield return new WaitForSeconds(fireRate);
            isShootable = true;
        }

        private void StartReload()
        {
            gundata.ReloadSound?.Play();
            if (isReloading) return;

            int need = gundata.MagazineCapacity - bulletRemaining;
            if (need <= 0) return;             // 既に満タン
            if (reserveAmmo <= 0)
            {
                if (!gundata.IsInfiniteAmmo) return;      // 予備弾なしかつ有限設定
            }
            int load = gundata.IsInfiniteAmmo ? need : Mathf.Min(need, reserveAmmo);

            float seconds = load * reloadConstant / 1000f;

            StartCoroutine(ReloadRoutine(load, seconds));
        }

        private IEnumerator ReloadRoutine(int load, float seconds)
        {
            isReloading = true;
            // UI: 開始
            if (gundata.ReloadText) gundata.ReloadText.gameObject.SetActive(true);
            if (gundata.ReloadProgress)
            {
                gundata.ReloadProgress.fillAmount = 0f;
                gundata.ReloadProgress.gameObject.SetActive(true);
            }

            float t = 0f;
            while (t < seconds)
            {
                t += Time.deltaTime;
                if (gundata.ReloadProgress)
                    gundata.ReloadProgress.fillAmount = Mathf.Clamp01(t / seconds);
                yield return null;
            }

            bulletRemaining += load;
            if (!gundata.IsInfiniteAmmo)
            {
                reserveAmmo -= load;
            }


            isReloading = false;

            // UI: 終了
            if (gundata.ReloadText) gundata.ReloadText.gameObject.SetActive(false);
            if (gundata.ReloadProgress)
            {
                gundata.ReloadProgress.fillAmount = 0f;
                gundata.ReloadProgress.gameObject.SetActive(false);
            }
            UpdateUI();
        }

        // 弾生成
        private void ShootAmmo()
        {
            ManagerLocator.Instance.Bullet.ActiveBullet
                (gundata.BulletType,
                gundata.MuzzlePos.position,
                gundata.MuzzlePos.rotation
                );
        }

        // UIまとめて更新
        private void UpdateUI()
        {
            string reserveAmmoText = gundata.IsInfiniteAmmo ? "∞" : reserveAmmo.ToString();
            if (gundata.RemainText)
            {
                gundata.RemainText.text = $"{bulletRemaining}/{gundata.MagazineCapacity} ({reserveAmmoText})";
                gundata.RemainText.color = (bulletRemaining <= lowAmmoThreshold) ? lowAmmoColor : normalAmmoColor;
            }
        }
        [OnInspectorButton]
        public void ReplenishAmmo(int replenish)
        {
            reserveAmmo += replenish;
        }

        
    }
}
