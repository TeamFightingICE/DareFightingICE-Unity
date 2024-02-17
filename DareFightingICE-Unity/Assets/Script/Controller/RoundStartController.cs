using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundStartController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(roundStart());
    }

    IEnumerator roundStart() 
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("Gameplay");
    }
}
