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
        //     get
        //     {
        //         int sliceSize = 2 * size + 2 * (size - 2); 
        //         if (y == 0 || y == size - 1) sliceSize = size * size; 

        //         T[] slice = new T[sliceSize];

        //         int i = 0;
        //         foreach (int index in SliceCoords(y, sliceSize))
        //         {
        //             slice[i] = backer[index];
        //             i++;
        //         }

        //         return slice;
        //     }
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

        // private int[] SliceCoords(int y, int sliceSize)
        // {
        //     // Avoid passing sliceSize by using a List?
        //     int[] indicies = new int[sliceSize];

        //     int index = 0;
        //     for (int x = 0; x < size; x++)
        //     {
        //         for (int z = 0; z < size; z++)
        //         {
        //             if ((x != 0 && x != size - 1) && (z != 0 && z != size - 1)) break;

        //             indicies[index] = CoordsToIndex(x, y, z);
        //             index++;
        //         }
        //     }

        //     return indicies;
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