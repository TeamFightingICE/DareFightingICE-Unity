using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderDetection : MonoBehaviour
{
    public AudioSource BorderAlert;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            if (!BorderAlert.isPlaying)
            {
                BorderAlert.Play();
            }
        }
    }
}
