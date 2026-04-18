using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public WaveManager waveManager;
    public Drone drone;

    const string BestKey = "BestWave";

    GUIStyle style;
    GUIStyle bigStyle;
    GUIStyle btnStyle;
    int recordedBest;
    bool savedThisRun;

    void Awake() { recordedBest = PlayerPrefs.GetInt(BestKey, 0); }

    void Update()
    {
        if (savedThisRun || waveManager == null || Ship.Instance == null) return;
        if (Ship.Instance.IsDestroyed())
        {
            int reached = Mathf.Max(1, waveManager.waveNumber - 1);
            if (reached > recordedBest)
            {
                recordedBest = reached;
                PlayerPrefs.SetInt(BestKey, recordedBest);
                PlayerPrefs.Save();
            }
            savedThisRun = true;
            Time.timeScale = 0f;
        }
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = 18;
            style.normal.textColor = Color.white;
            bigStyle = new GUIStyle(GUI.skin.label);
            bigStyle.fontSize = 40;
            bigStyle.alignment = TextAnchor.MiddleCenter;
            bigStyle.normal.textColor = Color.white;
            btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 22;
        }

        int y = 10;
        if (Ship.Instance != null)
        {
            foreach (var s in Ship.Instance.sections)
            {
                if (s == null) continue;
                GUI.Label(new Rect(10, y, 400, 24),
                    $"{s.name}: {Mathf.CeilToInt(s.HealthRatio * 100f)}%", style);
                y += 22;
            }
        }
        if (ResourceManager.Instance != null)
        {
            GUI.Label(new Rect(10, y, 300, 24), $"Ресурсы: {ResourceManager.Instance.resources}", style);
            y += 22;
        }
        if (drone != null)
        {
            string modeRu = drone.mode == DroneMode.Collect ? "Сбор" : "Ремонт";
            GUI.Label(new Rect(10, y, 300, 24), $"Дрон: {modeRu} (Tab)", style);
            y += 22;
        }
        if (waveManager != null)
        {
            GUI.Label(new Rect(10, y, 300, 24), $"Волна: {waveManager.waveNumber}", style);
            y += 22;
        }
        GUI.Label(new Rect(10, y, 300, 24), $"Рекорд: {recordedBest}", style);

        if (Ship.Instance != null && Ship.Instance.IsDestroyed())
        {
            float w = Screen.width, h = Screen.height;
            GUI.Label(new Rect(0, h * 0.3f + 60, w, 30),
                $"Вы дошли до волны {Mathf.Max(1, (waveManager != null ? waveManager.waveNumber : 1) - 1)}", style);
            if (GUI.Button(new Rect(w * 0.5f - 110, h * 0.7f, 220, 50), "Начать заново", btnStyle))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
