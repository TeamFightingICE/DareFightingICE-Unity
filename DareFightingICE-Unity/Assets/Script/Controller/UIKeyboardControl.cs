using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// This used for control keyboard inside launch and start only
/// </summary>
public class UIKeyboardControl : MonoBehaviour
{
    public Selectable firstButton; // Assign this in the inspector with your first button
    private Selectable lastButton;
    public GameObject gameController;
    private StartController startController;
    private float nextTimeAllowedToPress = 0f; // Timer to track delay
    public float delayBetweenPresses = 0.5f; // Delay in seconds

    void Start()
    {
        startController = gameController.GetComponent<StartController>();
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            lastButton = firstButton;
        }
    }

    public void ActiveSwitch(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Back()
    {
        SceneManager.LoadScene("Launch");
    }
    void Update()
    {
        if (Time.time >= nextTimeAllowedToPress)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Button selectedButton = EventSystem.current.currentSelectedGameObject != null ? EventSystem.current.currentSelectedGameObject.GetComponent<Button>() : null;
                if (selectedButton == null)
                {
                    EventSystem.current.SetSelectedGameObject(lastButton.gameObject);
                }
                Selectable next = lastButton.gameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (next != null)
                {
                    EventSystem.current.SetSelectedGameObject(next.gameObject);
                    lastButton = next;
                    nextTimeAllowedToPress = Time.time + delayBetweenPresses; // Reset the timer
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Button selectedButton = EventSystem.current.currentSelectedGameObject != null ? EventSystem.current.currentSelectedGameObject.GetComponent<Button>() : null;
                if (selectedButton == null)
                {
                    EventSystem.current.SetSelectedGameObject(lastButton.gameObject);
                }
                Selectable previous = lastButton.gameObject.GetComponent<Selectable>().FindSelectableOnUp();
                if (previous != null)
                {
                    EventSystem.current.SetSelectedGameObject(previous.gameObject);
                    lastButton = previous;
                    nextTimeAllowedToPress = Time.time + delayBetweenPresses; 
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            Button selectedButton = EventSystem.current.currentSelectedGameObject != null ? EventSystem.current.currentSelectedGameObject.GetComponent<Button>() : null;

            if (selectedButton == null && lastButton != null)
            {
                selectedButton = lastButton.GetComponent<Button>();
                EventSystem.current.SetSelectedGameObject(lastButton.gameObject);
            }

            if (selectedButton != null)
            {
                selectedButton.onClick.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && startController != null)
        {
            if (EventSystem.current.currentSelectedGameObject == startController.p1ControlBtn.gameObject)
            {
                startController.SelectControl(1, -1);
            }
            else if (EventSystem.current.currentSelectedGameObject == startController.p2ControlBtn.gameObject)
            {
                startController.SelectControl(2, -1);
            }
            else if(EventSystem.current.currentSelectedGameObject == startController.RepeatCountText.gameObject)
            {
                startController.SelectRepeatCount(-1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && startController != null)
        {
            if (EventSystem.current.currentSelectedGameObject == startController.p1ControlBtn.gameObject)
            {
                startController.SelectControl(1, 1);
            }
            else if (EventSystem.current.currentSelectedGameObject == startController.p2ControlBtn.gameObject)
            {
                startController.SelectControl(2, 1);
            }
             else if(EventSystem.current.currentSelectedGameObject == startController.RepeatCountText.gameObject)
            {
                startController.SelectRepeatCount(1);
            }
        }
    }
}
