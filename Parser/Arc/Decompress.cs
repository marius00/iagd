using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using EvilsoftCommons;

namespace IAGrim.Parser.Arc {
    public class Decompress : IDisposable {
        private static ILog logger = LogManager.GetLogger(typeof(Decompress));

        public const String LINE_SEPARATOR = "\n";
        public const char TEXT_SEPARATOR_0A = (char)0x0A;
        public const char TEXT_SEPARATOR_0D = (char)0x0D;

        private String zipFileName;
        private String arcFileName;
        private ARCHeader header;
        private ARCFilePart[] parts;
        public String[] strings;
        private Record[] tocs;
        private FileStream fs;
        private readonly bool lazyInitialize;

        public Decompress(String fileName, bool lazyInitialize) {
            String extension = Path.GetExtension(fileName.ToLower());
            if (".zip".Equals(extension)) {
                zipFileName = fileName;
            }
            else {
                arcFileName = fileName;
            }

            this.lazyInitialize = lazyInitialize;
        }

        public void decompress() {
            if (File.Exists(arcFileName)) {
                if (!string.IsNullOrEmpty(arcFileName)) 
                    decompressARC();

            }
        }

        public bool hasFile(string filename) {
            if (strings == null)
                return false;
            else
                return strings.Contains(filename);
        }


        private void decompressARC() {
            fs = new FileStream(arcFileName, FileMode.Open, FileAccess.Read);
            header = getHeader(fs);
            parts = getFileParts(fs);
            strings = getStrings(fs);
            tocs = getFileToCs(fs);

            // May have gotten less files than the header indicated
            header.num_files = tocs.Length;

            if (!lazyInitialize)
                extractFiles(fs);
            
        }


        [Obsolete]
        private ARCHeader getHeader(byte[] bytes) {
            ARCHeader header = new ARCHeader();

            header.unknown = IOHelper.GetInt(bytes, 0);
            header.version = IOHelper.GetInt(bytes, 4);
            header.num_files = IOHelper.GetInt(bytes, 8);
            header.rec_num = IOHelper.GetInt(bytes, 12);
            header.rec_size = IOHelper.GetInt(bytes, 16);
            header.str_size = IOHelper.GetInt(bytes, 20);
            header.rec_offset = IOHelper.GetInt(bytes, 24);

            if (header.version != 3) {
                logger.WarnFormat("Header version {0} is not supported.", header.version);
                return null;
            }

            return header;
        }


        private ARCHeader getHeader(FileStream fs) {
            ARCHeader header = new ARCHeader();


            header.unknown = IOHelper.ReadInteger(fs);
            header.version = IOHelper.ReadInteger(fs);
            header.num_files = IOHelper.ReadInteger(fs);
            header.rec_num = IOHelper.ReadInteger(fs);
            header.rec_size = IOHelper.ReadInteger(fs);
            header.str_size = IOHelper.ReadInteger(fs);
            header.rec_offset = IOHelper.ReadInteger(fs);

            if (header.version != 3) {
                logger.WarnFormat("Header version {0} is not supported.", header.version);
                return null;
            }

            return header;
        }


        [Obsolete]
        private ARCFilePart[] getFileParts(byte[] bytes) {
            ARCFilePart[] parts = new ARCFilePart[header.rec_num];

            int offset = header.rec_offset;
            for (int i = 0; i < header.rec_num; i = i + 1) {
                ARCFilePart part = new ARCFilePart();

                part.offset = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                part.len_comp = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                part.len_decomp = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                parts[i] = part;
            }

            return parts;
        }


        private ARCFilePart[] getFileParts(FileStream fs) {
            ARCFilePart[] parts = new ARCFilePart[header.rec_num];

            int offset = header.rec_offset;
            fs.Seek(offset, SeekOrigin.Begin);

            for (int i = 0; i < header.rec_num; i = i + 1) {
                ARCFilePart part = new ARCFilePart();

                part.offset = IOHelper.ReadInteger(fs);
                part.len_comp = IOHelper.ReadInteger(fs);
                part.len_decomp = IOHelper.ReadInteger(fs);

                parts[i] = part;
            }

            return parts;
        }


        [Obsolete]
        private String[] getStrings(byte[] bytes) {
            String[] strings = new String[header.num_files];

            int offset = header.rec_offset + header.rec_size;

            for (int i = 0; i < strings.Length; i = i + 1) {
                String s = IOHelper.GetNullString(bytes, offset);

                strings[i] = s;

                offset = offset + s.Length + 1; // + 1 for 0 termination byte
            }

            return strings;
        }


        private String[] getStrings(FileStream fs) {
            String[] strings = new String[header.num_files];

            
            fs.Seek(header.rec_offset + header.rec_size, SeekOrigin.Begin);

            byte[] bytes = new byte[header.str_size];
            fs.Read(bytes, 0, header.str_size);

            int offset = 0;
            for (int i = 0; i < strings.Length; i = i + 1) {
                String s = IOHelper.GetNullString(bytes, offset);
                strings[i] = s;
                offset = offset + s.Length + 1; // + 1 for 0 termination byte
            }

            return strings;
        }


        [Obsolete]
        private Record[] getFileToCs(byte[] bytes) {
            Record[] tocs = new Record[header.num_files];

            int offset = header.rec_offset + header.rec_size + header.str_size;

            for (int i = 0; i < tocs.Length; i = i + 1) {
                Record toc = new Record();

                toc.type = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.offset = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.len_comp = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.len_decomp = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.unknown = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.fileTime = IOHelper.GetLong(bytes, offset);
                offset = offset + 8;

                toc.num_parts = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.index = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.str_len = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                toc.str_offset = IOHelper.GetInt(bytes, offset);
                offset = offset + 4;

                tocs[i] = toc;
            }

            return tocs;
        }


        private Record[] getFileToCs(FileStream fs) {
            List<Record> tocs = new List<Record>();

            int offset = header.rec_offset + header.rec_size + header.str_size;
            fs.Seek(offset, SeekOrigin.Begin);

            for (int i = 0; i < header.num_files; i = i + 1) {
                Record toc = new Record();

                toc.type = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.offset = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.len_comp = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.len_decomp = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.unknown = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.fileTime = IOHelper.ReadLong(fs);
                offset = offset + 8;

                toc.num_parts = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.index = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.str_len = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                toc.str_offset = IOHelper.ReadInteger(fs);
                offset = offset + 4;

                if (toc.len_decomp > 0)
                    tocs.Add(toc);
            }

            return tocs.ToArray();
        }


        private void extractFiles(FileStream fs) {

            //for (int i = 0; i < header.num_files; i++) {
            for (int i = 0; i < header.num_files; i++) {
                tocs[i].data = extractFiles(fs, parts, tocs[i]);
            }
        }
        private static byte[] extractFiles(FileStream fs, ARCFilePart[] parts, Record record) {
            byte[] data = new byte[record.len_decomp];
            // strings[i] is filename
            // tocs[i] is ToC, references parts
            // bytes is references by parts

            //@tocs[i].data = new byte[tocs[i].len_decomp];

            int offDecomp = 0;
            for (int j = 0; j < record.num_parts; j = j + 1) {
                ARCFilePart part = parts[record.index + j];

                if (part.len_comp == part.len_decomp) {
                    // The file part is not compressed
                    byte[] bComp = new byte[part.len_comp];

                    // fs, offset, dest, 0, len
                    fs.Seek(part.offset, SeekOrigin.Begin);
                    fs.Read(bComp, 0, part.len_comp);
                        

                    for (int k = 0; k < bComp.Length; k = k + 1) {
                        data[k + offDecomp] = bComp[k];
                    }
                }
                else {
                    byte[] bComp = new byte[part.len_comp];
                    fs.Seek(part.offset, SeekOrigin.Begin);
                    fs.Read(bComp, 0, part.len_comp);

                    byte[] tmp = LZ4.LZ4Codec.Decode(bComp, 0, bComp.Length, part.len_decomp);
                    Array.Copy(tmp, 0, data, offDecomp, part.len_decomp);

                }

                offDecomp = offDecomp + part.len_decomp;
            }

            return data;
        }





        public byte[] GetTexture(string filename) {
            String s = filename;
            
            var maxlen = Math.Min(strings.Length, tocs.Length);
            for (int i = 0; i < maxlen; i++) {
                if (strings[i].Equals(s)) {
                    return extractFiles(fs, parts, tocs[i]);
                }
            }

            return null;
        }


        public object GetImage(String filename) {
            for (int i = 0; i < strings.Length; i = i + 1) {
                if (strings[i].Equals(filename)) {
                    try {
                        //return DDSLoader.getImage(tocs[i].data);
                    }
                    catch (Exception) {
                        return null;
                    }
                }
            }

            return null;
        }


        public static string RemoveControlChars(string s) {
            // remove the control characters like ^k
            if (!string.IsNullOrEmpty(s)) {
                int pos = s.IndexOf("^");
                while (pos != -1) {
                    string temp = s;

                    s = temp.Substring(0, pos) + temp.Substring(pos + 2);

                    pos = s.IndexOf("^");
                }
            }

            return s;
        }

        /*
        Dictionary<string, string> keyValueDict = new Dictionary<string, string>();
        public IDictionary<string, string> GetTagDictionary(string filename) {
            if (keyValueDict == null || keyValueDict.Count == 0)
                GetTags(filename);
            return keyValueDict;
        }*/


        public List<IItemTag> GetTags(string filename) {
            List<IItemTag> result = new List<IItemTag>();

            string[] lines = GetTagBlob(filename).Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string line in lines) {
                if (!string.IsNullOrEmpty(line.Trim()) && line.StartsWith("tag")) {
                    string[] keyVal = line.Split('=');
                    if (keyVal.Length != 2) {
                        logger.WarnFormat("Could not parse key-value pair \"{0}\"", line);
                    }
                    else {
                        result.Add(new ItemTag { Tag = keyVal[0], Name = keyVal[1] });
                        //keyValueDict[keyVal[0]] = keyVal[1];
                    }
                }
            }

            return result;
        }


        private string GetTagBlob(string filename) {
            string result = string.Empty;
    
            for (int i = 0; i < strings.Length; i++) {
                if (!strings[i].Equals(filename)) continue;
                string text;

                if (tocs[i].text == null) {
                    text = "";
                    string line = "";
          
                    int j = 0;
                    var data = extractFiles(fs, parts, tocs[i]);
                    while (j < data.Length) {

                        bool nextIs0A = j < data.Length - 1 && data[j + 1] == 0x0A;
                        if ((data[j]     == 0x0D) && nextIs0A) {
                            // Line break;
                            text = text + line + LINE_SEPARATOR;
                            line = "";
              
                            j = j + 2;
                        } 
                        else {
                            line = line + (char) data[j];
                    
                            j = j + 1;
                        }
                    }
          
                    tocs[i].text = text;
                } else {
                    text = tocs[i].text;
                }

                return RemoveControlChars(text);
            }
    
            result = RemoveControlChars(result);
    
            return result;
        }

        ~Decompress() {
            Dispose();
        }

        public void Dispose() {
            fs?.Dispose();
        }
    }
}
