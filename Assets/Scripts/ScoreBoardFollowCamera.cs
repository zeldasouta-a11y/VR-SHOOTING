using NUnit.Framework;
using UnityEngine;

public class ScoreBoardFollowCamera : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] Transform playercamera;
    [SerializeField] float followDictance = 2.0f;
    [SerializeField] float heightOffset = 0;
    [SerializeField] float smoothSpeed = 5.0f;

    void LateUpdate()
    {
        if (!playercamera) return;

        Vector3 foward = playercamera.forward;
        foward.y = 0;
        foward.Normalize();

        transform.rotation = Quaternion.LookRotation(foward);
    }
}
