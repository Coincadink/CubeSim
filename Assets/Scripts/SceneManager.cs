using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject cubelet;
    public int cubeDim = 3;

    public void spawnCube()
    {
        for (int i = 0; i < cubeDim; i++)
        {
            for (int j = 0; j < cubeDim; j++)
            {
                for (int k = 0; k < cubeDim; k++)
                {
                    float x = (i - Mathf.Floor(cubeDim / 2)) / cubeDim * 2;
                    float y = (j - Mathf.Floor(cubeDim / 2)) / cubeDim * 2;
                    float z = (k - Mathf.Floor(cubeDim / 2)) / cubeDim * 2;

                    GameObject cubeletGO = Instantiate(cubelet);
                    cubeletGO.transform.localScale -= new Vector3(0.666f, 0.666f, 0.666f);
                    cubeletGO.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    public void Quit() 
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
