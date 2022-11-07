using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

/// <summary>
/// Serves as the scene controller.
/// Handles button input.
/// </summary>
public class SceneManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject inputField;

    private Cube cube;
    private GameObject cubeObject;
    private CubeManager cubeManager;

    private int cubeDim = 3;
    private bool cubeSpawned = false;

    /// <summary>
    /// Creates a new cube.
    /// </summary>
    public void SpawnCube()
    {
        if (cubeSpawned)
            Clear();

        cubeObject = Instantiate(cubePrefab);
        cubeObject.name = "Cube";
        cubeManager = cubeObject.GetComponent<CubeManager>();

        string text = inputField.GetComponent<TMP_InputField>().text;
        cubeDim = Int32.Parse(text);

        cubeManager.Spawn(cubeDim);
        cubeSpawned = true;

        cube = new(cubeDim);
    }

    /// <summary>
    /// TODO: A function gets user input, constructs <paramref name="turn"/>, then calls this function.
    /// </summary>
    private void TurnCube(Slice turn)
    {
        cube.Turn(turn);
        cubeManager.Turn(turn, cube.Clone());
    }

    /// <summary>
    /// Destroys the existing cube.
    /// </summary>
    public void Clear()
    {
        Destroy(cubeObject);
        cube = null;
        cubeSpawned = false;
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit() 
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
