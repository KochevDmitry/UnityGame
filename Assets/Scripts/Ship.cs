using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public static Ship Instance;
    public List<ShipSection> sections = new List<ShipSection>();

    void Awake() { Instance = this; }

    public ShipSection GetMostDamagedSection()
    {
        ShipSection best = null;
        float bestRatio = 1f;
        foreach (var s in sections)
        {
            if (s == null || s.IsDestroyed) continue;
            float r = s.HealthRatio;
            if (r < 1f && r < bestRatio) { bestRatio = r; best = s; }
        }
        return best;
    }

    public bool IsDestroyed()
    {
        foreach (var s in sections)
            if (s != null && !s.IsDestroyed) return false;
        return sections.Count > 0;
    }
}
