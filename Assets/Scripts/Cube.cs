using System;
using System.Collections.Generic;
using HollowArray;

/// <summary>
/// Represents a Rubix cube, as a collection of rotated cubelets.
/// Contains six faces, labeled 1-6. Each cubelet is named by its face, then its position on the face, ordered left to right, top to bottom.
/// For example, the cubelet at [1, 12] on a 5x5 cube is directly to the left of the center cubelet on the 1st face.
/// </summary>
public class Cube {
    private HollowArray<byte> cubelets;
    private readonly int size;

    /// <summary>
    /// Creates a new cube.
    /// </summary>
    /// <param name="size">Side length of the new cube.</param>
    public Cube(int size)
    {
        this.size = size;
        cubelets = new(size);
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
    /// Rotate one of the cube's slices, where a positive <paramref name="face"/> indicates a clockwise turn, while a negative indicates a counter-clockwise turn.
    /// A slice is defined as a square plane of cubelets, named relative to its nearest parallel <paramref name="face"/>. For example, on a 4x4 cube, the slice directly beneath <paramref name="face"/> 0 is <paramref name="depth" /> 1.
    /// </summary>
    /// <param name="face">The nearest parallel face of the slice.</param>
    /// <param name="depth">Distance of the slice from the named <paramref name="face"/>.</param>
    public void Turn(int face, int depth = 0)
    {
        Turn(new Slice { Face = face, Depth = depth });
    }

    /// <summary>
    /// Rotate one of the cube's slices, where a positive face indicates a clockwise turn, while a negative indicates a counter-clockwise turn.
    /// </summary>
    /// <param name="turn">The slice to turn.</param>
    public void Turn(Slice turn)
    {
        throw new NotImplementedException(); 
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
/// Represents a square plane of cubelets, named relative to its nearest parallel face.
/// For example, on a 4x4 cube, the slice directly beneath <see cref="Face"/> 3 is <see cref="Depth"/> 1.
/// </summary>
public struct Slice
{
    public int Face;
    public int Depth;
}