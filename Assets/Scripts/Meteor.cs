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
    public int debrisCount = 1;

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
        baseVel.y = 0f;
        float planeY = transform.position.y;

        if (size > 0 && fragmentPrefab != null)
        {
            for (int i = 0; i < fragmentsOnDestroy; i++)
            {
                Vector2 off2 = Random.insideUnitCircle * 0.8f;
                Vector3 spawnPos = new Vector3(transform.position.x + off2.x, planeY, transform.position.z + off2.y);
                var frag = Instantiate(fragmentPrefab, spawnPos, Random.rotation);
                var m = frag.GetComponent<Meteor>();
                if (m != null)
                {
                    Vector2 jitter = Random.insideUnitCircle * 0.8f;
                    Vector3 dir = new Vector3(hitDir.x + jitter.x, 0f, hitDir.z + jitter.y);
                    if (dir.sqrMagnitude < 0.0001f) dir = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f);
                    dir.Normalize();
                    m.Launch(baseVel + dir * fragmentImpulse);
                }
            }
        }
        else if (debrisPrefab != null)
        {
            int n = Mathf.Max(1, debrisCount);
            for (int i = 0; i < n; i++)
            {
                Vector2 off2 = Random.insideUnitCircle * 0.6f;
                Vector3 spawnPos = new Vector3(transform.position.x + off2.x, planeY, transform.position.z + off2.y);
                Instantiate(debrisPrefab, spawnPos, Quaternion.identity);
            }
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
