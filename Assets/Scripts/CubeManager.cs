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
            int iAdj = size - i - 1; // swap direction of X (convert from model to unity)
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
                        float x = (iAdj - (float)(size - 1) / 2) * (4 / (float)size);
                        float y = (j - (float)(size - 1) / 2) * (4 / (float)size);
                        float z = (k - (float)(size - 1) / 2) * (4 / (float)size);
                        cubelet.transform.position = new Vector3(x, y, z);

                        // Store cubelet to cube.
                        objCube[i, j, k] = cubelet;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Scramble the cube.
    /// </summary>
    public void Scramble()
    {
        IEnumerable<Slice> scramble = dataCube.Scramble();

        foreach (Slice s in scramble)
        {
            Turn(s);
        }
    }

    /// <summary>
    /// Turn a slice of the cube.
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="cube"></param>
    public void Turn(Slice turn)
    {
        //dataCube.Turn(turn);
        objCube.Turn(turn);

        var axis = turn.Axis switch
        {
            0 => Vector3.up, //y
            1 => Vector3.forward, //z
            2 => Vector3.left, //x
            _ => throw new ArgumentException("Invalid axis"),
        };

        // TODO: make it look purty
        foreach (GameObject obj in objCube.GetSlice(turn))
        {
            float degrees = turn.Dir ? 90f : -90f;
            if (turn.Axis == 1) degrees *= -1; // reverse direction of z axis turns because reasons idk spatial problems give me migraines
            obj.transform.RotateAround(Vector3.zero, axis, degrees);
        }
    }
}
