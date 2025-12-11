using UnityEngine;

/// <summary>
/// ヒットを受信するもの
/// </summary>
public interface IHitReceiver
{
    public void OnHitNotify(IHitSender hitsource);
}
