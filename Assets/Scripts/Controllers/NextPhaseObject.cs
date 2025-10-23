using UnityEngine;

public class NextPhaseObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnDisable()
    {
        ManagerLocator.Instance.Phase.EndPhase(); 
    }
}
