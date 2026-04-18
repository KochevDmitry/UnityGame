using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ExplosionEffect : MonoBehaviour
{
    public Color color = new Color(1f, 0.55f, 0.15f, 1f);
    public int burstCount = 40;
    public float startSpeed = 8f;
    public float startLifetime = 0.8f;
    public float startSize = 0.3f;
    public float radius = 0.1f;

    void Awake()
    {
        var ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.6f;
        main.loop = false;
        main.startLifetime = startLifetime;
        main.startSpeed = startSpeed;
        main.startSize = startSize;
        main.startColor = color;
        main.stopAction = ParticleSystemStopAction.Destroy;
        main.gravityModifier = 0f;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.burstCount = 1;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, burstCount));

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = radius;

        ps.Play();
    }
}
