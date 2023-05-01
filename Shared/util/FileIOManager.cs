using System.IO;

namespace Shared.util
{
    public class FileIOManager
    {
        private const int CHUNK_SIZE = 1024 * 1024 * 300;

        public static void Write(Stream stream, string saveFullPath)
        {
            var directory = Path.GetDirectoryName(saveFullPath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var writer = new BinaryWriter(File.OpenWrite(saveFullPath));
            var reader = new BinaryReader(stream);

            byte[] chunk;

            while ((chunk = reader.ReadBytes(CHUNK_SIZE)).Length > 0) writer.Write(chunk);

            reader.Close();
            writer.Close();
        }
    }
}