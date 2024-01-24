using UnityEngine;

public class InterfaceDisplay : MonoBehaviour
{
    /// <summary>
    /// This is for user display (Hp and energy bar in future) it get data from FightingController
    /// </summary>
    public CharacterController player1; // Reference to player 1's controller
    public CharacterController player2; // Reference to player 2's controller
    public float currentFrame;

    float deltaTime = 0.0f;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
    void Update()
    {
        // Calculate the time taken to render the last frame (delta time) and accumulate it
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        // Calculate the frames per second
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // Format the string to display the framerate, player HPs, and frame limit
        string fpsText = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        string player1HpText = "Player 1 HP: " + player1.Hp;
        string player2HpText = "Player 2 HP: " + player2.Hp;
        string frameLimitText = "Frame Limit: " + currentFrame;

        // Set up the style for the GUI
        GUIStyle style = new GUIStyle();
        int w = Screen.width, h = Screen.height;
        Rect rect = new Rect(0, 0, w, 200);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        // Draw the information strings on the screen
        GUI.Label(rect, fpsText + "\n" + player1HpText + "\n" + player2HpText + "\n" + frameLimitText, style);
    }
}