using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;

public class Deserialize : MonoBehaviour
{

    public string directoryPath;
    public string fileName;
    public GameObject Indicator;
    public Color readyColour = Color.green; //this allows users to change these colours to account for colorblindess etc. 
    public Color waitColour = new Color(0.6f, 0.6f, 0.6f);
    private Material m; 

    // Start is called before the first frame update
    void Start()
    {
        m = Indicator.GetComponent<MeshRenderer>().material;
        m.color = readyColour;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPress()
    {
        m.color = waitColour;
        Type t = typeof(FrameJson);
        JsonSerializer j = new JsonSerializer(); 
        FrameJson serialized;
        byte[] arr = new byte[32];
        UTF8Encoding ut = new UTF8Encoding();
        string filePath = directoryPath + "\\" + fileName + ".txt";
        if (!(File.Exists(filePath)))
        {
            Debug.Log("File doesn't exist!");
        }
        else
        {
            using (Stream fs = File.OpenRead(filePath))
            {
                using (TextReader sr = new StreamReader(fs))
                {
                    //serial = sr.ReadLine();
                    serialized = (FrameJson) j.Deserialize(sr, t);
                }
            }
            Debug.Log(serialized.jsonList[0].handList[0].palmPos);
            //Debug.Log(serialized.JsonList[1]);
        }
        //m.color = readyColour;


        //buffered version attempt:
        //if (!(File.Exists(filePath)))
        //{
        //    Debug.Log("File doesn't exist!");
        //}
        //else
        //{
        //    using (Stream fs = File.OpenRead(filePath))
        //    {
        //        using (BufferedStream b = new BufferedStream(fs, 10))
        //        {
        //            while (r != 0 && x < 10) // read function returns 0 when it reaches the end of the stream 
        //            {
        //                r = b.Read(arr, 0, 1);
        //                x++;
        //            }
        //        }
        //    }
        //}
        //foreach (byte by in arr)
        //{
        //    string st = by.ToString();
        //}
    }

    //including the class structure so I can access data after deserialization.
    public class FrameJson
    {
        public List<FrameBreakdown> jsonList;
        public FrameJson()
        {
            jsonList = new List<FrameBreakdown>();
        }

        public void addf(FrameBreakdown f)
        {
            jsonList.Add(f);
        }
    }

    public class FrameBreakdown
    {
        public List<HandBreakdown> handList = new List<HandBreakdown>();
    }

    public class HandBreakdown
    {
        public FingerBreakdown thumb;
        public FingerBreakdown index;
        public FingerBreakdown middle;
        public FingerBreakdown ring;
        public FingerBreakdown pinky;
        public List<FingerBreakdown> extraFingers;
        public Vector3 palmPos;
        public Vector3 palmVel;
        public Vector3 palmNormal;
        public Vector3 palmDirection;
        public Quaternion handRotation;
        public float pinchDistance;
        public float palmWidth;
        public Vector3 wristPos;
        public bool isLeft;
        public ArmBreakdown arm;
    }

    public class ArmBreakdown
    {
        public Vector3 elbowPos;
        public Vector3 wristPos;

    }

    public class FingerBreakdown
    {
        public BoneBreakdown metacarpal;
        public BoneBreakdown proximal;
        public BoneBreakdown intermediate;
        public BoneBreakdown distal;
        public List<BoneBreakdown> boneList = new List<BoneBreakdown>();
        public Vector3 tipPos;
        public Vector3 direction;
        public float width;
        public float length;
    }

    public class BoneBreakdown
    {
        public Vector3 prevJoint;
        public Vector3 nextJoint;
        public Vector3 center;
        public Vector3 direction;
        public float length;
        public float width;
        public Quaternion rotation;
    }
}
