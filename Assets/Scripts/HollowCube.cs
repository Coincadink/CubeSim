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
        public readonly int Size;
        private readonly int mSize; // Size - 1, this value is used frequently

        /// <summary>
        /// Create a new HollowCube of side length <paramref name="size"/>.
        /// </summary>
        /// <param name="size"></param>
        public HollowCube(int size)
        {
            Size = size;
            mSize = size - 1;

            int arrSize = Size * Size * Size;
            arrSize -= Math.Max(0, (int)Math.Pow(Size - 2, 3)); // Remove internal spaces
            backer = new T[arrSize];
        }

        public override string ToString()
        {
            string str = GetSlice(1, 0).ToString();
            return str;
        }

        /// <summary>
        /// Return a shallow clone of this HollowCube.
        /// </summary>
        public HollowCube<T> Clone()
        {
            return new HollowCube<T>(Size)
            {
                backer = (T[])backer.Clone()
            };
        }

        /// <summary>
        /// Returns a slice of the array perpendicular to a coordinate direction, defined using an <paramref name="axis"/> and a <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// 
        /// If the specified slice contains empty space in the center, the face is returned in clockwise order, starting at 0, 0 on the two axes.
        /// 
        /// If the specified slice is a full face, the face is returned as a series of rows in order X, Z, Y (excluding the specified <paramref name="axis"/>).
        /// To access a face as a 2D array, consider <seealso cref="Get2DSlice(int, int)"/>.
        /// </summary>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="axis">The coordinate direction the slice lies on.</param>
        /// <returns>A 1D array containing the slice.</returns>
        public T[] GetSlice(int depth, int axis = 0)
        {
            if (IsEdge(depth))
            {
                // Return the face as a 1D array, ordered left to right, top to bottom
                switch (axis)
                {
                    case 0: // Y
                        return backer[CoordsToIndex(0, depth, 0)..(CoordsToIndex(mSize, depth, mSize) + 1)];
                    case 1: // Z
                    case 2: // X
                        T[] slice = new T[Size * Size];
                        for (int y = 0; y < Size; y++)
                            for (int xz = 0; xz < Size; xz++)
                                slice[y * Size + xz] = axis == 1 ? this[xz, y, depth] : this[depth, y, xz];
                        return slice;
                    default:
                        throw new ArgumentException("Invalid axis");
                }
            }
            else
            {
                // Return the slice as a 1D array, ordered in clockwise order, starting at (x|y, y|z) = (0, 0).
                int perimSize = (mSize) * 4;
                T[] slice = new T[perimSize];
                var indices = GetClockwiseIndices();
                for (int i = 0; i < perimSize; i++)
                {
                    var (yz, zx) = indices[i];
                    slice[i] = axis switch
                    {
                        0 => this[zx, depth, yz],
                        1 => this[zx, yz, depth],
                        2 => this[depth, yz, zx],
                        _ => throw new ArgumentException("Invalid axis")
                    };
                }

                return slice;
            }
        }

        /// <summary>
        /// Returns a slice of the array perpendicular to a coordinate direction, defined using an axis and a depth.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// 
        /// If the specified slice contains empty space in the center, the face is returned in clockwise order, starting at 0, 0 on the two axes.
        /// 
        /// If the specified slice is a full face, the face is returned as a series of rows in order X, Z, Y (excluding the specified axis).
        /// To access a face as a 2D array, consider <seealso cref="Get2DSlice(Slice)"/>.
        /// </summary>
        /// <param name="slice">The slice of the cube.</param>
        /// <returns>A 1D array containing the slice.</returns>
        public T[] GetSlice(Slice slice) => GetSlice(slice.Depth, slice.Axis);

        /// <summary>
        /// Sets a slice of the array perpendicular to a coordinate direction, defined using an <paramref name="axis"/> and a <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// 
        /// If the specified slice contains empty space in the center, the face is set in clockwise order, starting at 0, 0 on the two axes.
        /// 
        /// If the specified slice is a full face, the face is set as a series of rows in order X, Z, Y (excluding the specified <paramref name="axis"/>).
        /// To set a face using a 2D array, consider <seealso cref="Set2DSlice(T[,], int, int)"/>.
        /// </summary>
        /// <param name="slice">The array containing data to set the slice to.</param>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="axis">The coordinate direction the slice lies on.</param>
        public void SetSlice(T[] slice, int depth, int axis = 0)
        {
            if (IsEdge(depth))
            {
                for (int yz = 0; yz < Size; yz++)
                    for (int zx = 0; zx < Size; zx++)
                        switch (axis)
                        {
                            case 0:
                                this[zx, depth, yz] = slice[yz * Size + zx]; break;
                            case 1:
                                this[zx, yz, depth] = slice[yz * Size + zx]; break;
                            case 2:
                                this[depth, yz, zx] = slice[yz * Size + zx]; break;
                            default:
                                throw new ArgumentException("Invalid axis");
                        }
            }
            else
            {
                var indices = GetClockwiseIndices();
                for (int i = 0; i < (mSize) * 4; i++)
                {
                    var (yz, zx) = indices[i];
                    switch (axis)
                    {
                        case 0:
                            this[zx, depth, yz] = slice[i]; break;
                        case 1:
                            this[zx, yz, depth] = slice[i]; break;
                        case 2:
                            this[depth, yz, zx] = slice[i]; break;
                        default:
                            throw new ArgumentException("Invalid axis");
                    }
                }
            }
        }

        /// <summary>
        /// Returns a slice of the cube perpendicular to a coordinate direction, defined using an <paramref name="axis"/> and a <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// </summary>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="axis">The coordinate direction the slice lies on.</param>
        /// <returns>A 2D array, ordered X, Y, Z (excluding the specified axis <paramref name="axis"/>).</returns>
        public T[,] Get2DSlice(int depth, int axis = 0)
        {
            T[,] slice2D = new T[Size, Size];
            if (IsEdge(depth)) // Face
            {
                T[] slice = GetSlice(depth, axis);
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        slice2D[i, j] = slice[i * Size + j];
            }
            else // Hollow slice
            {
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (!IsEdge(i) && !IsEdge(j)) continue;
                        slice2D[i, j] = axis switch
                        {
                            0 => this[i, depth, j],
                            1 => this[i, j, depth],
                            2 => this[depth, i, j],
                            _ => throw new ArgumentException("Invalid axis")
                        };
                    }
                }
            }

            return slice2D;
        }

        /// <summary>
        /// Returns a slice of the cube perpendicular to a coordinate direction, defined using an axis and a depth.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = X,
        ///     2 = Z.
        /// </summary>
        /// <param name="slice">The slice of the cube.</param>
        /// <returns>A 2D array, ordered X, Y, Z (excluding the specified axis).</returns>
        public T[,] Get2DSlice(Slice slice) => Get2DSlice(slice.Depth, slice.Axis);

        /// <summary>
        /// Sets a slice of the array perpendicular to a coordinate direction, defined using an <paramref name="axis"/> and a <paramref name="depth"/>.
        /// Axes are defined as:
        ///     0 = Y (default),
        ///     1 = Z,
        ///     2 = X.
        /// 
        /// The face is set in order X, Z, Y (excluding the specified <paramref name="axis"/>).
        /// </summary>
        /// <param name="slice">The array containing data to set the slice to.</param>
        /// <param name="depth">The position of the slice along the given axis.</param>
        /// <param name="axis">The coordinate direction the slice lies on.</param>
        public void Set2DSlice(T[,] slice, int depth, int axis = 0)
        {
            T[] slice1D;
            if (IsEdge(depth)) // Square face
            {
                slice1D = new T[slice.Length];
                for (int i = 0; i < slice.Length; i++)
                    slice1D[i] = slice[i / Size, i % Size];
            }
            else
            {
                throw new NotImplementedException();
            }

            SetSlice(slice1D, depth, axis);
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
                if (IsEdge(depth)) return Get2DSlice(depth, dir);
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
            if (!IsEdge(x) && !IsEdge(y) && !IsEdge(z))
                throw new IndexOutOfRangeException();

            // Ignorant index value
            int index = (y * Size * Size) + (z * Size) + x;
            int iSize = Size - 2; // size of internal space

            // Adjust ignorant index for each value expected in a 1D array that isn't present.
            if (y > 1)
                for (int iy = 1; iy < y; iy++)
                    index -= iSize * iSize;
            if (!IsEdge(y) && z > 1)
            {
                for (int iz = 1; iz < z; iz++)
                    index -= iSize;
            }
            if (!IsEdge(y) && !IsEdge(z) && x > 0)
                index -= iSize;

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
            if (slice is T[] sl1)
            {
                T[] turnedSlice = new T[sl1.Length];
                if (turn.Dir) // clockwise
                {
                    sl1[0..(sl1.Length - (mSize))].CopyTo(turnedSlice, mSize);
                    sl1[(sl1.Length - (mSize))..sl1.Length].CopyTo(turnedSlice, 0);
                }
                else // counterclockwise
                {
                    sl1[(mSize)..sl1.Length].CopyTo(turnedSlice, 0);
                    sl1[0..(mSize)].CopyTo(turnedSlice, sl1.Length - (mSize));
                }

                this[turn.Depth, turn.Axis] = turnedSlice;
            }

            // 2D slice in row by row order
            else if (slice is T[,] sl2)
            {
                T[,] turnedSlice;
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
        private static T[,] Transpose(T[,] mat)
        {
            T[,] trans = new T[mat.GetLength(1), mat.GetLength(0)];
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
        private static T[,] ReverseRows(T[,] mat)
        {
            int colLen = mat.GetLength(0), rowLen = mat.GetLength(1);
            T[,] rev = new T[colLen, rowLen];
            for (int i = 0; i < colLen; i++)
            {
                int s = 0, e = rowLen - 1;
                while (s <= e)
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

            for (int i = 0; i < (mSize) * 4; i++)
            {
                //corners: 0, Size-1, (Size-1)*2, (Size-1)*3

                if (i < Size) // top side
                    indices.Add((0, i));
                else if (i < (mSize) * 2) // right side, no corners
                    indices.Add((i - (mSize), mSize));
                else if (i <= (mSize) * 3) // bottom side
                    indices.Add((mSize, (mSize) * 3 - i));
                else // left side, no corners
                    indices.Add(((mSize) * 4 - i, 0));
            }

            return indices;
        }

        private bool IsEdge(int value)
        {
            return value == 0 || value == mSize;
        }
    }

    /// <summary>
    /// Represents a square plane of cubelets, named relative to its perpendicular axis.
    /// For example, on a 4x4 cube, face 4 is <see cref="Axis"/> 1, <see cref="Depth"/> 3.
    /// </summary>
    public class Slice
    {
        public int Axis;
        public int Depth;
        public bool Dir;
    }
}
