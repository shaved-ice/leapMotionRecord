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
    public GameObject handModelLeft;
    public GameObject handModelRight;
    private FrameJson serialized;
    private int frameNum = 0;

    // I believe the three finger bones from palm to tip for each of the fingers of this model is the proximal, intermediate and distal 
    // transforming finger tips seems to do nothing so I have left them out.
    // Start is called before the first frame update
    void Start()
    {
        Type t = typeof(FrameJson);
        JsonSerializer j = new JsonSerializer();
        serialized = new FrameJson();
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
                    serialized = (FrameJson)j.Deserialize(sr, t);
                }
            }
            //Debug.Log(serialized.jsonList[0].handList[0].palmPos);
            //Debug.Log(serialized.JsonList[1]);
        }

        ////simple transform code to test finger movement
        //HandBreakdown Hand = serialized.jsonList[0].handList[0];
        //GameObject finger;
        //GameObject palm;
        //if (Hand.isLeft)
        //{
        //    palm = handModelLeft.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        //    finger = palm.transform.GetChild(1).gameObject;
        //}
        //else
        //{
        //    palm = handModelLeft.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        //    finger = palm.transform.GetChild(1).gameObject;
        //}

        //Debug.Log(handModelLeft.transform.GetChild(0).GetChild(0).gameObject); //prints wrists
        //finger.transform.TransformPoint(Vector3.forward + Vector3.forward);
        //finger.transform.GetChild(0).TransformPoint(Vector3.forward);



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

    // Update is called once per frame
    //void Update()
    //{
    //    TransformHand(serialized.jsonList[0].handList[0]);
    //    TransformHand(serialized.jsonList[0].handList[1]);
    //    frameNum = (frameNum + 1) % serialized.jsonList.Count; //increment the frameNum every time but don't go over the length of the framelist
    //}

    public void TransformHand(HandBreakdown b)
    {
        GameObject hand;
        if (b.isLeft)
        {
            hand = handModelLeft.transform.GetChild(0).gameObject;
        }
        else
        {
            hand = handModelRight.transform.GetChild(0).gameObject;
        }

    }

    public void moveToCoords(GameObject g, Vector3 v)
    {
        Vector3 resv;
        Quaternion resq;
        g.transform.GetPositionAndRotation(out resv, out resq); //this puts the position and rotation in our variables.
        g.transform.Translate((resv.x -  v.x), (resv.y - v.y), (resv.z - v.z), Space.World);
    }

    public void rotateQ(GameObject g, Quaternion q)
    {
        Vector3 resv;
        Quaternion resq;
        Quaternion inverse;
        g.transform.GetPositionAndRotation(out resv, out resq);
        inverse = Quaternion.Inverse(resq); //by applying the inverse rotation to the current rotation I hope to fully reset the object's rotation.
        g.transform.Rotate(inverse.eulerAngles, Space.World); //now we can apply our new rotation
        g.transform.Rotate(q.eulerAngles, Space.World);

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
