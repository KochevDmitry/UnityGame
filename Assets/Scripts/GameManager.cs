using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool gameOver;
    GUIStyle style;

    void Update()
    {
        if (!gameOver && Ship.Instance != null && Ship.Instance.IsDestroyed())
        {
            gameOver = true;
            Time.timeScale = 0f;
        }
    }

    void OnGUI()
    {
        if (!gameOver) return;
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = 40;
            style.normal.textColor = Color.red;
            style.alignment = TextAnchor.MiddleCenter;
        }
        GUI.Label(new Rect(0, Screen.height / 2 - 30, Screen.width, 60), "КОРАБЛЬ УНИЧТОЖЕН", style);
    }
}
