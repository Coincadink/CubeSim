using System;
using System.Collections;
using System.Collections.Generic;

namespace HollowArray
{
    /// <summary>
    /// Represents a hollow 3D cube as a 3D array.
    /// Only allocates memory for items on the surface of the cube.
    /// </summary>
    public class HollowArray<T> : IEnumerable<T>
    {
        readonly T[] backer;
        readonly int size;

        /// <summary>
        /// Create a new HollowArray of side length <paramref name="size"/>.
        /// </summary>
        /// <param name="size"></param>
        public HollowArray(int size) {
            this.size = size;

            int arrSize = size * size * size;
            arrSize -= Math.Max(0, (int)Math.Pow(size - 2, 3)); // Remove internal spaces
            backer = new T[arrSize];
        }

        public T[] GetSlice(int depth, int dir = 0)
        {
            switch (dir)
            {
                case 0: // Y
                    // Return the face as a 1D array, ordered left to right, top to bottom
                    if (depth == 0 || depth == size - 1) {
                        return backer[CoordsToIndex(0, depth, 0)..CoordsToIndex(size-1, depth, size-1)];
                    }

                    //corners: 0, size-1, (size-1)*2, (size-1)*3

                    // Return the slice as a 1D array, ordered in clockwise order, starting at (x, z) = (0, 0).
                    int perimSize = (size - 1) * 4;
                    T[] slice = new T[perimSize];
                    for (int i = 0; i < perimSize; i++)
                    {
                        if (i < size) // top side
                            slice[i] = this[0, depth, i];
                        else if (i < (size - 1) * 2) // right side, no corners
                            slice[i] = this[i - (size - 1), depth, size - 1];
                        else if (i <= (size - 1) * 3) // bottom side
                            slice[i] = this[size - 1, depth, (size * 2) - i];
                        else // left side, no corners
                            slice[i] = this[(size * 3) - 1 - i, depth, 0];
                    }
                    return slice;
                default:
                    throw new NotImplementedException();
            }
        }

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

        public Array this[int depth, int dir = 0]
        {
            get
            {
                if (depth == 0 || depth == size - 1) return Get2DSlice(depth, dir);
                else return GetSlice(depth, dir);
            }
            set
            {
                throw new NotImplementedException();
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

        // public T[] this[int y]
        // {
        //     set
        //     {
        //         int sliceSize = 2 * size + 2 * (size - 2);
        //         if (y == 0 || y == size - 1) sliceSize = size * size;

        //         int i = 0;
        //         foreach (int index in SliceCoords(y, sliceSize))
        //         {
        //             backer[index] = value[i];
        //             i++;
        //         }
        //     }
        // }

        

        /// <summary>
        /// Converts 3D coordinates to a 1D coordinate. Removes coordinate values associated with empty space.
        /// Throws <seealso cref="IndexOutOfRangeException"/> if the coordinate exists in empty space inside the cube.
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

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)backer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return backer.GetEnumerator();
        }
    }
}