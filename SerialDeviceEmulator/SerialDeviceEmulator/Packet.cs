using System.IO;
using System.Text;

namespace SerialDeviceEmulator;
public struct Packet
{
    public const int SizeOf = 8;

    public byte B1 { get; set; }
    public byte B2 { get; set; }
    public byte B3 { get; set; }
    public byte B4 { get; set; }
    public int I1 { get; set; }

    public Packet()
    {
    }

    public Packet(Stream source)
    {
        using var reader = new BinaryReader(source, Encoding.ASCII, true);
        B1 = reader.ReadByte();
        B2 = reader.ReadByte();
        B3 = reader.ReadByte();
        B4 = reader.ReadByte();
        I1 = reader.ReadInt32();
    }

    public Command Command
    {
        get => (Command)B1;
        set => B1 = (byte)value;
    }

    public int Timestamp
    {
        get => Timestamp;
        set => Timestamp = value;
    }

    public byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        writer.Write(B1);
        writer.Write(B2);
        writer.Write(B3);
        writer.Write(B4);
        writer.Write(I1);
        return stream.ToArray();
    }

    public static Packet CreateHandshakePacket()
    {
        return new Packet
        {
            Command = Command.Version,
            B2 = 1,
            B3 = 4,
        };
    }

    public static Packet CreateJoypadPacket(Button button)
    {
        return new Packet
        {
            Command = Command.Joypad,
            B2 = (byte)button,
        };
    }

    public static Packet CreateSync2Packet(byte data)
    {
        return new Packet
        {
            Command = Command.Sync2,
            B2 = data,
            B3 = (byte)SerialControlFlag.Start,
        };
    }

    public static Packet CreateStatusPacket(StatusFlags flags)
    {
        return new Packet
        {
            Command = Command.Status,
            B2 = (byte)flags,
        };
    }

    public override string ToString()
    {
        if (Command == Command.Status)
        {
            return $"({Command} flags={(StatusFlags)B2} {B3:X2} {B4:X2} {I1:X4})";
        }
        else
        {
            return $"({Command} {B2:X2} {B3:X2} {B4:X2} {I1:X4})";
        }
    }
}