using UnityEngine;

[System.Serializable]
public class BulletData
{
    [Header("bullet Speed (m/s)")]
    [SerializeField] float bulletSpeed = 30;
    /// <summary>
    /// ’e‚Ì‘¬“x (m/s)
    /// </summary>
    public float BulletSpeed => bulletSpeed;

    [Header("for vanishtime (s)")]
    [SerializeField] float bulletVanishTime = 10;
    /// <summary>
    /// ’e‚ªÁ‚¦‚é‚Ü‚Å‚ÌŠÔ
    /// </summary>
    public float BulletVanishTime => bulletVanishTime;
}
