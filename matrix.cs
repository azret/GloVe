namespace System.Grams {
    public class Matrix {
        static int mix(int addr, int seed) {
            addr ^= seed;
            addr = ~addr + (addr << 15);
            addr = addr ^ (addr >> 12);
            addr = addr + (addr << 2);
            addr = addr ^ (addr >> 4);
            addr = addr = (addr + (addr << 3)) + (addr << 11);
            addr = addr ^ (addr >> 16);
            return addr & 0x7fffffff;
        }
        public const int MAX = 15485863;
        class Cell {
            public int i;
            public int j;
            public float value;
            public Cell prev;
        }
        Cell[] data;
        public Matrix(int seed, int size = MAX) {
            this.seed = seed;
            this.data = new Cell[size];
        }
        int seed;
        public int Seed {
            get {
                return seed;
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
        uint version;
        public uint Version {
            get {
                return version;
            }
        }
        public void ForEach(Action<int, int, float> take) {
            for (int pos = 0; pos < data.Length; pos++) {
                Cell c = data[pos];
                while (c != null) {
                    take(c.i, c.j, c.value);
                    c = c.prev;
                }
            }
        }
        public float? this[int i, int j] {
            get {
                int pos = (mix(i * 13103 + j, seed)) % data.Length;
                Cell c = data[pos];
                while (c != null) {
                    if (c.i == i && c.j == j) {
                        break;
                    }
                    c = c.prev;
                }
                if (c != null) {
                    return c.value;
                }
                return null;
            }
        }
        Cell Alloc(int i, int j) {
            int pos = (mix(i * 13103 + j, seed)) % data.Length;
            Cell c = data[pos];
            int iter = 0;
            while (c != null) {
                if (c.i == i && c.j == j) {
                    break;
                }
                c = c.prev;
                iter++;
            }
            if (c == null) {
                c = new Cell() {
                    i = i,
                    j = j,
                    prev = data[pos]
                };
                if (c.prev != null) {
                    collisions++;
                }
                data[pos] = c;
                count++;
                iter++;
                if (iter > depth) {
                    depth = iter;
                }
            }
            version++;
            return c;
        }
        public void Set(int i, int j, float value) {
            Cell c = Alloc(i, j);
            if (c == null) {
                throw new OutOfMemoryException();
            }
            c.value = value;
        }
        public void Add(int i, int j, float value) {
            Cell c = Alloc(i, j);
            if (c == null) {
                throw new OutOfMemoryException();
            }
            c.value += value;
        }
        public void Sub(int i, int j, float value) {
            Cell c = Alloc(i, j);
            if (c == null) {
                throw new OutOfMemoryException();
            }
            c.value -= value;
        }
    }
}