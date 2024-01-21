using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FightingController : MonoBehaviour
{
    [SerializeField] private GameObject zen;
    [SerializeField] private GameObject spawnP1;
    [SerializeField] private GameObject spawnP2;
    [SerializeField] private GameObject LeftBorder;
    [SerializeField] private GameObject RightBorder;
    [SerializeField] private float flipThreshold = 2.0f;
    private bool isStart = false;
    public List<GameObject> character;
    private float framelimit;

    [SerializeField] private InterfaceDisplay _display;
    void Start()
    {
        SetupScene();
    }

    private void SetupScene()
    {
        ClearList();
        framelimit = GameSetting.Instance.frameLimit;
        GameObject zen1 = Instantiate(zen, spawnP1.transform.position, spawnP1.transform.rotation);
        zen1.GetComponent<CharacterController>().PlayerNumber = true;
        zen1.GetComponent<CharacterController>().IsFront = true;
        zen1.GetComponent<CharacterController>().Hp = GameSetting.Instance.p1Hp;
        zen1.GetComponent<CharacterController>().Energy = 0;
        GameObject zen2 = Instantiate(zen, spawnP2.transform.position, spawnP2.transform.rotation);
        zen2.GetComponent<CharacterController>().PlayerNumber = false;
        zen2.GetComponent<CharacterController>().IsFront = false;
        zen2.GetComponent<CharacterController>().Hp = GameSetting.Instance.p2Hp;
        zen2.GetComponent<CharacterController>().Energy = 0;
        Vector3 scale = zen2.transform.localScale;
        scale.x *= -1;
        zen2.transform.localScale = scale;
        zen1.GetComponent<CharacterController>().otherPlayer = zen2.GetComponent<CharacterController>();
        zen2.GetComponent<CharacterController>().otherPlayer = zen1.GetComponent<CharacterController>();
        zen1.tag = "Player1";
        zen2.tag = "Player2";
        zen1.GetComponent<CharacterController>().SetTarget("Player2");
        zen2.GetComponent<CharacterController>().SetTarget("Player1");
        character.Add(zen1);
        character.Add(zen2);
        _display.player1 = zen1.GetComponent<CharacterController>();
        _display.player2 = zen2.GetComponent<CharacterController>();
        isStart = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (framelimit <= 0 || CheckField())
        {
            End();
        }
        else if (isStart)
        {
            framelimit -= 1;
            _display.currentFrame = framelimit;
            HandlePositionOverlap();
        }
    }

    private void ClearList()
    {
        foreach (GameObject obj in character)
        {
            Destroy(obj);
        }
        character.Clear();
    }
    void End()
    {
        isStart = false;
        SetupScene();
    }

    bool CheckField()
    {
        GameObject player1 = character[0];
        GameObject player2 = character[1];
        if (player1.GetComponent<CharacterController>().Hp <= 0 || player2.GetComponent<CharacterController>().Hp <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    private void HandlePositionOverlap()
    {
        if (character.Count < 2) return; // Ensure there are two characters

        GameObject player1 = character[0];
        GameObject player2 = character[1];

        // Get positions of the players
        float player1Position = player1.transform.position.x;
        float player2Position = player2.transform.position.x;

        // Calculate the distance between the players
        float distanceBetweenPlayers = Mathf.Abs(player1Position - player2Position);

        // Only flip characters if they are beyond the threshold distance
        if (distanceBetweenPlayers > flipThreshold)
        {
            // Determine if players need to flip based on their relative positions
            if (player1Position < player2Position && !player1.GetComponent<CharacterController>().IsFront)
            {
                FlipCharacter(player1);
            }
            else if (player1Position > player2Position && player1.GetComponent<CharacterController>().IsFront)
            {
                FlipCharacter(player1);
            }

            if (player2Position < player1Position && !player2.GetComponent<CharacterController>().IsFront)
            {
                FlipCharacter(player2);
            }
            else if (player2Position > player1Position && player2.GetComponent<CharacterController>().IsFront)
            {
                FlipCharacter(player2);
            }
        }
    }

    // private void FlipCharacter(GameObject character)
    // {
    //     CharacterController charController = character.GetComponent<CharacterController>();
    //     charController.IsFront = !charController.IsFront;
    //     Vector3 scale = character.transform.localScale;
    //     scale.x *= -1;
    //     character.transform.localScale = scale;
    // }
    
    private void FlipCharacter(GameObject character)
    {
        SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;

            // Optionally, adjust facing direction property if you have one
            CharacterController charController = character.GetComponent<CharacterController>();
            if (charController != null)
            {
                charController.IsFront = !charController.IsFront;
            }
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on " + character.name);
        }
    }
}
