using UnityEngine;

public enum BulletType { Normal, Sniper };
[System.Serializable]
public class BulletData
{
    [Header("Bullet Type")]
    [SerializeField] BulletType type;
    public BulletType Type => type;
    [Header("bullet Speed (m/s)")]
    [SerializeField] float bulletSpeed = 30;
    /// <summary>
    /// 弾の速度 (m/s)
    /// </summary>
    public float BulletSpeed => bulletSpeed;

    [Header("for vanishtime (s)")]
    [SerializeField] float bulletVanishTime = 10;
    /// <summary>
    /// 弾が消えるまでの時間
    /// </summary>
    public float BulletVanishTime => bulletVanishTime;
}
