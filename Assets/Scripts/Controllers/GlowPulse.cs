using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    private Material mat;
    [SerializeField] private Color baseColor = Color.cyan;
    [SerializeField] private float intensity = 2f;
    [SerializeField] private float speed = 2f;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float emission = (Mathf.Sin(Time.time * speed) + 1.0f) * 0.5f * intensity;
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        mat.SetColor("_EmissionColor", finalColor);
    }
}
