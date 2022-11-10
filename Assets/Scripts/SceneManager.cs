using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Serves as the scene controller.
/// Handles button input.
/// </summary>
public class SceneManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject inputField;

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
    }

    /// <summary>
    /// Gets user input and turns the cube model.
    /// </summary>
    private void TurnCube()
    {
        // TODO: collect user input

        // TODO: construct turn using user input
        Slice turn = new();
        cubeManager.Turn(turn);
    }

    /// <summary>
    /// Destroys the existing cube.
    /// </summary>
    public void Clear()
    {
        Destroy(cubeObject);
        cubeSpawned = false;
    }
    
    /// <summary>
    /// Generates and applies a scramble to the cube.
    /// </summary>
    public void Scramble()
    {
        if (!cubeSpawned)
            return;

        cubeManager.Scramble();
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
