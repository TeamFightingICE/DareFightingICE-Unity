using DareFightingICE.Grpc.Proto;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class GrpcUtil
{
    public static Key FromGrpcKey(GrpcKey grpcKey)
    {
        Key key = new Key();
        key.A = grpcKey.A;
        key.B = grpcKey.B;
        key.C = grpcKey.C;
        key.D = grpcKey.D;
        key.U = grpcKey.U;
        key.L = grpcKey.L;
        key.R = grpcKey.R;
        return key;
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
