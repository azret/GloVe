namespace System.Grams {
    public partial class Gram : IComparable<Gram> {
        public static int Compare(int a, int b) {
            return a - b;
        }
        public static int Compare(float[] a, float[] b) {
            if (a == null && b == null) {
                return 0;
            } else if (a != null && b == null) {
                return +1;
            } else if (a == null && b != null) {
                return -1;
            }
            for (int i = 0; i < a.Length && i < b.Length; i++) {
                if (a[i] > b[i]) {
                    return +1;
                } else if (a[i] < b[i]) {
                    return -1;
                }
            }
            if (a.Length > b.Length) {
                return +1;
            } else if (a.Length < b.Length) {
                return -1;
            }
            return 0;
        }
        public static int Compare(string a, string b) {
            if (a == null && b == null) {
                return 0;
            } else if (a != null && b == null) {
                return +1;
            } else if (a == null && b != null) {
                return -1;
            }
            for (int i = 0; i < a.Length && i < b.Length; i++) {
                if (a[i] > b[i]) {
                    return +1;
                } else if (a[i] < b[i]) {
                    return -1;
                }
            }
            if (a.Length > b.Length) {
                return +1;
            } else if (a.Length < b.Length) {
                return -1;
            }
            return 0;
        }
        public static int Score(Gram a, Gram b) {
            if (a == null && b == null) {
                return 0;
            } else if (a != null && b == null) {
                return +1;
            } else if (a == null && b != null) {
                return -1;
            }
            int c = -Gram.Compare(a.Vector, b.Vector);
            if (c == 0) {
                c = Gram.Compare(a.Key, b.Key);
            }
            return c;
        }
        public Gram(string key) {
            if (key == null || key.Length == 0) {
                throw new ArgumentNullException();
            }
            Key = key;
        }
        public Gram(string key, float[] vector) {
            if (key == null || key.Length == 0) {
                throw new ArgumentNullException();
            }
            Key = key;
            Vector = vector;
        }
        public Gram(string key, int hash, Gram prev) {
            if (key == null || key.Length == 0) {
                throw new ArgumentNullException();
            }
            Key = key;
            HashCode = hash;
            Prev = prev;
        }
        int IComparable<Gram>.CompareTo(Gram other) {
            return Compare(Key, other.Key);
        }
        public readonly Gram Prev;
        public readonly int HashCode = -1;
        public override int GetHashCode() {
            return HashCode;
        }
        public readonly string Key;
        public override string ToString() {
            return Key;
        }
        public string ToString(string format) {
            System.Text.StringBuilder fmt = new Text.StringBuilder();
            fmt.Append("[");
            if (Vector != null) {
                for (int j = 0; j < Vector.Length; j++) {
                    if (j > 0) fmt.Append(" ");
                    fmt.Append(Vector[j].ToString(format));
                }
            }
            fmt.Append("]");
            return fmt.ToString();
        }
        public float Norm;
        public float[] Vector;
    }
}