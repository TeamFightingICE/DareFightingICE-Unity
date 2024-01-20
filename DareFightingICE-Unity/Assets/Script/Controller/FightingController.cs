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
        character.Add(zen1);
        character.Add(zen2);
        _display.player1 = zen1.GetComponent<CharacterController>();
        _display.player2 = zen2.GetComponent<CharacterController>();
        isStart = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (framelimit <= 0)
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
    
    private void HandlePositionOverlap()
    {
        if (character.Count < 2) return; // Ensure there are two characters

        GameObject player1 = character[0];
        GameObject player2 = character[1];

        // Calculate distances to borders
        float player1DistanceToLeft = Mathf.Abs(player1.transform.position.x - LeftBorder.transform.position.x);
        float player1DistanceToRight = Mathf.Abs(player1.transform.position.x - RightBorder.transform.position.x);
        float player2DistanceToLeft = Mathf.Abs(player2.transform.position.x - LeftBorder.transform.position.x);
        float player2DistanceToRight = Mathf.Abs(player2.transform.position.x - RightBorder.transform.position.x);

        // Determine if players need to flip based on their relative positions to the borders
        if (player1DistanceToLeft < player2DistanceToLeft && !player1.GetComponent<CharacterController>().IsFront)
        {
            FlipCharacter(player1);
        }
        else if (player1DistanceToRight < player2DistanceToRight && player1.GetComponent<CharacterController>().IsFront)
        {
            FlipCharacter(player1);
        }

        if (player2DistanceToLeft < player1DistanceToLeft && !player2.GetComponent<CharacterController>().IsFront)
        {
            FlipCharacter(player2);
        }
        else if (player2DistanceToRight < player1DistanceToRight && player2.GetComponent<CharacterController>().IsFront)
        {
            FlipCharacter(player2);
        }
    }

    private void FlipCharacter(GameObject character)
    {
        CharacterController charController = character.GetComponent<CharacterController>();
        charController.IsFront = !charController.IsFront;
        Vector3 scale = character.transform.localScale;
        scale.x *= -1;
        character.transform.localScale = scale;
    }
}
