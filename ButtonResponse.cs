using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.IO;
using System.Text.Json;

public class ButtonResponse : MonoBehaviour
{
    Leap.Controller player;
    public GameObject indicator; //place your indicator object here in unity. It simply changes colour to indicate to the player what is happening
    public Color readyColour = Color.green; //this allows users to change these colours to account for colorblindess etc. 
    public Color waitColour = new Color(0.6f, 0.6f, 0.6f);
    public Color recColour = Color.red;
    public int secondsBeforeRecording = 5; //time before program starts recording
    public int secondsToRecord = 5; 
    private bool recTime = false;
    private bool first = false;
    private Material m;
    // Start is called before the first frame update
    void Start()
    {
        m = indicator.GetComponent<MeshRenderer>().material;
        player = new Leap.Controller();
        m.color = readyColour;
    }

    public void ButtonPress()
    {
        Frame f = player.Frame();
        m.color = waitColour;
        Invoke("delayCall", secondsBeforeRecording);
        //for (int i = 0; i < f.Hands.Count; i++) //loop through every hand in the frame
        //{
        //    for (int j = 0; j < f.Hands[i].fingers.Length; j++) //in the hand, go through every finger
        //    {
        //        if (f.Hands[i].fingers[j] != null)
        //        {
        //            for (int k = 0; k < f.Hands[i].fingers[j].bones.Length; k++) //in our finger, go through every bone
        //            {
        //                Debug.Log(f.Hands[i].fingers[j].bones[k].ToString() + " on hand: " + i + " on finger: " + j);
        //            }
        //        }

        //    }
        //}
    }

    private void delayCall()
    {
        recTime = true; 
    }

    void Update()
    {
        if (recTime && !first)
        {
            m.color = recColour;
        }
        else if (recTime)
        {

        }
    }
}
