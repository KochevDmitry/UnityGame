using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public int value = 1;
    public float lifetime = 15f;
    public float blinkStart = 10f;
    public float blinkPeriod = 1.2f;

    float spawnTime;
    Vector3 baseScale;

    void Start()
    {
        if (!gameObject.CompareTag("Debris")) gameObject.tag = "Debris";
        spawnTime = Time.time;
        baseScale = transform.localScale;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float age = Time.time - spawnTime;
        if (age < blinkStart) return;
        float k = Mathf.Abs(Mathf.Sin((age - blinkStart) * Mathf.PI / blinkPeriod));
        transform.localScale = baseScale * k;
    }

    public void Collect()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(value);
        Destroy(gameObject);
    }
}
