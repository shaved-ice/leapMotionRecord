using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrubBarScript : MonoBehaviour
{
    //public GameObject deserializeHolder; //GameObject holding the Deserialize script
    private Deserialize deserializer;
    private GameObject scrubber;
    private GameObject Pointer;
    private float ratio;
    private float startOfBar;
    private int frameNum = 0; //initialise to the start
    // Start is called before the first frame update
    void Start()
    {
        //d = deserializeHolder.GetComponent<Deserialize>();
        deserializer = FindFirstObjectByType<Deserialize>();
        //this.gameObject is the ScrubBar parent object as this script will be placed on that object.
        Pointer = this.gameObject.transform.GetChild(1).gameObject; //this starts off at position 0 in the prefab
        startOfBar = Pointer.transform.position.z;
        ratio = 1.0f / (deserializer.getMaxSize() - 1);
        updateScrubBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(deserializer.frameDiff(frameNum)))
        {
            updateScrubBar();
        }
    }

    private void updateScrubBar()
    {
        Vector3 oldPos = Pointer.transform.position;
        frameNum = deserializer.FrameNum;
        Pointer.transform.position = new Vector3(oldPos.x, oldPos.y, startOfBar + (ratio * frameNum));
    }
}
