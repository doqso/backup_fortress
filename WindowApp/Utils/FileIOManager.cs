using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowApp.Utils
{
    public class FileIOManager
    {
        private const int CHUNK_SIZE = 1024 * 1024 * 300;

        public static void Write(Stream stream, string saveFullPath)
        {
            var directory = Path.GetDirectoryName(saveFullPath);
            
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using var writer = new BinaryWriter(File.OpenWrite(saveFullPath));
            using var reader = new BinaryReader(stream);
            
            byte[] chunk;

            while ((chunk = reader.ReadBytes(CHUNK_SIZE)).Length > 0)
            {
                writer.Write(chunk);
            }
        }
    }
}