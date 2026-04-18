using UnityEngine;
using UnityEngine.InputSystem;

public enum DroneMode { Collect, Repair }

public class Drone : MonoBehaviour
{
    public DroneMode mode = DroneMode.Collect;
    public float moveSpeed = 9f;
    public float pickupRadius = 1f;
    public float repairRadius = 2.2f;
    public float repairRate = 20f;
    public float hpPerResource = 10f;
    public Transform idleAnchor;
    float repairBuffer;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.tabKey.wasPressedThisFrame)
            mode = mode == DroneMode.Collect ? DroneMode.Repair : DroneMode.Collect;

        Transform goal = PickGoal();
        Vector3 goalPos = goal != null ? goal.position : (idleAnchor != null ? idleAnchor.position : transform.position);

        transform.position = Vector3.MoveTowards(transform.position, goalPos, moveSpeed * Time.deltaTime);
        Vector3 look = goalPos - transform.position;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 6f * Time.deltaTime);

        if (goal == null) return;

        float d = Vector3.Distance(transform.position, goal.position);
        if (mode == DroneMode.Collect && d < pickupRadius)
        {
            var pickup = goal.GetComponent<ResourcePickup>();
            if (pickup != null) pickup.Collect();
        }
        else if (mode == DroneMode.Repair && d < repairRadius)
        {
            var section = goal.GetComponent<ShipSection>();
            if (section != null) TryRepair(section);
        }
    }

    Transform PickGoal()
    {
        if (mode == DroneMode.Collect)
            return NearestByTag("Debris");
        if (ResourceManager.Instance != null && ResourceManager.Instance.resources <= 0)
            return null;
        if (Ship.Instance != null)
        {
            var s = Ship.Instance.GetMostDamagedSection();
            if (s != null) return s.transform;
        }
        return null;
    }

    void TryRepair(ShipSection section)
    {
        if (hpPerResource <= 0f) { section.Repair(repairRate * Time.deltaTime); return; }
        float want = repairRate * Time.deltaTime;
        repairBuffer += want;
        int cost = Mathf.FloorToInt(repairBuffer / hpPerResource);
        if (cost <= 0) return;
        if (ResourceManager.Instance == null) { section.Repair(cost * hpPerResource); repairBuffer -= cost * hpPerResource; return; }
        int spent = 0;
        for (int i = 0; i < cost; i++) { if (ResourceManager.Instance.Spend(1)) spent++; else break; }
        if (spent > 0) section.Repair(spent * hpPerResource);
        repairBuffer -= spent * hpPerResource;
        if (spent < cost) repairBuffer = 0f;
    }

    Transform NearestByTag(string tag)
    {
        GameObject[] objs;
        try { objs = GameObject.FindGameObjectsWithTag(tag); }
        catch { return null; }
        Transform best = null;
        float bestD = float.MaxValue;
        foreach (var o in objs)
        {
            float d = (o.transform.position - transform.position).sqrMagnitude;
            if (d < bestD) { bestD = d; best = o.transform; }
        }
        return best;
    }
}
