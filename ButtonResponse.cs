using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class ButtonResponse : MonoBehaviour
{
    Leap.Controller player;


    // Start is called before the first frame update
    void Start()
    {
        player = new Leap.Controller();
    }

    public void ButtonPress()
    {
        Frame f = player.Frame(); 
        for (int i = 0; i < f.Hands.Count; i++) //loop through every hand in the frame
        {
            for (int j = 0; j < f.Hands[i].fingers.Length; j++) //in the hand, go through every finger
            {
                if (f.Hands[i].fingers[j] != null)
                {
                    for (int k = 0; k < f.Hands[i].fingers[j].bones.Length; k++) //in our finger, go through every bone
                    {
                        Debug.Log(f.Hands[i].fingers[j].bones[k].ToString() + " on hand: " + i + " on finger: " + j);
                    }
                }

            }
        }
    }

}
