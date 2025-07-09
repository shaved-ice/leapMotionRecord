using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Deserialize : MonoBehaviour
{

    public string directoryPath;
    public string fileName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPress()
    {
        byte[] arr = new byte[32];
        UTF8Encoding ut = new UTF8Encoding();
        int r = 1;
        int x = 0;
        string filePath = directoryPath + "\\" + fileName + ".txt";
        if (!(File.Exists(filePath)))
        {
            Debug.Log("File doesn't exist!");
        }
        else
        {
            using (Stream fs = File.OpenRead(filePath))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    Debug.Log(sr.ReadLine());
                }
            }
        }


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
}
