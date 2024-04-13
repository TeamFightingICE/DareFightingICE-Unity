using DareFightingICE.Grpc.Proto;
using System;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;

public class ProtobufUtil
{
    public static Key FromProtoKey(GrpcKey grpcKey)
    {
        return new Key
        {
            A = grpcKey.A,
            B = grpcKey.B,
            C = grpcKey.C,
            D = grpcKey.D,
            U = grpcKey.U,
            L = grpcKey.L,
            R = grpcKey.R
        };
    }

    public static byte[] CompressBytes(byte[] data)
    {
        using var memoryStream = new MemoryStream();
        using (var gzip = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gzip.Write(data, 0, data.Length);
        }
        return memoryStream.ToArray();
    }
}
