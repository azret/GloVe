namespace System.Grams {
    public class Hash {
        public const int MAX = 15485863;
        public static int GetHashCode(string key) {
            int h = 0x1505;
            for (int i = 0; i < key.Length; i++) {
                h ^= (h << 0x5) + key[i] + (h >> 2);
            }
            return h & 0x7fffffff;
        }
        public Hash(int size = MAX) {
            nodes = new Gram[size];
        }
        uint version;
        public uint Version {
            get {
                return version;
            }
        }
        Gram[] nodes;
        public Gram[] Nodes {
            get {
                return nodes;
            }
        }
        public Gram this[string key] {
            get {
                return Get(key);
            }
        }
        int count;
        public int Count {
            get {
                return count;
            }
        }
        int collisions;
        public int Collisions {
            get {
                return collisions;
            }
        }
        int depth;
        public int Depth {
            get {
                return depth;
            }
        }
        public void ForEach(Action<Gram> take) {
            for (int i = 0; i < nodes.Length; i++) {
                Gram g = nodes[i];
                while (g != null) {
                    take(g);
                    g = g.Prev;
                }
            }
        }
        public System.Collections.Generic.IEnumerable<Gram> All() {
            for (int i = 0; i < nodes.Length; i++) {
                Gram g = nodes[i];
                while (g != null) {
                    yield return g;
                    g = g.Prev;
                }
            }
        }
        public void Clear(bool gc = true) {
            for (int i = 0; i < nodes.Length; i++) {
                nodes[i] = null;
            }
            version = 0;
            count = 0;
            collisions = 0;
            depth = 0;
            if (gc) {
                GC.Collect();
                GC.WaitForFullGCComplete();
            }
        }
        public Gram[] List() {
            Gram[] list = new Gram[Count];
            ForEach(take: (g) => {
                list[g.ID] = g;
            });
            return list;
        }
        public Gram Get(string key) {
            if (key == null || key.Length == 0) {
                return null;
            }
            int h = GetHashCode(key);
            Gram g = nodes[h % nodes.Length];
            while (g != null) {
                if (g.HashCode == h && Gram.Compare(g.Key, key) == 0) {
                    break;
                }
                g = g.Prev;
            }
            return g;
        }
        public Gram Put(string key) {
            if (key == null || key.Length == 0) {
                throw new ArgumentNullException();
            }
            var iter = 0;
            int h = GetHashCode(key);
            Gram g = nodes[h % nodes.Length];
            while (g != null) {
                if (g.HashCode == h && Gram.Compare(g.Key, key) == 0) {
                    break;
                }
                g = g.Prev;
                iter++;
            }
            if (g == null) {
                g = new Gram(key, h, count, nodes[h % nodes.Length]);
                if (g.Prev != null) {
                    collisions++;
                }
                nodes[h % nodes.Length] = g;
                count++;
                iter++;
                if (iter > depth) {
                    depth = iter;
                }
            }
            version++;
            return g;
        }
    }
}