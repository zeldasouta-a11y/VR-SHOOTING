using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GunController : MonoBehaviour
{
    [SerializeField] GunData gundata;

    // 弾管理
    private int bulletRemaining;
    private int reserveAmmo;

    // 状態
    private bool isActivate = false;
    private bool isShootable = true;
    private bool isReloading = false;
    private bool isFullAuto = false;

    private float fireRate = 0;
    private float reloadConstant = 0;
    private int infiniteAmmo = -1;

    // 低残弾の色設定
    [SerializeField] private int lowAmmoThreshold = 3;
    [SerializeField] private Color lowAmmoColor = Color.red;
    private Color normalAmmoColor = Color.white;

    void Start()
    {
        bulletRemaining = gundata.MagazineCapacity;
        reserveAmmo = gundata.IsInfiniteAmmo? infiniteAmmo:gundata.ReserveAmmo;

        // XRイベント
        var xrGrab = gundata.gunModelObject.GetComponent<XRGrabInteractable>();
        if (xrGrab == null) xrGrab = gundata.gunModelObject.AddComponent<XRGrabInteractable>();
        xrGrab.activated.AddListener(Activate);
        xrGrab.deactivated.AddListener(Deactivate);
        xrGrab.hoverExited.AddListener(HoverExited);
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
    private void OnEnable()
    {
        //イベント購読
        ManagerLocator.Instance.Game.OnFullAutoChanged += OnFullAutoHandle;
    }
    void OnDisable()
    {
        if(ManagerLocator.Instance.Game != null)
        {
            //イベント解除
            ManagerLocator.Instance.Game.OnFullAutoChanged -= OnFullAutoHandle;
        }
        
    }

    void Update()
    {
        // フルオート連射
        if (!(isFullAuto && isActivate))return;
        
        if (bulletRemaining <= 0)
        {
            StartReload();
            UpdateUI();
            return;
        }
        if (isShootable)
        {
            GunShotFire();
            StartCoroutine(FireRountine());
            UpdateUI();
            return;
        }
    }

    public void Init(GunData _data) { gundata = _data; }

    private void OnFullAutoHandle(bool mode)
    {
        if (mode)
        {
            isFullAuto = true;
            gundata.FullAutoSound?.Play();
            fireRate = gundata.FullAutoFireRate;
            reloadConstant = gundata.FillAutoReloadConstant;
        }
        else
        {
            isFullAuto = false;
            gundata.FullAutoSound?.Stop();
            fireRate = gundata.FireRate;
            reloadConstant = gundata.ReloadConstant;
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
        if (ManagerLocator.Instance.Game.IsFullAutoMode) { return; }

        if (isShootable)
        {
            GunShotFire();
            StartCoroutine(FireRountine());
            UpdateUI();
        }
    }

    public void Deactivate(DeactivateEventArgs args) { isActivate = false; }
    public void HoverExited(HoverExitEventArgs args) { isActivate = false; }

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
        int load = gundata.IsInfiniteAmmo? need : Mathf.Min(need, reserveAmmo);
        
        float seconds = load * reloadConstant  / 1000f;

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
        ManagerLocator.Instance.Bullet.ActiveBullet(gundata.BulletType,gundata.MuzzlePos.position,gundata.MuzzlePos.rotation);
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