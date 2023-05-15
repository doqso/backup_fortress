using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.util
{
    public class FilesIO
    {
        private const int CHUNK_SIZE = 1024 * 1024 * 300;

        public static void Write(Stream fileStream, string pathToSave)
        {
            var directory = Path.GetDirectoryName(pathToSave);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var writer = new BinaryWriter(File.OpenWrite(pathToSave));
            var reader = new BinaryReader(fileStream);

            byte[] chunk;

            while ((chunk = reader.ReadBytes(CHUNK_SIZE)).Length > 0) writer.Write(chunk);

            reader.Close();
            writer.Close();
        }

        public static Task CompressAndWrite(ref string folderPath)
        {
            ZipFile.CreateFromDirectory(folderPath, folderPath += ".zip");

            return Task.CompletedTask;
        }

        public static void RemoveFoldersBackup(string filePath)
        {
            File.Delete(filePath);
        }
    }
}