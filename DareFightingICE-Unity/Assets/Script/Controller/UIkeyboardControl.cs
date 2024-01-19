using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIKeyboardControl : MonoBehaviour
{
    public Selectable firstButton; // Assign this in the inspector with your first button
    private Selectable lastButton;
    
    private float nextTimeAllowedToPress = 0f; // Timer to track delay
    public float delayBetweenPresses = 0.5f; // Delay in seconds

    void Start()
    {
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
        SceneManager.LoadScene("Start");
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
    }
}