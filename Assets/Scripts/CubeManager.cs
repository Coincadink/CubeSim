using HollowCube;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameObject cubie;

    private HollowCube<GameObject> objCube;
    private RubiksCube dataCube;

    /// <summary>
    /// Creates a new cube of the given <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    public void Spawn(int size)
    {
        objCube = new(size);
        dataCube = new(size);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    if (i == 0 || i == size - 1 || j == 0 || j == size - 1 || k == 0 || k == size - 1) // If on cube face
                    {
                        // Create cubelet.
                        GameObject cubelet = Instantiate(cubie);

                        // Name and assign parent to cubelet.
                        cubelet.transform.parent = transform;
                        cubelet.name = "Cubelet " + i + ", " + j + ", " + k;

                        // Scale cubelet.
                        cubelet.transform.localScale = new Vector3(2 / (float)size, 2 / (float)size, 2 / (float)size);

                        // Place cubelet.
                        float x = (i - (float)(size - 1) / 2) * (4 / (float)size);
                        float y = (j - (float)(size - 1) / 2) * (4 / (float)size);
                        float z = (k - (float)(size - 1) / 2) * (4 / (float)size);
                        cubelet.transform.position = new Vector3(x, y, z);

                        // Store cubelet to cubes.
                        objCube[i, j, k] = cubelet;
                        dataCube[i, j, k] = CubeOrientation.Up;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Yeah. Deal with a lack of definition.
    /// </summary>
    public void Scramble()
    {
        IEnumerable<Slice> scramble = dataCube.Scramble();
        Debug.Log(dataCube.ToString());
        // Game Object updating dear god good l uck you bold ass motherfuckers
    }

    /// <summary>
    /// Turn a slice of the cube.
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="cube"></param>
    public void Turn(Slice turn)
    {
        dataCube.Turn(turn);

        // TODO: make it look purty
        objCube.Turn(turn);
    }
}
