namespace Stleganographer.Data
{
    public class Triangle : IEquatable<Triangle>
    {
        private readonly Vector3D _normal;
        private readonly Vector3D _v1;
        private readonly Vector3D _v2;
        private readonly Vector3D _v3;

        public Triangle(Vector3D v1, Vector3D v2, Vector3D v3, Vector3D normal)
        {
            ArgumentNullException.ThrowIfNull(v1);
            ArgumentNullException.ThrowIfNull(v2);
            ArgumentNullException.ThrowIfNull(v3);
            ArgumentNullException.ThrowIfNull(normal);

            _v1 = new Vector3D(v1);
            _v2 = new Vector3D(v2);
            _v3 = new Vector3D(v3);
            _normal = new Vector3D(normal);
        }

        public Vector3D N
        {
            get { return _normal; }
        }

        public Vector3D V1
        {
            get { return _v1; }
        }

        public Vector3D V2
        {
            get { return _v2; }
        }

        public Vector3D V3
        {
            get { return _v3; }
        }

        public IEnumerable<Triangle> Subdivision
        {
            get
            {
                Vector3D center = new Vector3D((V1.X + V2.X + V3.X) / 3,
                    (V1.Y + V2.Y + V3.Y) / 3,
                    (V1.Z + V2.Z + V3.Z) / 3);

                yield return new Triangle(V1, V2, center, N);
                yield return new Triangle(center, V2, V3, N);
                yield return new Triangle(V1, center, V3, N);
            }
        }

        public override string ToString()
        {
            return $"V1: {_v1}, V1: {_v2}, V1: {_v3}, N: {_normal}";
        }

        public bool Equals(Triangle? other)
        {
            if (other == null)
            {
                return false;
            }

            return other.V1.Equals(V1) && other.V2.Equals(V2) && other.V3.Equals(V3) && other.N.Equals(N);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(V1, V2, V3, N);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Triangle);
        }
    }
}
