using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 60f;
    public float lifetime = 3f;
    public float damage = 25f;

    void Start() { Destroy(gameObject, lifetime); }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        var meteor = other.GetComponentInParent<Meteor>();
        if (meteor != null)
        {
            meteor.TakeDamage(damage, transform.position, transform.forward);
            Destroy(gameObject);
        }
    }
}
