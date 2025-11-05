using UnityEngine;

public class PlayerContoller : MonoBehaviour
{
    [OnInspectorButton]
    public void PlayerRespawn()
    {
        if (this.gameObject.transform.localPosition.y < -1)
        {
            this.gameObject.transform.localPosition = new Vector3(0, 1, 0);
        }
    }
    void FixedUpdate()
    {
        if (this.gameObject.transform.localPosition.y < -1)
        {
            PlayerRespawn();
        }
    }
}
