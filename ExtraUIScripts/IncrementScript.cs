using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class IncrementScript : MonoBehaviour
{
    private int increment = 0;
    public TMP_Text incrementText;
    public TMP_Text incrementError;
    private Replay replay;
    // Start is called before the first frame update

    void Start()
    {
        replay = FindFirstObjectByType<Replay>();
    }
    void Update()
    {
        int newIncrement = replay.GetFrameIncrement();
        if (increment != newIncrement)
        {
            increment = newIncrement;
            DisplayIncrement();
        }
    }

    public void UpdateIncrement(string i)
    {
        incrementError.text = "";
        try
        {
            int x = Int32.Parse(i);
            increment = x;
            DisplayIncrement();
            replay.SetFrameIncrement(increment);
        }
        catch
        {
            DisplayError(i);
        }
    }

    private void DisplayIncrement()
    {
        incrementText.text = "Increment: " + increment;
    }

    private void DisplayError(string error)
    {
        incrementError.text = "Error! " + error + " is not an integer.";
    }
}
