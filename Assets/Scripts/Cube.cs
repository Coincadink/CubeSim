using System;
using System.Collections.Generic;
using HollowArray;

/// <summary>
/// Represents a Rubix cube as a collection of rotated cubelets.
/// Contains six faces, labeled 1-6. Faces are labeled in order Y=0, Y=size, Z=0, Z=size, X=0, X=size.
/// Each cubelet can also be named by its position in 3D space, where [0,0,0] is the bottom right front corner.
/// </summary>
public class Cube {
    private HollowArray<byte> cubelets;
    private readonly int size;
    readonly LinkedList<Slice> moveHistory;

    /// <summary>
    /// Creates a new cube.
    /// </summary>
    /// <param name="size">Side length of the new cube.</param>
    public Cube(int size)
    {
        this.size = size;
        cubelets = new(size);
        moveHistory = new();
    }

    /// <summary>
    /// Returns a deep clone of this Cube.
    /// </summary>
    /// <returns></returns>
    public Cube Clone()
    {
        return new(size)
        {
            cubelets = cubelets.Clone()
        };
    }

    /// <summary>
    /// Returns the cubelet at the named coordinate postion.
    /// </summary>
    public byte GetCubelet(int x, int y, int z)
    {
        return cubelets[x, y, z];
    }

    /// <summary>
    /// Returns the cubelet at the named coordinate position.
    /// </summary>
    public byte this[int x, int y, int z] => GetCubelet(x, y, z);

    /// <summary>
    /// Rotate one of the cube's slices, where <paramref name="dir"/> = true indicateds a clockwise turn, while false indicates a counter-clockwise turn.
    /// A slice is defined as a square plane of cubelets, named relative to its nearest parallel <paramref name="face"/>. For example, on a 4x4 cube, the slice directly beneath <paramref name="face"/> 0 is <paramref name="depth" /> 1.
    /// </summary>
    /// <param name="axis">The axis perpendicular to the slice. 0=Y, 1=Z, 2=X.</param>
    /// <param name="dir">The direction to turn the slice in.</param>
    /// <param name="depth">Coordinate position of the slice along the named <paramref name="axis"/>.</param>
    public void Turn(int axis, bool dir, int depth = 0)
    {
        Turn(new Slice { Axis = axis, Dir = dir, Depth = depth });
    }

    /// <summary>
    /// Rotate one of the cube's slices, where <see cref="Slice.Dir"/> = true indicateds a clockwise turn, while false indicates a counter-clockwise turn.
    /// </summary>
    /// <param name="turn">The slice to turn.</param>
    public void Turn(Slice turn)
    {
        moveHistory.AddLast(turn);

        Array slice = cubelets[turn.Depth, turn.Axis];
        if (slice is byte[] sl1) // 1D slice in clockwise order
        {
            byte[] turnedSlice = new byte[sl1.Length];
            if (turn.Dir) // clockwise
            {
                sl1[0..(sl1.Length - (size - 1))].CopyTo(turnedSlice, size - 1);
                sl1[(sl1.Length - (size - 1))..sl1.Length].CopyTo(turnedSlice, 0);
            }
            else // counterclockwise
            {
                sl1[(size - 1)..sl1.Length].CopyTo(turnedSlice, 0);
                sl1[0..(size - 1)].CopyTo(turnedSlice, sl1.Length - (size - 1));                
            }

            cubelets[turn.Depth, turn.Axis] = turnedSlice;
        }
        else if (slice is byte[,] sl2) // 2D slice in row by row order
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Scrambles the cube.
    /// </summary>
    /// <param name="turns">Number of turns to make to scramble the cube.</param>
    /// <returns>The turns made to scramble the cube, in order.</returns>
    public IEnumerable<Slice> Scramble(int turns = 100)
    {
        throw new NotImplementedException();
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

/// <summary>
/// Represents a square plane of cubelets, named relative to its perpendicular axis.
/// For example, on a 4x4 cube, face 4 is <see cref="Axis"/> 1, <see cref="Depth"/> 3.
/// </summary>
public struct Slice
{
    public int Axis;
    public int Depth;
    public bool Dir;
}