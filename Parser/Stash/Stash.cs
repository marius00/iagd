using log4net;
using System;
using System.Collections.Generic;
using IAGrim.Parser.Stash;

namespace IAGrim.StashFile {
    public class Stash {
        private static ILog logger = LogManager.GetLogger(typeof(Stash));
        public const int UNKNOWN1 = 2;

        public const int BLOCK_RESULT = 18;

        public const int CURRENT_VERSION = 4;

        public const int UNKNOWN2 = 0;

        public Block Block = new Block();

        public uint Unknown1 = 2u;

        public uint Version = 4u;

        public uint Unknown2 = 0u;

        public string ModLabel = "";

        public bool IsExpansion1 = false;

        public List<StashTab> Tabs = new List<StashTab>();

        public uint Width {
            get {
                if (Tabs.Count > 0)
                    return Tabs[0].Width;
                else
                    return 0;
            }
        }

        public uint Height {
            get {
                if (Tabs.Count > 0)
                    return Tabs[0].Height;
                else
                    return 0;
            }
        }

        public void Write(DataBuffer pBuffer) {
            pBuffer.WriteUInt(0x55555555);
            pBuffer.WriteUInt(2);
            this.Block.WriteStart(0x12, pBuffer);
            pBuffer.WriteUInt(5);
            pBuffer.WriteUInt(0);
            pBuffer.WriteString(this.ModLabel);
            pBuffer.WriteBoolean(this.IsExpansion1); 
            if ((this.Tabs == null) || (this.Tabs.Count < 1)) {
                pBuffer.WriteUInt(0);
            }
            else {
                pBuffer.WriteUInt((uint)this.Tabs.Count);
                for (int i = 0; i < this.Tabs.Count; i++) {
                    this.Tabs[i].Write(pBuffer);
                }
            }
            this.Block.WriteEnd(pBuffer);
        }

        public bool Read(GDCryptoDataBuffer pCrypto) {
            if (!pCrypto.ReadCryptoKey())
                return false;

            bool result;
            if (!pCrypto.ReadCryptoUInt(out this.Unknown1) || this.Unknown1 != 2u) {
                logger.Warn($"Unexpected failure reading transfer stash, expected file to start with 2, got {this.Unknown1}. Transfer file possibly corrupted.");
                return false;
            }

            if (!Block.ReadStart(out this.Block, pCrypto) || this.Block.Result != 18u) {
                return false;
            }

            if (!pCrypto.ReadCryptoUInt(out this.Version) || (this.Version != 5u && this.Version != 4u && this.Version != 8u)) {
                logger.Warn($"Detected stash file version {this.Version}, only version 4, 5 and 8 are supported");
                return false;
            }

            if (!pCrypto.ReadNextCryptoUInt(out this.Unknown2) || this.Unknown2 != 0u)
                return false;

            if (!pCrypto.ReadCryptoString(out this.ModLabel))
                return false;
            
            
            if (this.Version >= 5) {
                if (!pCrypto.ReadCryptoBool(out IsExpansion1)) {
                    logger.Warn($"New version of transfer stash out? Cannot parse version {this.Version}");
                    return false;
                }
            }

            uint numStashTabs = 0u;
            if (!pCrypto.ReadCryptoUInt(out numStashTabs) || numStashTabs > 100)
                return false;


            this.Tabs = new List<StashTab>();
            int num2 = 0;
            while ((long)num2 < (long)((ulong)numStashTabs)) {
                StashTab stashTab = new StashTab(this.Version);
                bool flag6 = !stashTab.Read(pCrypto);
                if (flag6) {
                    result = false;
                    return result;
                }
                this.Tabs.Add(stashTab);
                num2++;
            }
            bool flag7 = !this.Block.ReadEnd(pCrypto);
            result = !flag7;

            return result;
        }

        
    }
}