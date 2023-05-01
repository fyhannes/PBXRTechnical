/**
 * Little script that just showcases the portability of ApplyTransforms.cs
 * to different meshes. Press Left or Right arrow key to cycle through gallery.
 * @author Johannes Fung
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCollection : MonoBehaviour
{
    public GameObject[] objectsToDisplay;
    private int numObjectsToDisplay;
    private int toggled = 0;
    // Start is called before the first frame update
    void Start()
    {
        numObjectsToDisplay = objectsToDisplay.Length;
    }

    void Cycle(bool toggle)
    {
        objectsToDisplay[toggled].SetActive(toggle);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Cycle(false);
            toggled += 1;
            toggled = toggled % numObjectsToDisplay;
            Cycle(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Cycle(false);
            toggled -= 1;
            // Cycle back to end of list.
            if (toggled == -1)
            {
                toggled = numObjectsToDisplay - 1;
            }
            Cycle(true);
        }
    }
}
