using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.BuddyShare {
    public static class GzipCompressionHandler {


        public static string DecodeBase64AndDecompress(string base64) {
            var bytes = Convert.FromBase64String(base64);
            using (var stream = new MemoryStream(bytes)) {
                using (GZipStream gz = new GZipStream(stream, CompressionMode.Decompress, true)) {
                    using (var resultStream = new MemoryStream()) {
                        gz.CopyTo(resultStream);
                        var data = resultStream.ToArray();
                        return Encoding.ASCII.GetString(data);
                    }
                }
            }
        }


        public static string CompressAndConvertToBase64(string json) {
            using (var stream = new MemoryStream()) {
                using (GZipStream gz = new GZipStream(stream, CompressionMode.Compress, true)) {
                    byte[] data = new ASCIIEncoding().GetBytes(json);
                    gz.Write(data, 0, data.Length);
                }

                return Convert.ToBase64String(stream.ToArray());
            }
        }

    }
}
