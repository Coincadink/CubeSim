using HollowCube;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    private int size;

    public GameObject cubie;
    private Vector3 scale;

    private HollowCube<GameObject> objCube;
    private PuzzleCube dataCube;

    public int instantiationLimit = 2048;
    private readonly Queue<SpawnCubeletJob> spawnList = new();

    public float turnSpeed = 1f;
    private readonly LinkedList<TurnJob> turnList = new();

    private void Update()
    {
        // Manage cube x, y, z rotations.
        void HandleKeyInput(KeyCode key)
        {
            if (Input.GetKeyDown(key))
            {
                switch (key)
                {
                    case KeyCode.RightArrow:
                    case KeyCode.LeftArrow:
                        for (int i = 0; i < size; i++)
                            Turn(new Slice { Axis = 1, Dir = key == KeyCode.LeftArrow, Depth = i });
                        break;
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                        for (int i = 0; i < size; i++)
                            Turn(new Slice { Axis = 2, Dir = key == KeyCode.UpArrow, Depth = i });
                        break;
                }
            }
        }

        HandleKeyInput(KeyCode.RightArrow);
        HandleKeyInput(KeyCode.LeftArrow);
        HandleKeyInput(KeyCode.UpArrow);
        HandleKeyInput(KeyCode.DownArrow);

        // Instantiate cubelets
        int spawned = 0;
        while (spawnList.Count > 0 && spawned++ < instantiationLimit)
        {
            SpawnCubeletJob spawnJob;
            lock (spawnList) { spawnJob = spawnList.Dequeue(); }

            // Create cubelet.
            GameObject cubelet = Instantiate(cubie);

            // Name and assign parent to cubelet.
            cubelet.transform.parent = transform;
            cubelet.name = spawnJob.Name;

            // Scale and place cubelet.
            cubelet.transform.localScale = scale;
            cubelet.transform.position = spawnJob.Position;

            // Store cubelet to cube.
            objCube[spawnJob.i, spawnJob.j, spawnJob.k] = cubelet;
        }

        // Rotate cubelets
        if (turnList.Count > 0)
        {
            var turnNode = turnList.First;
            var turnVal = turnNode.Value;
            Vector3 axis = turnVal.Axis;
            HashSet<TurnJob> simultaneous = new();
            while (axis.Equals(turnVal.Axis))
            {
                if (simultaneous.Contains(turnVal)) break;
                simultaneous.Add(turnVal);
                turnNode = turnNode.Next;
                if (turnNode is null) break;
                turnVal = turnNode.Value;
            }

            foreach (TurnJob turn in simultaneous)
            {
                float dist = turn.Degrees * turnSpeed * Time.deltaTime;
                if (Math.Abs(dist + turn.Traveled) > Math.Abs(turn.Degrees))
                    dist = turn.Degrees - turn.Traveled;

                foreach (var obj in turn)
                    obj.transform.RotateAround(Vector3.zero, turn.Axis, dist);

                turn.Travel(dist);
                if (turn.Traveled == turn.Degrees)
                    lock (turnList) { turnList.RemoveFirst(); }
            }
        }
    }

    /// <summary>
    /// Creates a new cube of the given <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    public void Spawn(int size)
    {
        Thread spawnThread = new(GenerateCubeletJobs);
        spawnThread.Start(size);

        this.size = size;
    }

    /// <summary>
    /// Initializes cubelet parameters and adds them to the spawn queue.
    /// </summary>
    /// <param name="sizeObj"></param>
    private void GenerateCubeletJobs(object sizeObj)
    {
        int size = (int)sizeObj;
        scale = new Vector3(2 / (float)size, 2 / (float)size, 2 / (float)size);

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
                        // Calculate cubelet location.
                        float x = (4 * (float)iAdj + 2) / size - 2;
                        float y = (4 * (float)j + 2) / size - 2;
                        float z = (4 * (float)k + 2) / size - 2;
                        Vector3 pos = new(x, y, z);

                        SpawnCubeletJob cubelet = new($"Cubelet {i}, {j}, {k}", pos, i, j, k);
                        lock (spawnList) { spawnList.Enqueue(cubelet); }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents a cubelet waiting to be instantiated.
    /// </summary>
    private class SpawnCubeletJob
    {
        public string Name { get; private set; }
        public Vector3 Position { get; private set; }
        public int i, j, k;

        public SpawnCubeletJob(string name, Vector3 position, int i, int j, int k)
        {
            Name = name;
            Position = position;
            this.i = i; this.j = j; this.k = k;
        }
    }

    /// <summary>
    /// Represents a cubelet slice that needs to be turned.
    /// </summary>
    private class TurnJob : IEnumerable<GameObject>
    {
        private readonly GameObject[] cubelets;
        public Vector3 Axis { get; private set; }
        public float Degrees { get; private set; }
        public float Traveled { get; private set; }

        public TurnJob(GameObject[] cubelets, Vector3 axis, float degrees)
        {
            this.cubelets = new GameObject[cubelets.Length];
            cubelets.CopyTo(this.cubelets, 0);
            Axis = axis;
            Degrees = degrees;
            Traveled = 0f;
        }

        public void Travel(float amount)
        {
            Traveled += amount;
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return ((IEnumerable<GameObject>)cubelets).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cubelets.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj is not TurnJob other) return false;
            return this.Axis == other.Axis && this.Degrees == other.Degrees
                && this.cubelets[0] == other.cubelets[0];
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(cubelets, Axis, Degrees, Traveled);
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
        float degrees = turn.Dir ? 90f : -90f;
        if (turn.Axis == 1) degrees *= -1; // reverse direction of z axis turns because reasons idk spatial problems give me migraines

        TurnJob turnJob = new(objCube.GetSlice(turn), axis, degrees);
        lock (turnList) { turnList.AddLast(turnJob); }
    }

    public void Solve()
    {
        IEnumerable<Slice> solve = dataCube.Solve();

        foreach (Slice s in solve)
        {
            Turn(s);
        }
    }
}
