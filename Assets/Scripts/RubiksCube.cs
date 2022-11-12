using HollowCube;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a Rubik's cube as a collection of rotated cubelets.
/// Contains six faces, labeled 0-5. Faces are labeled in order Y=0, Y=size, Z=0, Z=size, X=0, X=size.
/// Each cubelet can also be named by its position in 3D space, where [0,0,0] is the bottom right front corner.
/// </summary>
public class RubiksCube : HollowCube<Cubelet>
{
    // Just storing scramble for now since I'm lazy and we can't make any other moves.
    Slice[] scramble;
    LinkedList<Slice> moveHistory = new();

    /// <summary>
    /// Creates a new cube.
    /// </summary>
    /// <param name="size">Side length of the new cube.</param>
    public RubiksCube(int size) : base(size)
    {
        // Initialize faces
        // Y faces
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                AddFace(x, 0, z, 000);
                AddFace(x, size - 1, z, 100);
            }
        }
        // Z faces
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                AddFace(x, y, 0, 001);
                AddFace(x, y, size - 1, 101);
            }
        }
        // X faces
        for (int y = 0; y < size; y++)
        {
            for (int z = 0; z < size; z++)
            {
                AddFace(0, y, z, 010);
                AddFace(size - 1, y, z, 110);
            }
        }
    }

    private void AddFace(int x, int y, int z, byte face)
    {
        if (this[x, y, z] is null)
        {
            this[x, y, z] = new Cubelet(face);
        }
        else
        {
            this[x, y, z].AddFace(face);
        }
    }

    /// <summary>
    /// Returns a deep clone of this RubiksCube.
    /// </summary>
    /// <returns></returns>
    public new RubiksCube Clone()
    {
        RubiksCube clone = (RubiksCube)base.Clone();
        clone.moveHistory = new(moveHistory);
        return clone;
    }

    /// <summary>
    /// Returns the cubelet at the named coordinate postion.
    /// </summary>
    public Cubelet GetCubelet(int x, int y, int z) => this[x, y, z];

    /// <summary>
    /// Rotate one of the cube's slices, where <paramref name="dir"/> = true indicateds a clockwise turn, while false indicates a counter-clockwise turn.
    /// A slice is defined as a square plane of cubelets, named relative to its nearest parallel <paramref name="face"/>. For example, on a 4x4 cube, the slice directly beneath <paramref name="face"/> 0 is <paramref name="depth" /> 1.
    /// </summary>
    /// <param name="axis">The axis perpendicular to the slice. 0=Y, 1=Z, 2=X.</param>
    /// <param name="dir">The direction to turn the slice in.</param>
    /// <param name="depth">Coordinate position of the slice along the named <paramref name="axis"/>.</param>
    public new void Turn(int axis, bool dir, int depth = 0)
    {
        Turn(new Slice { Axis = axis, Dir = dir, Depth = depth });
    }

    /// <summary>
    /// Rotate one of the cube's slices, where <see cref="Slice.Dir"/> = true indicateds a clockwise turn, while false indicates a counter-clockwise turn.
    /// </summary>
    /// <param name="turn">The slice to turn.</param>
    public new void Turn(Slice slice)
    {
        base.Turn(slice);
        moveHistory.AddLast(slice);

        foreach (Cubelet c in GetSlice(slice))
        {
            c.Rotate(slice.Axis, slice.Dir);
        }
    }

    /// <summary>
    /// Scrambles the cube.
    /// </summary>
    /// <param name="turns">Number of turns to make to scramble the cube.</param>
    /// <returns>The turns made to scramble the cube, in order.</returns>
    public IEnumerable<Slice> Scramble(int turns = 100)
    {
        Slice[] movesMade = new Slice[turns];
        Random r = new();

        for (int i = 0; i < turns; i++)
        {
            int axis = r.Next(3);
            int depth = r.Next(size);
            bool dir = r.Next() % 2 == 0;

            Slice turn = new() { Axis = axis, Dir = dir, Depth = depth };
            Turn(turn);
            movesMade[i] = turn;
        }

        scramble = movesMade;
        return movesMade;
    }

    /// <summary>
    /// Solves the cube.
    /// </summary>
    /// <returns>The turns made to solve the cube, in order.</returns>
    public IEnumerable<Slice> Solve()
    {
        throw new NotImplementedException();
    }
}

public class Cubelet
{
    readonly Dictionary<byte, byte> faces = new();

    readonly byte[] rotArr = new byte[6] { 110, 000, 101, 010, 100, 001 }; // z, -y, x, -z, y, -x

    public Cubelet(params byte[] faces)
    {
        foreach (byte f in faces)
        {
            AddFace(f);
        }
    }

    public void AddFace(byte face)
    {
        faces[face] = (byte)(4 * (face % 2) + (face / 2)); // f % 2 determines direction (1 = pos, 0 = neg), f /2 determines face
    }

    public void Rotate(int axis, bool dir)
    {
        foreach (byte face in faces.Keys)
        {
            int faceAxis = face / 4;
            if (faceAxis == axis) continue;

            int fi = Array.FindIndex(rotArr, x => x == face);
            do
            {
                fi += dir ? 1 : -1;

                // loop index
                if (fi >= 6) fi -= 6;
                else if (fi < 0) fi += 6;
            } while (axis != rotArr[fi] / 4);

            faces[face] = rotArr[fi];
        }
    }
}