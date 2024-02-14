using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceDisplay : MonoBehaviour
{
    /// <summary>
    /// This is for user display (Hp and energy bar in future) it get data from FightingController
    /// </summary>
    public ZenCharacterController player1; // Reference to player 1's controller
    public ZenCharacterController player2; // Reference to player 2's controller
    public float currentFrame;

    [SerializeField] private Image hp1;
    [SerializeField] private Image hp2;
    [SerializeField] private Image energy1;
    [SerializeField] private Image energy2;
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text p1Status;
    [SerializeField] private TMP_Text p2Status;
    
    float deltaTime = 0.0f;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void SetPlayerController(ZenCharacterController p1,ZenCharacterController p2)
    {
        player1 = p1;
        player2 = p2;
    }
    void Update()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        hp1.fillAmount = (float)player1.Hp / GameSetting.Instance.p1Hp;
        hp2.fillAmount = (float)player2.Hp / GameSetting.Instance.p2Hp;
        energy1.fillAmount = (float)player1.Energy / 300;
        energy2.fillAmount = (float)player2.Energy / 300;
        timerText.text = "Frame Limit: " + currentFrame;
        p1Status.text = $"P1 HP : {player1.Hp} Energy : {player1.Energy}";
        p2Status.text = $"P2 HP : {player2.Hp} Energy : {player2.Energy}";
        SetEnergyColor();
    }

    private void SetEnergyColor()
    {
        if (player1.Energy == 300)
        {
            energy1.color = Color.blue;
        }else if (player1.Energy > 150)
        {
            energy1.color = Color.yellow;
        }
        else
        {
            energy1.color = Color.red;
        }
        
        if (player2.Energy == 300)
        {
            energy2.color = Color.blue;
        }else if (player2.Energy > 150)
        {
            energy2.color = Color.yellow;
        }
        else
        {
            energy2.color = Color.red;
        }
    }

    public int GetElaspedFrame()
    {
        float _temp = GameSetting.Instance.frameLimit - currentFrame;
        int result = Mathf.RoundToInt(_temp);
        return result;
    }
    // void OnGUI()
    // {
    //     // Calculate the frames per second
    //     float msec = deltaTime * 1000.0f;
    //     float fps = 1.0f / deltaTime;
    //
    //     // Format the string to display the framerate, player HPs, and frame limit
    //     string fpsText = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    //     string player1HpText = "Player 1 HP: " + player1.Hp;
    //     string player2HpText = "Player 2 HP: " + player2.Hp;
    //     string frameLimitText = "Frame Limit: " + currentFrame;
    //     string player1Energy = "Player 1 Energy: " + player1.Energy;
    //     string player2Energy = "Player 2 Energy: " + player2.Energy;
    //     string player1Combo = "Player 1 Combo: " + player1.currentCombo;
    //     string player2Combo = "Player 2 Combo: " + player2.currentCombo;
    //
    //     // Set up the style for the GUI
    //     GUIStyle style = new GUIStyle();
    //     int w = Screen.width, h = Screen.height;
    //     Rect rect = new Rect(0, 0, w, 200);
    //     style.alignment = TextAnchor.UpperLeft;
    //     style.fontSize = h * 2 / 50;
    //     style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    //
    //     // Draw the information strings on the screen
    //     GUI.Label(rect, fpsText + "\n" + player1HpText + "\n" + player2HpText + "\n" +player1Energy + "\n" + player2Energy + "\n" + player1Combo + "\n" + player2Combo + "\n" + frameLimitText, style);
    // }
}