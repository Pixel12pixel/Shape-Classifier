namespace ShapeClassifier.Converters;

public class BitConverter
{
    /// <summary>
    /// Converts an array of boolean values (bits) into an array of bytes. Each byte can represent 8 bits, so the resulting byte array will be smaller than the input boolean array if the length of the boolean array is not a multiple of 8. The method iterates through each bit in the input array and sets the corresponding bit in the output byte array if the boolean value is true.
    /// </summary>
    /// <param name="bits">array of boolean values (bits)</param>
    /// <returns>array of bytes</returns>
    public byte[] GetBytes(bool[] bits)
    {
        var byteCount = (bits.Length + 7) / 8;
        var bytes = new byte[byteCount];

        for (var i = 0; i < bits.Length; i++)
        {
            if (bits[i])
            {
                bytes[i / 8] |= (byte)(1 << (i % 8));
            }
        }

        return bytes;
    }

    /// <summary>
    /// Converts an array of bytes back into an array of boolean values (bits). The method iterates through each byte in the input array and checks each bit within the byte. If a bit is set (i.e., its value is 1), the corresponding boolean value in the output array is set to true; otherwise, it is set to false. The resulting boolean array will have a length that is 8 times the length of the input byte array, as each byte can represent 8 bits.
    /// </summary>
    /// <param name="bytes">array of bytes</param>
    /// <returns>array of boolean values (bits)</returns>
    public bool[] GetBits(byte[] bytes)
    {
        var bits = new bool[bytes.Length * 8];
        for (var i = 0; i < bytes.Length; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                bits[i * 8 + j] = (bytes[i] & (1 << j)) != 0;
            }
        }
        return bits;
    }
}