using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class GunController : MonoBehaviour
{
    [SerializeField] GunData gundata;

    private int bulletRemaining;
    private bool isShooting = false;
    private bool isfullAutoPlaying = false;
    private bool isReloading = false;
    public bool isAsync = false;
    private int reloadSeconds;
    private int reloadMilisecons ;


    public void Start()
    {
        bulletRemaining = gundata.MagazineCapacity;
        //関数設定
        XRGrabInteractable xrGrab = gundata.gunModelObject.GetComponent<XRGrabInteractable>();
        if (xrGrab == null) xrGrab = gundata.gunModelObject.AddComponent<XRGrabInteractable>();

        xrGrab.activated.AddListener(Activate);
        xrGrab.deactivated.AddListener(Deactivate);
        xrGrab.hoverExited.AddListener(HoverExited);
    }

    void Update()
    {
        Debug.Log($"isShooting={isShooting}");
        if (gundata.IsFullAuto && isShooting)
        {
            ShootAmmo();
            if (!isfullAutoPlaying)
            {
                gundata.FullAutoSound?.Play();
                Debug.Log("FullAutoSound!!!");
                isfullAutoPlaying = true;
            }
        }
        else
        {
            if (isfullAutoPlaying)
            {
                gundata.FullAutoSound?.Stop();
            }
            isfullAutoPlaying = false;
            
        }

        if (gundata.RemainText)
            gundata.RemainText.text = bulletRemaining.ToString();
    }
    public void Init(GunData _data)
    {
        gundata = _data;
    }
    /// <summary>
    /// VRコントローラーのトリガーが握られた時に呼び出す。
    /// </summary>
    [OnInspectorButton("Shoot Active")]
    public void Activate(ActivateEventArgs args)
    {
        if (bulletRemaining <= 0 && !gundata.IsFullAuto)
        {
            gundata.ReloadSound?.Play();
            if (isReloading) return;
            isReloading = true;
            reloadSeconds = gundata.MagazineCapacity - bulletRemaining;
            reloadMilisecons = reloadSeconds * gundata.ReloadConstant;
            if (isAsync)
            {
                Debug.Log("Async Reload");
                Task.Run(() => ReloadAsync(reloadMilisecons));
                Debug.Log("Reloaded");
            }
            else
            {
                Debug.Log("Coroutine Reload");
                StartCoroutine(Reload(reloadSeconds));
                Debug.Log("Reloaded");
            }

            return;
        }
        bulletRemaining--;
        if (!gundata.IsFullAuto) gundata.ShootSound?.Play();
        isShooting = true;
        ShootAmmo();
    }
    public void Deactivate(DeactivateEventArgs args)
    {
        isShooting = false;
        Debug.Log("Deactivate!!!");
    }
    public void HoverExited(HoverExitEventArgs args)
    {
        isShooting = false;
        Debug.Log("HoverExited!!!");
    }

    //メインスレッドなのでUnity推奨
    private IEnumerator Reload(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        bulletRemaining = gundata.MagazineCapacity;
        isReloading = false;
    }
    //別スレッド、Unity非推奨
    private async Task ReloadAsync(float miliseconds)
    {
        await Task.Delay((int)miliseconds);
        bulletRemaining = gundata.MagazineCapacity;
        isReloading = false;
        return;
    }
    /// <summary>
    /// 銃弾を生成する。
    /// </summary>
    public void ShootAmmo()
    {
        //弾のプレハブか銃口位置が設定されていなければ処理を行わず帰る。ついでに煽る。
        if (gundata.BulletPrefab == null ||
            gundata.MuzzlePos == null)
        {
            Debug.Log(" Inspector の設定が間違ってるでww m9(^Д^)ﾌﾟｷﾞｬｰ ");
            return;
        }

        //弾を生成する。
        GameObject bulletObj = Instantiate(gundata.BulletPrefab);

        //弾の位置を、銃口の位置と同一にする。
        bulletObj.transform.position = gundata.MuzzlePos.position;

        //弾の向きを、銃口の向きと同一にする。
        bulletObj.transform.rotation = gundata.MuzzlePos.rotation;
        

    }
}
