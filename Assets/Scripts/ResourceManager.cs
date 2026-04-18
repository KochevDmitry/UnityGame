using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int resources;

    void Awake() { Instance = this; }

    public void Add(int amount) { resources += amount; }
    public bool Spend(int amount)
    {
        if (resources < amount) return false;
        resources -= amount;
        return true;
    }
}
