using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject largeMeteorPrefab;
    public Transform shipTarget;
    public float spawnRadius = 45f;
    public float spawnHeight = 2f;
    public float meteorSpeed = 6f;
    public int waveNumber = 1;
    public int initialCount = 1;
    public float waveInterval = 18f;
    public float perWaveIncrement = 1f;
    public float firstWaveDelay = 5f;
    public int maxCount = 10;

    float nextWave;

    void Start() { nextWave = Time.time + firstWaveDelay; }

    void Update()
    {
        if (Time.time >= nextWave)
        {
            Spawn();
            waveNumber++;
            nextWave = Time.time + waveInterval;
        }
    }

    void Spawn()
    {
        if (largeMeteorPrefab == null) return;
        int count = Mathf.Min(maxCount, initialCount + Mathf.FloorToInt(perWaveIncrement * (waveNumber - 1)));
        for (int i = 0; i < count; i++)
        {
            Vector2 r = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 pos = new Vector3(r.x, spawnHeight, r.y);
            var go = Instantiate(largeMeteorPrefab, pos, Random.rotation);
            var m = go.GetComponent<Meteor>();
            if (m != null && shipTarget != null)
            {
                Vector3 dir = (shipTarget.position - pos).normalized;
                m.Launch(dir * meteorSpeed);
            }
        }
    }
}
