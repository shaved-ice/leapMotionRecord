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
    private Deserialize deserializer;
    // Start is called before the first frame update

    void Start()
    {
        deserializer = FindFirstObjectByType<Deserialize>();
    }
    void Update()
    {
        int newIncrement = deserializer.getFrameIncrement();
        if (increment != newIncrement)
        {
            increment = newIncrement;
            displayIncrement();
        }
    }

    public void updateIncrement(string i)
    {
        incrementError.text = "";
        try
        {
            int x = Int32.Parse(i);
            increment = x;
            displayIncrement();
            deserializer.setFrameIncrement(increment);
        }
        catch
        {
            displayError(i);
        }
    }

    private void displayIncrement()
    {
        incrementText.text = "Increment: " + increment;
    }

    private void displayError(string error)
    {
        incrementError.text = "Error! " + error + " is not an integer.";
    }
}
