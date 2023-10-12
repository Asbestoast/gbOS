using System.IO;
using System.Threading;

namespace SerialDeviceEmulator;
public static class CommandExtension
{
    public static async void WriteAsync(this Stream self, Packet packet, CancellationToken cts)
    {
        await self.WriteAsync(packet.ToBytes(), cts);
    }
}
