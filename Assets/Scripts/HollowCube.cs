using System;
using System.Collections;
using System.Collections.Generic;

namespace HollowCube
{
    /// <summary>
    /// Represents a hollow 3D cube as a 3D array.
    /// Only allocates memory for items on the surface of the cube.
    /// </summary>
    public class HollowCube<T> : IEnumerable<T>
    {
        private T[] backer;
        protected readonly int size;

        /// <summary>
        /// Create a new HollowCube of side length <paramref name="size"/>.
        /// </summary>
        /// <param name="size"></param>
        public HollowCube(int size)
        {
            this.size = size;

            int arrSize = size * size * size;
            arrSize -= Math.Max(0, (int)Math.Pow(size - 2, 3)); // Remove internal spaces
            backer = new T[arrSize];
        }

        /// <summary>
        /// Return a shallow clone of this HollowCube.
        /// </summary>
        public HollowCube<T> Clone()
        {
            return new HollowCube<T>(size)
            {
                backer = (T[])backer.Clone()
            };
        }

        /// <summary>
        /// Returns a slice of the array perpendicular to a coordinate direction, defined using an axis <paramref name="dir"/> and a coordinate <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// 
        /// If the specified slice contains empty space in the center, the face is returned in clockwise order, starting at 0, 0 on the two axes.
        /// 
        /// If the specified slice is a full face, the face is returned as a series of rows in order X, Z, Y (excluding the specified <paramref name="dir"/>).
        /// To access a face as a 2D array, consider <seealso cref="Get2DSlice(int, int)"/>.
        /// </summary>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="dir">The coordinate direction the slice lies on.</param>
        /// <returns>A 1D array containing the slice.</returns>
        public T[] GetSlice(int depth, int dir = 0)
        {
            switch (dir)
            {
                case 0: // Y
                    // Return the face as a 1D array, ordered left to right, top to bottom
                    if (depth == 0 || depth == size - 1)
                    {
                        return backer[CoordsToIndex(0, depth, 0)..CoordsToIndex(size - 1, depth, size - 1)];
                    }

                    //corners: 0, size-1, (size-1)*2, (size-1)*3

                    // Return the slice as a 1D array, ordered in clockwise order, starting at (x, z) = (0, 0).
                    int perimSize = (size - 1) * 4;
                    T[] slice = new T[perimSize];
                    var indices = GetClockwiseIndices();
                    for (int i = 0; i < perimSize; i++)
                    {
                        var (x, z) = indices[i];
                        slice[i] = this[x, depth, z];
                    }
                    return slice;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sets a slice of the array perpendicular to a coordinate direction, defined using an axis <paramref name="dir"/> and a coordinate <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// 
        /// If the specified slice contains empty space in the center, the face is set in clockwise order, starting at 0, 0 on the two axes.
        /// 
        /// If the specified slice is a full face, the face is set as a series of rows in order X, Z, Y (excluding the specified <paramref name="dir"/>).
        /// To set a face using a 2D array, consider <seealso cref="Set2DSlice(T[,], int, int)"/>.
        /// </summary>
        /// <param name="slice">The array containing data to set the slice to.</param>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="dir">The coordinate direction the slice lies on.</param>
        public void SetSlice(T[] slice, int depth, int dir = 0)
        {
            switch (dir)
            {
                case 0:
                    if (depth == 0 || depth == size - 1)
                    {
                        for (int i = 0; i < size; i++)
                            for (int j = 0; j < size; j++)
                                this[i, depth, j] = slice[i * size + j];
                    }
                    else
                    {
                        var indices = GetClockwiseIndices();
                        for (int i = 0; i < (size - 1) * 4; i++)
                        {
                            var (x, z) = indices[i];
                            this[x, depth, z] = slice[i];
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns a slice of the cube perpendicular to a coordinate direction, defined using an axis <paramref name="dir"/> and a coordinate <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// </summary>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="dir">The coordinate direction the slice lies on.</param>
        /// <returns>A 2D array, ordered X, Y, Z (excluding the specified axis <paramref name="dir"/>).</returns>
        public T[,] Get2DSlice(int depth, int dir = 0)
        {
            T[] slice = GetSlice(depth, dir);
            T[,] slice2D;
            if (depth == 0 || depth == size - 1) // Square face
            {
                slice2D = new T[size, size];

                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        slice2D[i, j] = slice[i * size + j];
            }
            else
            {
                throw new NotImplementedException();
            }

            return slice2D;
        }

        /// <summary>
        /// Sets a slice of the array perpendicular to a coordinate direction, defined using an axis <paramref name="dir"/> and a coordinate <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// 
        /// The face is set in order X, Z, Y (excluding the specified <paramref name="dir"/>).
        /// </summary>
        /// <param name="slice">The array containing data to set the slice to.</param>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="dir">The coordinate direction the slice lies on.</param>
        public void Set2DSlice(T[,] slice, int depth, int dir = 0)
        {
            T[] slice1D;
            if (depth == 0 || depth == size - 1) // Square face
            {
                slice1D = new T[slice.Length];
                for (int i = 0; i < slice.Length; i++)
                    slice1D[i] = slice[i / size, i % size];
            }
            else
            {
                throw new NotImplementedException();
            }

            SetSlice(slice1D, depth, dir);
        }

        /// <summary>
        /// Returns/sets a slice of the array as either a 1D or 2D array, dependent on whether the slice is hollow in the center or full, respectively.
        /// See method descriptions of <seealso cref="GetSlice(int, int)"/>, <seealso cref="Get2DSlice(int, int)"/>, <seealso cref="SetSlice(T[], int, int)"/>, and <seealso cref="Set2DSlice(T[,], int, int)"/>.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="dir"></param>
        public Array this[int depth, int dir = 0]
        {
            get
            {
                if (depth == 0 || depth == size - 1) return Get2DSlice(depth, dir);
                else return GetSlice(depth, dir);
            }
            set
            {
                if (value is T[,] v) Set2DSlice(v, depth, dir);
                else SetSlice(value as T[], depth, dir);
            }
        }

        public T this[int x, int y, int z]
        {
            get
            {
                return backer[CoordsToIndex(x, y, z)];
            }
            set
            {
                backer[CoordsToIndex(x, y, z)] = value;
            }
        }

        /// <summary>
        /// Converts 3D coordinates to a 1D coordinate. Removes coordinate values associated with empty space.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private int CoordsToIndex(int x, int y, int z)
        {
            // If not on cube surface, throw exception
            if ((x != 0 && x != size - 1) && (y != 0 && y != size - 1) && (z != 0 && z != size - 1))
                throw new IndexOutOfRangeException();

            // Ignorant index value
            int index = (y * size * size) + (z * size) + (x);

            // Adjust ignorant index for each value expected in a 1D array that isn't present.
            for (int iy = 1; iy < size - 1; iy++)
            {
                if (iy > y) break;
                for (int iz = 1; iz < size - 1; iz++)
                {
                    if (iz > z && iy == y) break;
                    for (int ix = 1; ix < size - 1; ix++)
                    {
                        if (ix > x && iz == z && iy == y) break;
                        index--;
                    }
                }
            }

            return index;
        }

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
            Array slice = this[turn.Depth, turn.Axis];

            // 1D slice in clockwise order
            if (slice is byte[] sl1) 
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

                this[turn.Depth, turn.Axis] = turnedSlice;
            }

            // 2D slice in row by row order
            else if (slice is byte[,] sl2) 
            {
                byte[,] turnedSlice;
                if (turn.Dir) // clockwise
                {
                    turnedSlice = Transpose(sl2);
                    turnedSlice = ReverseRows(turnedSlice);
                }
                else // counterclockwise
                {
                    turnedSlice = ReverseRows(sl2);
                    turnedSlice = Transpose(turnedSlice);
                }

                this[turn.Depth, turn.Axis] = turnedSlice;
            }
        }

        /// <summary>
        /// Transposes a 2D matrix.
        /// </summary>
        /// <param name="mat">The matrix to transpose.</param>
        /// <returns>The transposed matrix.</returns>
        private static byte[,] Transpose(byte[,] mat)
        {
            byte[,] trans = new byte[mat.GetLength(1), mat.GetLength(0)];
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    trans[j, i] = mat[i, j];
                }
            }
            return trans;
        }

        /// <summary>
        /// Reverses the order of items in each row of a 2D matrix.
        /// </summary>
        /// <param name="mat">The matrix to reverse the rows of.</param>
        /// <returns>The row reversed matrix.</returns>
        private static byte[,] ReverseRows(byte[,] mat)
        {
            int colLen = mat.GetLength(0), rowLen = mat.GetLength(1);
            byte[,] rev = new byte[colLen, rowLen];
            for (int i = 0; i < colLen; i++)
            {
                int s = 0, e = rowLen - 1;
                while (s < e)
                {
                    rev[i, s] = mat[i, e];
                    rev[i, e--] = mat[i, s++];
                }

            }
            return rev;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)backer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return backer.GetEnumerator();
        }

        /// <summary>
        /// Returns a list of tuples with the corresponding coordinate pairs for each index of a clockwise slice.
        /// </summary>
        private IList<(int, int)> GetClockwiseIndices()
        {
            List<(int, int)> indices = new();

            for (int i = 0; i < (size - 1) * 4; i++)
            {
                if (i < size) // top side
                    indices.Add((0, i));
                else if (i < (size - 1) * 2) // right side, no corners
                    indices.Add((i - (size - 1), size - 1));
                else if (i <= (size - 1) * 3) // bottom side
                    indices.Add((size - 1, (size * 2) - i));
                else // left side, no corners
                    indices.Add(((size * 3) - i, 0));
            }

            return indices;
        }
    }
}