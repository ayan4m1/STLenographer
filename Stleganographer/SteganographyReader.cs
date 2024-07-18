using System.Text;
using Stleganographer.Data;

namespace Stleganographer
{
    public class SteganographyReader(ByteReadHelper readHelper)
    {
        private readonly ByteReadHelper readHelper = readHelper;
        private readonly HashSet<Vector3D> knownVertices = new HashSet<Vector3D>();

        public void ReadFromTriangles(IEnumerable<Triangle> triangles)
        {
            foreach (Triangle tri in triangles)
            {
                if (!readHelper.HasReadEverything())
                {
                    if (!knownVertices.Contains(tri.V1))
                    {
                        Vector3D tmp = new Vector3D(tri.V1);
                        ReadStenographyPerVertex(tri.V1, readHelper);
                        knownVertices.Add(tmp);
                    }
                    if (!knownVertices.Contains(tri.V2))
                    {
                        Vector3D tmp = new Vector3D(tri.V2);
                        ReadStenographyPerVertex(tri.V2, readHelper);
                        knownVertices.Add(tmp);
                    }
                    if (!knownVertices.Contains(tri.V3))
                    {
                        Vector3D tmp = new Vector3D(tri.V3);
                        ReadStenographyPerVertex(tri.V3, readHelper);
                        knownVertices.Add(tmp);
                    }
                }
            }
        }

        private void ReadStenographyPerVertex(Vector3D v, ByteReadHelper readHelper)
        {
            if (readHelper.HasReadEverything()) return;
            readHelper.SetCurrentBitAndMove(ReadStenographyByFloat(v.X));
            if (readHelper.HasReadEverything()) return;
            readHelper.SetCurrentBitAndMove(ReadStenographyByFloat(v.Y));
            if (readHelper.HasReadEverything()) return;
            readHelper.SetCurrentBitAndMove(ReadStenographyByFloat(v.Z));
        }

        private bool ReadStenographyByFloat(float val)
        {
            byte[] bts = BitConverter.GetBytes(val);
            return (bts[0] & 1) == 1;
        }

        public string GetString(Encoding encoding)
        {
            if (readHelper.HasReadEverything())
            {
                return encoding.GetString(readHelper.Data.ToArray());
            }
            else
            {
                throw new InvalidOperationException("Data seems to be incomplete!");
            }
        }
    }
}
