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
            this.key = key;
        }
        public Gram(string key, float[] vector) {
            if (key == null || key.Length == 0) {
                throw new ArgumentNullException();
            }
            this.key = key;
            this.vector = vector;
        }
        public Gram(string key, int index) {
            if (key == null || key.Length == 0) {
                throw new ArgumentNullException();
            }
            this.key = key;
            this.id = index;
        }
        public Gram(string key, int hash, int index, Gram prev) {
            if (key == null || key.Length == 0) {
                throw new ArgumentNullException();
            }
            this.key = key;
            this.hash = hash;
            this.prev = prev;
            this.id = index;
        }
        int IComparable<Gram>.CompareTo(Gram other) {
            return Compare(this.key, other.key);
        }
        Gram prev;
        public Gram Prev {
            get {
                return prev;
            }
        }
        int hash = -1;
        public int HashCode {
            get {
                return hash;
            }
        }
        public override int GetHashCode() {
            return hash;
        }
        string key;
        public string Key {
            get {
                return key;
            }
        }
        public override string ToString() {
            return key;
        }
        public string ToString(string format) {
            System.Text.StringBuilder fmt = new Text.StringBuilder();
            fmt.Append("[");
            if (vector != null) {
                for (int j = 0; j < vector.Length; j++) {
                    if (j > 0) fmt.Append(" ");
                    fmt.Append(vector[j].ToString(format));
                }
            }
            fmt.Append("]");
            return fmt.ToString();
        }
        int id = -1;
        public int ID {
            get {
                return id;
            }
        }
        float[] vector;
        public float[] Vector {
            get {
                return vector;
            }
            set {
                this.vector = value;
            }
        }
    }
}