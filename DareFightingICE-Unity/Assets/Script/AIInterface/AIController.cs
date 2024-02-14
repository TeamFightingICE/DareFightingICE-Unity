using System;
using UnityEngine;


public class AIController : MonoBehaviour
{
    public ZenCharacterController characterController;
    private AIScreenData _screenData;
    private bool _isPlayerOne;
    private bool _toggleFlag;
    private Key _inputKey;
    
    //Test
    public void Awake()
    {
        _inputKey = new Key();
    }

    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        // Initialize AI with game data
        _isPlayerOne = true;
        _toggleFlag = true;
        _inputKey = new Key();
    }
    
    public void GetInformation(FrameData frameData)
    {
        
    }

    public void GetScreenData(ScreenData screenData)
    {
        
    }
    
    public void Processing()
    {
        // AI decision-making logic goes here
    }

    public Key Input()
    {
        // Return the AI's input (actions to take)
        // You'll need to define how you want to structure the Key type or use an existing input structure
        
        if (characterController.PlayerNumber)
        {
            _inputKey.B = true;
            _isPlayerOne = true;
            InputManager.Instance.SetInput(true,_inputKey);
            //_inputKey.UpdatePreviousState();
            Debug.Log("AIController.Key.R : " + InputManager.Instance.GetInput(_isPlayerOne).B );
        }
        return InputManager.Instance.GetInput(_isPlayerOne);
    }

    public void Close()
    {
        // Cleanup resources if needed
    }

    public void RoundEnd(RoundResult result)
    {
        // Process round-end information
    }
    
    void Update()
    {
        FrameData frameData = FrameDataManager.Instance.GetFrameData(); // You'll need to define how FrameData is structured and populated
        ScreenData screenData = new ScreenData();
        GetInformation(frameData);
        GetScreenData(screenData);
        Processing();
    }

    
    void ApplyInputToCharacter(Key input)
    {
        // Convert AI input into character actions
        // This is where you'll control the character based on AI decisions
    }
}