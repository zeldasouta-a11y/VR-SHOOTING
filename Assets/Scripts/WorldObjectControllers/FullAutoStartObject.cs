using UnityEngine;

public class FullAutoStartObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnDisable()
    {
        var gameManager = ManagerLocator.Instance.Game;
        if(gameManager != null)
        {
            gameManager.StartFullAuto();
        }
    }
}
