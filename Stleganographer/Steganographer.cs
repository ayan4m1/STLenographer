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
        private ReaderBase reader;
        private WriterBase writer;

        public StlFormat InputFormat
        {
            set
            {
                switch (value)
                {
                    case StlFormat.Binary:
                        reader = new BinaryReader(new DataCreator());
                        break;
                    case StlFormat.ASCII:
                        reader = new AsciiReader(new DataCreator());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public StlFormat OutputFormat
        {
            set
            {
                switch (value)
                {
                    case StlFormat.Binary:
                        writer = new BinaryWriter(new DataExtractor());
                        break;
                    case StlFormat.ASCII:
                        writer = new AsciiWriter(new DataExtractor());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public Steganographer(StlFormat inputFormat, StlFormat outputFormat)
        {
            InputFormat = inputFormat;
            OutputFormat = outputFormat;
        }

        public void Encode(string inputPath, string outputPath, string payload, string? encryptionKey)
        {
            var success = false;
            var triangles = new List<Triangle>();

            triangles.AddRange(reader.ReadFromFile(inputPath));

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

                writer.WriteToFile(outputPath, stenographyWriter.Triangles);
            }
        }

        public string Decode(string path, string? encryptionKey)
        {
            var readHelper = new ByteReadHelper(encryptionKey == null, encryptionKey ?? "");
            var stenographyReader = new SteganographyReader(readHelper);

            stenographyReader.ReadFromTriangles(reader.ReadFromFile(path));

            return stenographyReader.GetString(Encoding.UTF8);
        }
    }
}
