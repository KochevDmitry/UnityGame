using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject largeMeteorPrefab;
    public Transform shipTarget;
    public float spawnRadius = 45f;
    public float spawnHeight = 2f;
    public float meteorSpeed = 6f;
    public float meteorSpeedMax = 12f;
    public int waveNumber = 1;
    public int initialCount = 1;
    public float waveInterval = 18f;
    public float perWaveIncrement = 1f;
    public float firstWaveDelay = 5f;
    public int maxCount = 10;
    public int finalMaxCount = 20;
    public int speedRampWaves = 10;
    public int countRampWaves = 10;
    public float waveIntervalMin = 12f;
    public int intervalShrinkStart = 28;
    public int intervalShrinkEnd = 35;
    public float damageMultStage1 = 2f;
    public float damageMultStage2 = 3f;
    public int damageRampStart = 35;
    public int damageRampEnd = 40;
    public int damageRampEnd2 = 50;

    public static float DamageMultiplier = 1f;

    float nextWave;

    void Start() { nextWave = Time.time + firstWaveDelay; DamageMultiplier = 1f; }

    void Update()
    {
        if (Time.time >= nextWave)
        {
            Spawn();
            waveNumber++;
            DamageMultiplier = ComputeDamageMultiplier(waveNumber);
            nextWave = Time.time + ComputeInterval(waveNumber);
        }
    }

    float ComputeInterval(int wave)
    {
        if (wave <= intervalShrinkStart) return waveInterval;
        if (wave >= intervalShrinkEnd) return waveIntervalMin;
        float t = (float)(wave - intervalShrinkStart) / Mathf.Max(1, intervalShrinkEnd - intervalShrinkStart);
        return Mathf.Lerp(waveInterval, waveIntervalMin, t);
    }

    float ComputeDamageMultiplier(int wave)
    {
        if (wave <= damageRampStart) return 1f;
        if (wave <= damageRampEnd)
        {
            float t = (float)(wave - damageRampStart) / Mathf.Max(1, damageRampEnd - damageRampStart);
            return Mathf.Lerp(1f, damageMultStage1, t);
        }
        if (wave >= damageRampEnd2) return damageMultStage2;
        float t2 = (float)(wave - damageRampEnd) / Mathf.Max(1, damageRampEnd2 - damageRampEnd);
        return Mathf.Lerp(damageMultStage1, damageMultStage2, t2);
    }

    void Spawn()
    {
        if (largeMeteorPrefab == null) return;

        int count;
        float speed;

        if (waveNumber <= maxCount)
        {
            count = Mathf.Min(maxCount, initialCount + Mathf.FloorToInt(perWaveIncrement * (waveNumber - 1)));
            speed = meteorSpeed;
        }
        else if (waveNumber <= maxCount + speedRampWaves)
        {
            count = maxCount;
            float t = (float)(waveNumber - maxCount) / Mathf.Max(1, speedRampWaves);
            speed = Mathf.Lerp(meteorSpeed, meteorSpeedMax, t);
        }
        else if (waveNumber <= maxCount + speedRampWaves + countRampWaves)
        {
            int extra = waveNumber - (maxCount + speedRampWaves);
            count = Mathf.Min(finalMaxCount, maxCount + extra);
            speed = meteorSpeedMax;
        }
        else
        {
            count = finalMaxCount;
            speed = meteorSpeedMax;
        }

        for (int i = 0; i < count; i++)
        {
            Vector2 r = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 pos = new Vector3(r.x, spawnHeight, r.y);
            var go = Instantiate(largeMeteorPrefab, pos, Random.rotation);
            var m = go.GetComponent<Meteor>();
            if (m != null && shipTarget != null)
            {
                Vector3 dir = (shipTarget.position - pos).normalized;
                m.Launch(dir * speed);
            }
        }
    }
}
