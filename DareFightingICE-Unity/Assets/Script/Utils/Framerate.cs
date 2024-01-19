using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Framerate : MonoBehaviour
{
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

        // Format the string to display the framerate
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        // Set up the style for the GUI
        GUIStyle style = new GUIStyle();
        int w = Screen.width, h = Screen.height;
        Rect rect = new Rect(0, 0, w, 200);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        // Draw the framerate string on the screen
        GUI.Label(rect, text, style);
    }
}
