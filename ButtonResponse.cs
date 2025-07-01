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
    public string filePath;
    public string fileName;
    private bool recTime = false;
    private bool first = false;
    private bool saveFlag = false;
    private float saveTime;
    private Material m;
    private List<Frame> frameList = new List<Frame>();
    private Frame lastf;
    // Start is called before the first frame update
    void Start()
    {
        m = indicator.GetComponent<MeshRenderer>().material;
        player = new Leap.Controller();
        m.color = readyColour;
    }

    public void ButtonPress()
    {
        if (!recTime && !saveFlag)
        {
            m.color = waitColour;
            Invoke("delayCall", secondsBeforeRecording);
        }
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
            frameList.Add(player.Frame());
            lastf = player.Frame();
            saveTime = Time.time;
            //Debug.Log(Time.time);
            m.color = recColour;
            first = true;
        }
        else if (recTime)
        {
            if (Time.time > saveTime + secondsToRecord)
            {
                //Debug.Log(Time.time);
                //for (int i = 0; i < frameList.Count; i++) //prints every frame in our frameList
                //{
                //    Debug.Log(frameList[i]);
                //}
                first = false;
                recTime = false;
                saveFlag = true;
                m.color = waitColour;
                saveRec();
            }
            else
            {
                if (!(player.Frame().Equals(lastf)))
                {
                    frameList.Add(player.Frame());
                }
                lastf = player.Frame();
            }
        }
    }

    private void saveRec()
    {
        string path = filePath + "\\" + fileName + ".txt";
        string jsonString = "";
        string s;
        for (int i = 0; i < frameList.Count; i++) //loop through every Frame we recorded
        {
            s = createJson(frameList[i]);
            jsonString = jsonString + s;
        }
        frameList.Clear(); //we've got the information we need, so we can delete this
        File.WriteAllText(path, jsonString);
        m.color = readyColour;
        saveFlag = false;
    }

    private string createJson(Frame f) //extract the informtion we want, put it into our saveClass object and convert the object into JSON.
    {
        saveClass x = new saveClass(9);
        //return JsonConvert.SerializeObject(x);
        return JsonUtility.ToJson(x);
    }
}

public class saveClass
{
    public int x;
    public int z = 79;

    public saveClass(int y)
    {
        x = y;
    }
}
