using UnityEngine;

public class ShipSection : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    public bool IsDestroyed => health <= 0f;
    public float HealthRatio => Mathf.Clamp01(health / maxHealth);

    void Awake() { health = maxHealth; }

    public void TakeDamage(float amount)
    {
        health = Mathf.Max(0f, health - amount);
        RefreshTag();
    }

    public void Repair(float amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        RefreshTag();
    }

    void RefreshTag()
    {
        bool damaged = health < maxHealth && health > 0f;
        string want = damaged ? "DamagedSection" : "Untagged";
        if (gameObject.tag != want) gameObject.tag = want;
    }
}
