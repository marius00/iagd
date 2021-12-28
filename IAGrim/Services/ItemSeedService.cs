using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.Database;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.Services {
    class ItemSeedService {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ItemSeedService));
        public bool DispatchItemSeedInfoRequest(PlayerItem pi) {
            List<byte> buffer = Serialize(pi);

            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "gdiahook", PipeDirection.InOut, PipeOptions.Asynchronous)) {
                try {
                    pipeStream.Connect(250);
                }
                catch (TimeoutException) {
                    // TODO: Verify if GD is running before even trying. Prevent infinite logspam
                    _logger.Debug("Timed out connecting to GD");
                    return false;
                }
                catch (Exception ex) {
                    // TODO: Verify if GD is running before even trying. Prevent infinite logspam
                    _logger.Warn("Exception connecting to GD", ex);
                    return false;
                }

                pipeStream.Write(buffer.ToArray(), 0, buffer.Count);
                _logger.Debug("Wrote item to pipe");
            }

            return true;
        }


        private static List<byte> Serialize(PlayerItem pi) {
            List<byte> buffer = new List<byte>();

            // TODO: Safety check, if record length > 255 abort.
            buffer.AddRange(BitConverter.GetBytes((int)pi.Id));
            buffer.AddRange(BitConverter.GetBytes((int)pi.Seed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.RelicSeed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.EnchantmentSeed));

            buffer.AddRange(BitConverter.GetBytes(pi.BaseRecord.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(pi.BaseRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.PrefixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.PrefixRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.PrefixRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.SuffixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.SuffixRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.SuffixRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.ModifierRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.ModifierRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.ModifierRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.MateriaRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.MateriaRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.MateriaRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.EnchantmentRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.EnchantmentRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.EnchantmentRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.TransmuteRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.TransmuteRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.TransmuteRecord));

            return buffer;
        }
    }
}
