using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 22f;
    public float turnRate = 220f;
    public float acquireRadius = 80f;
    public float lifetime = 6f;
    public float damage = 80f;
    public float explosionRadius = 4f;
    public LayerMask meteorMask;
    public GameObject explosionPrefab;

    Transform target;
    float reacquireTimer;

    void Start()
    {
        Destroy(gameObject, lifetime);
        FindNearestMeteor();
    }

    void FindNearestMeteor()
    {
        int mask = meteorMask.value == 0 ? ~0 : meteorMask.value;
        var hits = Physics.OverlapSphere(transform.position, acquireRadius, mask);
        float best = float.MaxValue;
        foreach (var h in hits)
        {
            if (h.GetComponentInParent<Meteor>() == null) continue;
            float d = (h.transform.position - transform.position).sqrMagnitude;
            if (d < best) { best = d; target = h.transform; }
        }
    }

    void Update()
    {
        reacquireTimer -= Time.deltaTime;
        if ((target == null || !target.gameObject.activeInHierarchy) && reacquireTimer <= 0f)
        {
            FindNearestMeteor();
            reacquireTimer = 0.25f;
        }

        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion want = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, want, turnRate * Time.deltaTime);
            }
        }
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Meteor>() == null) return;
        Detonate();
    }

    void Detonate()
    {
        int mask = meteorMask.value == 0 ? ~0 : meteorMask.value;
        var hits = Physics.OverlapSphere(transform.position, explosionRadius, mask);
        foreach (var h in hits)
        {
            var m = h.GetComponentInParent<Meteor>();
            if (m != null)
            {
                Vector3 dir = (m.transform.position - transform.position).normalized;
                m.TakeDamage(damage, transform.position, dir);
            }
        }
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
