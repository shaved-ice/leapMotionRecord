using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class saveScript : MonoBehaviour
{
    public TMP_Text[] fileHolder = new TMP_Text[3]; //unfortunately this allows the user to change the size of the array. I only account for the first 3 and this code will likely break if there are less than 2
    public TMP_Text status; //alerts the user that something has happened
    private int currentFile = 1; //starting with the file in the middle for selection purposes
    private string saveFileName = "";
    private Deserialize deserializer;
    private bool[] storedOrNot = new bool[3];
    private Deserialize.FrameJson emptyFile = new Deserialize.FrameJson();
    private Deserialize.FrameJson[] storage = new Deserialize.FrameJson[3]; //this is my display array - they will be displayed in unity in clockwise order from the middle - starting from whoever's index is the currentFile.

    void Start()
    {
        deserializer = FindFirstObjectByType<Deserialize>(); //initialise deserialize script connection
        for (int i = 0; i < 3; i++) //initialise the storedOrNot array
        {
            storedOrNot[i] = false;
        }
    }

    public void updateSaveFileName(string newFileName)
    {
        saveFileName = newFileName;
    }

    public void saveAFile()
    {
        storage[currentFile] = deserializer.getFile(); //save the file into our storage at the current index
        if (saveFileName == "") //change the text of the fileHolder to reflect the name of our newly saved file
        {
            fileHolder[1].text = currentFile.ToString(); //if there's no file name given, just save the number as the name
        }
        else
        {
            fileHolder[1].text = saveFileName;
        }
        storedOrNot[currentFile] = true;
        status.text = "file saved!";
    }

    public void loadAFile()
    {
        if (storedOrNot[currentFile])
        {
            deserializer.setFile(storage[currentFile]); //call the deserializer load file function to load the file
            status.text = "file loaded!";
        }
        else
        {
            status.text = "file is empty!";
        }
    }

    public void deleteASaveFile()
    {
        storage[currentFile] = emptyFile; //change it to take up less storage space
        storedOrNot[currentFile] = false;
        fileHolder[1].text = "Empty";
        status.text = "file deleted!";
    }

    public void prevFile()
    {
        string[] textHolder = new string[3];
        for (int j = 0; j < 3; j++)  //save all the text so we don't lose any
        {
            textHolder[j] = fileHolder[j].text;
        }
        currentFile = (currentFile - 1); //decrement which file we're looking at
        if (currentFile == -1)
        {
            currentFile = 2;
        }
        //this is the new middle file
        for (int i = 0; i < 3; i++) //now just replace all the text with the text of the save file previous
        //aka move all the text one over to the right
        {
            int oldSpace;
            if (i == 0)
            {
                oldSpace = 2;
            }
            else
            {
                oldSpace = i - 1;
            }
            fileHolder[i].text = textHolder[oldSpace];
        }
    }

    public void nextFile()
    {
        string[] textHolder = new string[3];
        for (int j = 0; j < 3; j++)  //save all the text so we don't lose any
        {
            textHolder[j] = fileHolder[j].text;
        }
        currentFile = (currentFile + 1) % 3; //increment which file we're looking at
        for (int i = 0; i < 3; i++) //now just replace all the text with the text of the save file after it
        //aka move all the text one over to the left
        {
            int oldSpace;
            if (i == 2)
            {
                oldSpace = 0;
            }
            else
            {
                oldSpace = i + 1;
            }
            fileHolder[i].text = textHolder[oldSpace];
        }
    }
}
