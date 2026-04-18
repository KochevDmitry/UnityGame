using UnityEngine;

public class Meteor : MonoBehaviour
{
    public int size = 2;                   // 2=large, 1=medium, 0=small
    public float health = 50f;
    public float damageToShip = 25f;
    public int fragmentsOnDestroy = 3;
    public float fragmentImpulse = 4f;
    public GameObject fragmentPrefab;
    public GameObject explosionPrefab;
    public GameObject debrisPrefab;

    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    public void Launch(Vector3 velocity)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = velocity;
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDir)
    {
        health -= amount;
        if (health <= 0f) Shatter(hitPoint, hitDir);
    }

    void Shatter(Vector3 hitPoint, Vector3 hitDir)
    {
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Vector3 baseVel = rb != null ? rb.linearVelocity * 0.5f : Vector3.zero;

        if (size > 0 && fragmentPrefab != null)
        {
            for (int i = 0; i < fragmentsOnDestroy; i++)
            {
                Vector3 offset = Random.insideUnitSphere * 0.8f;
                var frag = Instantiate(fragmentPrefab, transform.position + offset, Random.rotation);
                var m = frag.GetComponent<Meteor>();
                if (m != null)
                {
                    Vector3 dir = (hitDir + Random.insideUnitSphere * 0.8f).normalized;
                    m.Launch(baseVel + dir * fragmentImpulse);
                }
            }
        }
        else if (debrisPrefab != null)
        {
            Instantiate(debrisPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision c)
    {
        var ship = c.collider.GetComponentInParent<Ship>();
        if (ship == null) return;
        var section = c.collider.GetComponentInParent<ShipSection>();
        if (section != null) section.TakeDamage(damageToShip);
        var contact = c.GetContact(0);
        Shatter(contact.point, -contact.normal);
    }
}
