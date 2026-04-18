using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    public Transform yaw;
    public Transform muzzle;
    public GameObject projectilePrefab;
    public GameObject missilePrefab;
    public GameObject muzzleFlashPrefab;
    public float fireRate = 5f;
    public float missileCooldown = 2f;
    public float aimPlaneY = 0f;
    public int missileCost = 1;

    float nextShot;
    float nextMissile;
    Camera cam;

    void Awake() { cam = Camera.main; }

    void Update()
    {
        AimAtMouse();

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.isPressed && Time.time >= nextShot)
        {
            nextShot = Time.time + 1f / fireRate;
            SpawnBullet();
        }
        if (mouse.rightButton.wasPressedThisFrame && Time.time >= nextMissile)
        {
            if (ResourceManager.Instance == null || ResourceManager.Instance.Spend(missileCost))
            {
                nextMissile = Time.time + missileCooldown;
                SpawnMissile();
            }
        }
    }

    void AimAtMouse()
    {
        if (cam == null || yaw == null || Mouse.current == null) return;
        Vector2 mp = Mouse.current.position.ReadValue();
        var ray = cam.ScreenPointToRay(mp);
        var plane = new Plane(Vector3.up, new Vector3(0, aimPlaneY, 0));
        if (plane.Raycast(ray, out float t))
        {
            Vector3 hit = ray.GetPoint(t);
            Vector3 dir = hit - yaw.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                yaw.rotation = Quaternion.LookRotation(dir);
        }
    }

    void SpawnBullet()
    {
        if (projectilePrefab == null || muzzle == null) return;
        Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        if (muzzleFlashPrefab != null) Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation);
    }

    void SpawnMissile()
    {
        if (missilePrefab == null || muzzle == null) return;
        Instantiate(missilePrefab, muzzle.position, muzzle.rotation);
    }
}
