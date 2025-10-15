using System.Collections;
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
    private bool isShooting = false;
    private bool isFullAutoPlaying = false;
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

    void Update()
    {
        // フルオート連射
        if (gundata.IsFullAuto && isShooting && !isReloading)
        {
            if (Time.time >= nextShotTime)
            {
                if (bulletRemaining > 0)
                {
                    FireOne();
                    nextShotTime = Time.time + 1f / fireRate;
                    if (!isFullAutoPlaying) { gundata.FullAutoSound?.Play(); isFullAutoPlaying = true; }
                }
                else
                {
                    StartReload();
                }
            }
        }
        else
        {
            if (isFullAutoPlaying) { gundata.FullAutoSound?.Stop(); isFullAutoPlaying = false; }
        }
    }

    public void Init(GunData _data) { gundata = _data; }

    public void Activate(ActivateEventArgs args)
    {
        if (isReloading) return;

        if (!gundata.IsFullAuto)
        {
            if (bulletRemaining <= 0) { StartReload(); return; }
            FireOne();
        }
        else
        {
            isShooting = true;
        }
    }

    public void Deactivate(DeactivateEventArgs args) { isShooting = false; }
    public void HoverExited(HoverExitEventArgs args) { isShooting = false; }

    private void FireOne()
    {
        if (bulletRemaining <= 0) return;

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

        isReloading = true;
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
            Debug.Log("Inspector の設定を確認してください（BulletPrefab / MuzzlePos）");
            return;
        }

        GameObject bulletObj = Instantiate(gundata.BulletPrefab);
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