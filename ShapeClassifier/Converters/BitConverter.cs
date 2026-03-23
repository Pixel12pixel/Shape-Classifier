using System.Collections;

namespace ShapeClassifier.Converters;

public class BitConverter
{
    /// <summary>
    ///     Converts a 2D array of booleans into an array of BitArrays, where each BitArray represents 8 bits (1 byte) of the
    ///     original boolean array.
    /// </summary>
    /// <param name="bits">array of booleans</param>
    /// <returns>array of BitArrays</returns>
    public BitArray[] ToBitArrays(bool[] bits)
    {
        var output = new BitArray[bits.Length / 8];
        for (var i = 0; i < output.Length; i++) output[i] = new BitArray(bits.Skip(i * 8).Take(8).ToArray());
        return output;
    }

    public BitArray[] ToBitArrays(bool[,] bits)
    {
        var output = new BitArray[bits.Length / 8];
        for (var i = 0; i < output.Length; i++)
        {
            var byteBits = new bool[8];
            for (var j = 0; j < 8; j++)
            {
                var index = i * 8 + j;
                if (index < bits.Length) byteBits[j] = bits[index / bits.GetLength(1), index % bits.GetLength(1)];
            }

            output[i] = new BitArray(byteBits);
        }
        return output;
    }

    /// <summary>
    ///     Converts an array of BitArrays back into a 2D array of booleans. The method iterates through each BitArray and
    ///     extracts the individual bits, reconstructing the original boolean array based on the specified size (default is
    ///     224x224). The resulting 2D boolean array represents the original data that was converted into BitArrays.
    /// </summary>
    /// <param name="bitArrays">array of BitArrays</param>
    /// <param name="size">target array size</param>
    /// <returns>array of booleans</returns>
    public bool[,] ToBits(BitArray[] bitArrays, int size = 224)
    {
        var bits = new bool[size, size];
        for (var i = 0; i < bitArrays.Length; i++)
        for (var j = 0; j < 8; j++)
        {
            var index = i * 8 + j;
            if (index < size * size) bits[index / size, index % size] = bitArrays[i][j];
        }

        return bits;
    }
}