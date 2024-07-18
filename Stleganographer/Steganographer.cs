using System.Text;
using Stleganographer.Data;

using ReaderBase = GenericStl.StlReaderBase<Stleganographer.Data.Triangle, Stleganographer.Data.Vector3D, Stleganographer.Data.Vector3D>;
using WriterBase = GenericStl.StlWriterBase<Stleganographer.Data.Triangle, Stleganographer.Data.Vector3D, Stleganographer.Data.Vector3D>;
using BinaryReader = GenericStl.BinaryStlReader<Stleganographer.Data.Triangle, Stleganographer.Data.Vector3D, Stleganographer.Data.Vector3D>;
using BinaryWriter = GenericStl.BinaryStlWriter<Stleganographer.Data.Triangle, Stleganographer.Data.Vector3D, Stleganographer.Data.Vector3D>;
using AsciiReader = GenericStl.AsciiStlReader<Stleganographer.Data.Triangle, Stleganographer.Data.Vector3D, Stleganographer.Data.Vector3D>;
using AsciiWriter = GenericStl.AsciiStlWriter<Stleganographer.Data.Triangle, Stleganographer.Data.Vector3D, Stleganographer.Data.Vector3D>;

namespace Stleganographer
{
    public class Steganographer
    {
        private static ReaderBase GetReader(StlFormat format) => format switch
        {
            StlFormat.Binary => new BinaryReader(new DataCreator()),
            StlFormat.ASCII => new AsciiReader(new DataCreator()),
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };

        private static WriterBase GetWriter(StlFormat format) => format switch
        {
            StlFormat.Binary => new BinaryWriter(new DataExtractor()),
            StlFormat.ASCII => new AsciiWriter(new DataExtractor()),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        public static void Encode(string inputPath, string outputPath, StlFormat inputFormat, StlFormat outputFormat, string payload, string? encryptionKey)
        {
            var success = false;
            var triangles = new List<Triangle>();

            triangles.AddRange(GetReader(inputFormat).ReadFromFile(inputPath));

            while (!success)
            {
                var payloadBytes = Encoding.UTF8.GetBytes(payload);
                var writeHelper = new ByteWriteHelper(encryptionKey == null, encryptionKey ?? "");
                writeHelper.AppendData([0x77, 0]); // Magic byte, version
                writeHelper.AppendData(BitConverter.GetBytes(payloadBytes.Length));
                writeHelper.AppendData(payloadBytes);
                writeHelper.FinalizeData();

                var stenographyWriter = new StegaographyWriter(writeHelper);
                stenographyWriter.AddTriangles(triangles);

                if (stenographyWriter.HasUnencodedData)
                {
                    var newTris = new List<Triangle>();
                    foreach (var tri in triangles)
                    {
                        newTris.AddRange(tri.Subdivision);
                    }
                    triangles = newTris;
                    continue;
                }
                else
                {
                    success = true;
                }

                GetWriter(outputFormat).WriteToFile(outputPath, stenographyWriter.Triangles);
            }
        }

        public static string Decode(string path, StlFormat format, string? encryptionKey)
        {
            var readHelper = new ByteReadHelper(encryptionKey == null, encryptionKey ?? "");
            var stenographyReader = new SteganographyReader(readHelper);

            stenographyReader.ReadFromTriangles(GetReader(format).ReadFromFile(path));

            return stenographyReader.GetString(Encoding.UTF8);
        }
    }
}
