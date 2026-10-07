namespace Anima.Core.Utils;

/// <summary>
/// Provides utility methods for creating deterministic and reserved GUIDs.
/// </summary>
public static class ReservedGuid
{
    private const byte SystemAnchor = 0x01;

    /// <summary>
    /// Creates a unique, deterministic <see cref="Guid"/> based on a module index and a serial number.
    /// </summary>
    /// <param name="moduleIndex">The index of the module in the system.</param>
    /// <param name="serial">A serial number within the module.</param>
    /// <returns>A newly created, deterministic <see cref="Guid"/>.</returns>
    public static Guid Create(int moduleIndex, int serial)
    {
        byte[] bytes = new byte[16];
        
        bytes[8] = (byte)((moduleIndex >> 8) & 0xFF);
        bytes[9] = (byte)(moduleIndex & 0xFF);
        bytes[10] = SystemAnchor;
        bytes[12] = (byte)((serial >> 24) & 0xFF);
        bytes[13] = (byte)((serial >> 16) & 0xFF);
        bytes[14] = (byte)((serial >> 8) & 0xFF);
        bytes[15] = (byte)(serial & 0xFF);
        return new Guid(bytes);
    }
}