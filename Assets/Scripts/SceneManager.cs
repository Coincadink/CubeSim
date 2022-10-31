using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using TMPro;

public class SceneManager : MonoBehaviour
{
    public GameObject cubie;
    public GameObject inputField;
    
    private int cubeDim = 3;
    private GameObject[] cube;
    private bool cubeSpawned = false;

    public void spawnCube()
    {
        if (cubeSpawned)
            Clear(); 

        string text = inputField.GetComponent<TMP_InputField>().text;
        cubeDim = Int32.Parse(text);

        cube = new GameObject[cubeDim * cubeDim * cubeDim];

        int index = 0;
        for (int i = 0; i < cubeDim; i++)
        {
            for (int j = 0; j < cubeDim; j++)
            {
                for (int k = 0; k < cubeDim; k++)
                {
                    if (i == 0 || i == cubeDim - 1 || j == 0 || j == cubeDim - 1 || k == 0 || k == cubeDim - 1)
                    {
                        GameObject cubeletGO = Instantiate(cubie);
                        
                        cubeletGO.transform.localScale = new Vector3(2 / (float) cubeDim, 2 / (float) cubeDim, 2 / (float) cubeDim);

                        float x = ((float) i - ((float) cubeDim - 1) / 2)  * (4 / (float) cubeDim);
                        float y = ((float) j - ((float) cubeDim - 1) / 2)  * (4 / (float) cubeDim);
                        float z = ((float) k - ((float) cubeDim - 1) / 2)  * (4 / (float) cubeDim);

                        cubeletGO.transform.position = new Vector3(x, y, z);

                        cube[index] = cubeletGO;

                        index++;
                    }
                }
            }
        }

        cubeSpawned = true;
    }

    public void Quit() 
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void Clear()
    {
        foreach (GameObject cubelet in cube)
        {
            Destroy(cubelet);
        }

        cubeSpawned = false;
    }
}
