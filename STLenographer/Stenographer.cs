using STLenographer.Data;

using ReaderBase = GenericStl.StlReaderBase<STLenographer.Data.Triangle, STLenographer.Data.Vector3D, STLenographer.Data.Vector3D>;
using WriterBase = GenericStl.StlWriterBase<STLenographer.Data.Triangle, STLenographer.Data.Vector3D, STLenographer.Data.Vector3D>;
using BinaryReader = GenericStl.BinaryStlReader<STLenographer.Data.Triangle, STLenographer.Data.Vector3D, STLenographer.Data.Vector3D>;
using BinaryWriter = GenericStl.BinaryStlWriter<STLenographer.Data.Triangle, STLenographer.Data.Vector3D, STLenographer.Data.Vector3D>;
using AsciiReader = GenericStl.AsciiStlReader<STLenographer.Data.Triangle, STLenographer.Data.Vector3D, STLenographer.Data.Vector3D>;
using AsciiWriter = GenericStl.AsciiStlWriter<STLenographer.Data.Triangle, STLenographer.Data.Vector3D, STLenographer.Data.Vector3D>;
using System.Text;

namespace STLenographer
{
    public class Stenographer
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

        public Stenographer(StlFormat inputFormat, StlFormat outputFormat)
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

                var stenographyWriter = new StenographyWriter(writeHelper);
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
            var stenographyReader = new StenographyReader(readHelper);

            stenographyReader.ReadFromTriangles(reader.ReadFromFile(path));

            return stenographyReader.GetString(System.Text.Encoding.UTF8);
        }
    }
}
