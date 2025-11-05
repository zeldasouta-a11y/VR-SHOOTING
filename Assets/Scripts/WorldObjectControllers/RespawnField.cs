using UnityEngine;

public class RespawnField : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        PlayerContoller contoller = collision.gameObject.GetComponent<PlayerContoller>();
        if (contoller != null)
        {
            contoller.PlayerRespawn();
        }
#if UNITY_EDITOR
        Debug.Log($"collision {collision.gameObject.name}");
#endif
    }
}
