using Stleganographer.Data;

namespace Stleganographer
{
    class StegaographyWriter
    {
        readonly ByteWriteHelper writeHelper;
        readonly List<Triangle> triangles;
        readonly Dictionary<Vector3D, Vector3D> knownVertices;

        public StegaographyWriter(ByteWriteHelper writeHelper)
        {
            this.writeHelper = writeHelper;
            knownVertices = [];
            triangles = [];
        }

        public List<Triangle> Triangles => triangles;

        public bool HasUnencodedData => writeHelper.HasUnencodedData();

        public void AddTriangles(IEnumerable<Triangle> triangles)
        {
            foreach (Triangle tri in triangles)
            {
                AddTriangle(tri);
            }
        }

        public void AddTriangle(Triangle tri)
        {
            if (writeHelper.HasUnencodedData())
            {
                if (!knownVertices.ContainsKey(tri.V1))
                {
                    Vector3D tmp = new Vector3D(tri.V1);
                    PerformSteganographyPerVertex(tri.V1, writeHelper);
                    knownVertices.Add(tmp, new Vector3D(tri.V1));
                }
                if (!knownVertices.ContainsKey(tri.V2))
                {
                    Vector3D tmp = new Vector3D(tri.V2);
                    PerformSteganographyPerVertex(tri.V2, writeHelper);
                    knownVertices.Add(tmp, new Vector3D(tri.V2));
                }
                if (!knownVertices.ContainsKey(tri.V3))
                {
                    Vector3D tmp = new Vector3D(tri.V3);
                    PerformSteganographyPerVertex(tri.V3, writeHelper);
                    knownVertices.Add(tmp, new Vector3D(tri.V3));
                }
            }

            if (knownVertices.ContainsKey(tri.V1))
            {
                tri.V1.Set(knownVertices[tri.V1]);
            }
            if (knownVertices.ContainsKey(tri.V2))
            {
                tri.V2.Set(knownVertices[tri.V2]);
            }
            if (knownVertices.ContainsKey(tri.V3))
            {
                tri.V3.Set(knownVertices[tri.V3]);
            }
            triangles.Add(tri);
        }

        private void PerformSteganographyPerVertex(Vector3D v, ByteWriteHelper writeHelper)
        {
            if (!writeHelper.HasUnencodedData()) return;
            v.X = PerformSteganographyPerFloat(writeHelper.GetAndMoveCurrentBit(), v.X);
            if (!writeHelper.HasUnencodedData()) return;
            v.Y = PerformSteganographyPerFloat(writeHelper.GetAndMoveCurrentBit(), v.Y);
            if (!writeHelper.HasUnencodedData()) return;
            v.Z = PerformSteganographyPerFloat(writeHelper.GetAndMoveCurrentBit(), v.Z);
        }

        private float PerformSteganographyPerFloat(bool bit, float val)
        {
            byte[] bts = BitConverter.GetBytes(val);
            if (bit)
            {
                bts[0] |= 1;
            }
            else
            {
                bts[0] &= 0xFE;
            }
            return BitConverter.ToSingle(bts, 0);
        }

    }
}
