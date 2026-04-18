using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public int value = 1;
    public float lifetime = 30f;

    void Start()
    {
        if (!gameObject.CompareTag("Debris")) gameObject.tag = "Debris";
        Destroy(gameObject, lifetime);
    }

    public void Collect()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(value);
        Destroy(gameObject);
    }
}
