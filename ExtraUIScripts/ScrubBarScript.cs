using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrubBarScript : MonoBehaviour
{
    private Deserialize deserializer;
    private GameObject scrubber;
    private GameObject Pointer;
    private float ratio;
    private float startOfBar;
    private int frameNum = 0; //initialise to the start
    private int maxSize;
    private int fileNum;
    // Start is called before the first frame update
    void Start()
    {
        deserializer = FindFirstObjectByType<Deserialize>();
        //this.gameObject is the ScrubBar parent object as this script will be placed on that object.
        Pointer = this.gameObject.transform.GetChild(1).gameObject; //this starts off at the start of the bar in my prefab
        startOfBar = Pointer.transform.position.z;
        CalculateRatio();
        UpdateScrubBar();
        fileNum = deserializer.GetFileNum();
    }

    // Update is called once per frame
    void Update()
    {
        if (fileNum != deserializer.GetFileNum()) //file has changed so we need to change the ratio as we have a new maxSize
        {
            CalculateRatio(); //don't update frameNum as it will update or stay as 0 next anyway
        }
        if (deserializer.FrameDiff(frameNum))
        {
            UpdateScrubBar();
        }
    }

    private void UpdateScrubBar()
    {
        Vector3 oldPos = Pointer.transform.position;
        frameNum = deserializer.FrameNum;
        Pointer.transform.position = new Vector3(oldPos.x, oldPos.y, startOfBar + (ratio * frameNum)); //calculate new pointer position based on old position and the new frameNum
    }

    private void CalculateRatio()
    {
        maxSize = deserializer.GetMaxSize();
        ratio = 1.0f / (maxSize - 1);
    }
}
