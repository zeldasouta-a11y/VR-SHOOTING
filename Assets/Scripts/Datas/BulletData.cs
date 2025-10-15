using UnityEngine;

[System.Serializable]
public class BulletData
{
    [Header("bullet Speed (m/s)")]
    [SerializeField] float bulletSpeed = 30;
    /// <summary>
    /// �e�̑��x (m/s)
    /// </summary>
    public float BulletSpeed => bulletSpeed;

    [Header("for vanishtime (s)")]
    [SerializeField] float bulletVanishTime = 10;
    /// <summary>
    /// �e��������܂ł̎���
    /// </summary>
    public float BulletVanishTime => bulletVanishTime;
}
