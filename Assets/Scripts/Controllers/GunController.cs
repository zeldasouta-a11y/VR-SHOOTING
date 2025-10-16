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
    private bool isReloading = false;
    // フルオート設定（1秒あたりの発射数）
    [SerializeField] private float fireRate = 8f;
    private float nextShotTime = 0f;

    // 低残弾の色設定
    [SerializeField] private int lowAmmoThreshold = 3;
    [SerializeField] private Color lowAmmoColor = Color.red;
    private Color normalAmmoColor = Color.white;

    void Start()
    {
        bulletRemaining = gundata.MagazineCapacity;
        reserveAmmo = gundata.InitialReserveAmmo;

        // XRイベント
        var xrGrab = gundata.gunModelObject.GetComponent<XRGrabInteractable>();
        if (xrGrab == null) xrGrab = gundata.gunModelObject.AddComponent<XRGrabInteractable>();
        xrGrab.activated.AddListener(Activate);
        xrGrab.deactivated.AddListener(Deactivate);
        xrGrab.hoverExited.AddListener(HoverExited);

        //ローカルイベント
        ManagerLocator.Instance.Game.OnFullAutoChanged += OnFullAutoHandle;

        // UI初期化
        if (gundata.RemainText) normalAmmoColor = gundata.RemainText.color;
        if (gundata.ReloadText) gundata.ReloadText.gameObject.SetActive(false);
        if (gundata.ReloadProgress)
        {
            gundata.ReloadProgress.fillAmount = 0f;
            gundata.ReloadProgress.gameObject.SetActive(false);
        }
        UpdateUI();
    }
    private void OnDestroy()
    {
        //イベント解除
        ManagerLocator.Instance.Game.OnFullAutoChanged -= OnFullAutoHandle;
    }

    void Update()
    {
        // フルオート連射
        if (!(ManagerLocator.Instance.Game.IsFullAutoMode && isActivate)) return;
        if (isReloading) return;
        
        if (bulletRemaining <= 0)
        {
            StartReload();
            Debug.Log("Fuck you");
            return;
        }
        if (Time.time >= nextShotTime)
        {
            Debug.Log("TTT");
            GunShotFire();
            nextShotTime = Time.time + 1f / fireRate;
        }
        return;  
    }

    public void Init(GunData _data) { gundata = _data; }


    private void OnFullAutoHandle(bool mode)
    {
        if (mode)
        {
            gundata.FullAutoSound?.Play();
        }
        else
        {
            gundata.FullAutoSound?.Stop();
        }
    }

    public void Activate(ActivateEventArgs args)
    {
        isActivate = true;
        if (isReloading) return;

        if (!ManagerLocator.Instance.Game.IsFullAutoMode)
        {
            if (bulletRemaining <= 0) { StartReload(); return; }
            GunShotFire();
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
        UpdateUI();
    }

    private void StartReload()
    {
        if (isReloading) return;

        int need = gundata.MagazineCapacity - bulletRemaining;
        if (need <= 0) return;             // 既に満タン
        if (reserveAmmo <= 0) return;      // 予備弾なし

        int load = Mathf.Min(need, reserveAmmo); // 装填できる弾数
        float seconds = load * gundata.ReloadConstant / 1000f;

        
        gundata.ReloadSound?.Play();

        // UI: 開始
        if (gundata.ReloadText) gundata.ReloadText.gameObject.SetActive(true);
        if (gundata.ReloadProgress)
        {
            gundata.ReloadProgress.fillAmount = 0f;
            gundata.ReloadProgress.gameObject.SetActive(true);
        }

        StartCoroutine(ReloadRoutine(load, seconds));
    }

    private IEnumerator ReloadRoutine(int load, float seconds)
    {
        isReloading = true;
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            if (gundata.ReloadProgress)
                gundata.ReloadProgress.fillAmount = Mathf.Clamp01(t / seconds);
            yield return null;
        }

        bulletRemaining += load;
        reserveAmmo -= load;

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
        if (gundata.BulletPrefab == null || gundata.MuzzlePos == null)
        {
            Debug.Log("Inspector の設定を確認してください　焼き肉食べ放題（BulletPrefab / MuzzlePos）");
            return;
        }

        GameObject bulletObj = Instantiate(gundata.BulletPrefab);
        BulletController bullet = bulletObj.GetComponent<BulletController>();
        if(bullet == null) bullet = bulletObj.AddComponent<BulletController>();

        bullet.Init(gundata.BulletData);

        //弾の位置を、銃口の位置と同一にする。
        bulletObj.transform.position = gundata.MuzzlePos.position;
        bulletObj.transform.rotation = gundata.MuzzlePos.rotation;
    }

    // UIまとめて更新
    private void UpdateUI()
    {
        if (gundata.RemainText)
        {
            gundata.RemainText.text = $"{bulletRemaining}/{gundata.MagazineCapacity} ({reserveAmmo})";
            gundata.RemainText.color = (bulletRemaining <= lowAmmoThreshold) ? lowAmmoColor : normalAmmoColor;
        }
    }
}