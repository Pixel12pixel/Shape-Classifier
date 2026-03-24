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
        var outputLength = bits.Length / 8;
        var output = new BitArray[outputLength];

        for (var i = 0; i < outputLength; i++)
        {
            var byteBits = new bool[8];
            var baseIndex = i * 8;
            for (var j = 0; j < 8; j++) byteBits[j] = bits[baseIndex + j];
            output[i] = new BitArray(byteBits);
        }

        return output;
    }



    public bool[,] ToBits(ulong[] data, int size = 224)
    {
        var result = new bool[size, size];

        for (int y = 0; y < size; y++)
        {
            var rowBaseIndex = y * size;

            for (int x = 0; x < size; x++)
            {
                var linearIndex = rowBaseIndex + x;
                var wordIndex = linearIndex >> 6;
                var bitIndex = linearIndex & 63;

                result[y, x] = (data[wordIndex] & (1UL << bitIndex)) != 0;
            }
        }

        return result;
    }
}