using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Rubix cube, as a collection of rotated cubelets.
/// Contains six faces, labeled 1-6. Each cubelet is named by its face, then its position on the face, ordered left to right, top to bottom.
/// For example, the cubelet at [1, 12] on a 5x5 cube is directly to the left of the center cubelet on the 1st face.
/// </summary>
public class Cube {
    private byte[] cubelets;

    /// <summary>
    /// Creates a new cube.
    /// </summary>
    /// <param name="size"></param>
    public Cube(int size)
    {
        int arrSize = size * size * size;
        arrSize -= Math.Max(0, (int)Math.Pow(size - 2, 3)); // Remove internal cubes
        cubelets = new byte[arrSize];
    }

    /// <summary>
    /// Returns the cubelet at the named postion.
    /// </summary>
    /// <param name="face">The face containing the cubelet.</param>
    /// <param name="num">The location of the cubelet on the named <paramref name="face"/>.</param>
    /// <returns></returns>
    public byte GetCubelet(int face, int num)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a list of all cubelets in a slice, defined as a square plane of cubelets at a relative <paramref name="depth"/> to a parallel <paramref name="face"/>.
    /// </summary>
    /// <param name="face"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public byte[] GetSlice(int face, int depth = 0)
    {
        return GetSlice(new Slice { Face = face, Depth = depth });
    }

    /// <summary>
    /// Returns a list of all cubelets in a slice.
    /// </summary>
    /// <returns></returns>
    public byte[] GetSlice(Slice slice)
    {
        throw new NotImplementedException();
    }

    public byte this[int face, int num] => GetCubelet(face, num);
    public byte[] this[int face] => GetSlice(face);

    /// <summary>
    /// Rotate one of the cube's slices, where a positive <paramref name="face"/> indicates a clockwise turn, while a negative indicates a counter-clockwise turn.
    /// A slice is defined as a square plane of cubelets, named relative to its nearest parallel <paramref name="face"/>. For example, on a 4x4 cube, the slice directly beneath <paramref name="face"/> 0 is <paramref name="depth" /> 1.
    /// </summary>
    /// <param name="face">The nearest parallel face of the slice.</param>
    /// <param name="depth">Distance of the slice from the named <paramref name="face"/>.</param>
    public void Rotate(int face, int depth = 0)
    {
        Rotate(new Slice { Face = face, Depth = depth });
    }

    /// <summary>
    /// Rotate one of the cube's slices, where a positive face indicates a clockwise turn, while a negative indicates a counter-clockwise turn.
    /// </summary>
    /// <param name="turn">Repr</param>
    public void Rotate(Slice turn)
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