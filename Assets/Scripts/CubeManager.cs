using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameObject cubie;

    private GameObject[] cubelets;

    /// <summary>
    /// Creates a new cube of the given <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    public void Spawn(int size)
    {
        int arrSize = size * size * size;
        arrSize -= Math.Max(0, (int)Math.Pow(size - 2, 3)); // Remove internal cubes
        cubelets = new GameObject[arrSize];

        int index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    if (i == 0 || i == size - 1 || j == 0 || j == size - 1 || k == 0 || k == size - 1) // If on cube face
                    {
                        GameObject cubeletGO = Instantiate(cubie);
                        cubeletGO.transform.parent = transform;
                        cubeletGO.name = "Cubelet " + index;

                        cubeletGO.transform.localScale = new Vector3(2 / (float)size, 2 / (float)size, 2 / (float)size);

                        float x = ((float)i - ((float)size - 1) / 2) * (4 / (float)size);
                        float y = ((float)j - ((float)size - 1) / 2) * (4 / (float)size);
                        float z = ((float)k - ((float)size - 1) / 2) * (4 / (float)size);

                        cubeletGO.transform.position = new Vector3(x, y, z);

                        cubelets[index] = cubeletGO;

                        index++;
                    }
                }
            }
        }
    }
}
