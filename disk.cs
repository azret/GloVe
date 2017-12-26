using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Grams {
    public static class Disk {
        public static void Save(this Hash list, string file, int take = -1) { Save(list.All(), file, take); }
        public static void Save(this IEnumerable<Gram> list, string file, int take = -1) {
            void Write(Stream stream, string data) {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                if (bytes != null && bytes.Length > 0) {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            using (var f = File.Open(file, FileMode.Create,
                FileAccess.Write, FileShare.None)) {
                int total = 0,
                    dim = 0;
                foreach (var g in list) {
                    if (g != null && g.Key.Length > 0) {
                        total++;
                        if (g.Vector != null && g.Vector.Length > dim) {
                            dim = g.Vector.Length;
                        }
                    }
                }
                if (take >= 0) {
                    if (total > take) {
                        total = take;
                    }
                }
                int written = 0;
                foreach (var g in list) {
                    if (g != null && g.Key.Length > 0) {
                        if (written >= total) {
                            return;
                        }
                        if (written == 0 /* is first entry ? */) {
                            Write(f, "6053");
                            Write(f, " ");
                            Write(f, total.ToString());
                            Write(f, " ");
                            Write(f, dim.ToString());
                            Write(f, "\n");
                        }
                        Write(f, g.Key.ToString());
                        if (g.Vector != null && g.Vector.Length > 0) {
                            Write(f, " ⇾ [");
                            for (int j = 0; j < g.Vector.Length; j++) {
                                if (j > 0) {
                                    Write(f, " ");
                                }
                                Write(f, g.Vector[j].ToString());
                            }
                            Write(f, "]");
                        }
                        Write(f, "\n");
                        written++;
                    }
                }
                f.Flush();
            }
        }
        struct Line {
            string s;
            int i;
            public Line(string line) {
                this.s = line;
                this.i = 0;
            }
            public int Length {
                get { return s.Length; }
            }
            public char Char {
                get { return s[i]; }
            }
            public bool IsEof {
                get {
                    if (s != null && i < s.Length) {
                        return false;
                    }
                    return true;
                }
            }
            public void Skip() { if (i < s.Length) { i++; } }
            public void SkipWhite() { while (i < s.Length && char.IsWhiteSpace(s[i])) { i++; } }
            public string ReadKey() {
                int start = i;
                while (i < s.Length && (s[i] != '⇾' && s[i] != '\r' && s[i] != '\n')) {
                    if (char.IsWhiteSpace(s[i]) &&
                                    i + 1 < s.Length && s[i + 1] == '⇾') {
                        break;
                    }
                    i++;
                }
                int end = i;
                return s.Substring(
                    start, end - start);
            }
            public string ReadInt() {
                int start = i;
                while (i < s.Length && (s[i] == '-' || char.IsDigit(s[i]))) {
                    i++;
                }
                int end = i;
                return s.Substring(
                    start, end - start);
            }
            public string ReadFloat() {
                int start = i;
                while (i < s.Length && (s[i] == '.' || s[i] == '-' || s[i] == 'e' || s[i] == 'E' || char.IsDigit(s[i]))) {
                    i++;
                }
                int end = i;
                return s.Substring(
                    start, end - start);
            }
        }
        public static Hash Load(string file, Func<string, float[], bool> take) {
            Hash table = new Hash();
            using (var stream = File.OpenText(file)) {
                Line h = new Line(stream.ReadLine());
                int magic;
                if (!int.TryParse(h.ReadInt(), out magic) || magic != 6053) {
                    throw new InvalidDataException();
                }
                h.SkipWhite();
                int total;
                if (!int.TryParse(h.ReadInt(), out total)) {
                    throw new InvalidDataException();
                }
                h.SkipWhite();
                int dim;
                if (!int.TryParse(h.ReadInt(), out dim)) {
                    throw new InvalidDataException();
                }
                int read = 0; float[] buff = new float[0];
                for (; ; ) {
                    Line r = new Line(stream.ReadLine());
                    if (r.IsEof) {
                        break;
                    }
                    string key = r.ReadKey();
                    if (key == null || key.Length == 0) {
                        throw new InvalidDataException();
                    }
                    float[] vector = null;
                    r.SkipWhite();
                    if (!r.IsEof && r.Char == '⇾') {
                        r.Skip(); int j = 0;
                        r.SkipWhite();
                        if (r.IsEof || r.Char != '[') {
                            throw new InvalidDataException();
                        }
                        r.Skip();
                        while (!r.IsEof && r.Char != ']') {
                            r.SkipWhite();
                            float n; string f = r.ReadFloat();
                            if (!float.TryParse(f, out n)) {
                                throw new InvalidDataException();
                            }
                            if (buff.Length < j + 1) {
                                Array.Resize(ref buff, j + 1);
                            }
                            buff[j++] = n;
                        }
                        if (r.IsEof || r.Char != ']') {
                            throw new InvalidDataException();
                        }
                        if (j > 0) {
                            vector = new float[j];
                            Array.Copy(buff, vector, j);
                        }
                    }
                    if (take != null) {
                        if (!take(key, vector)) {
                            key = null;
                        }
                    }
                    if (key != null) {
                        Gram g = table.Put(key);
                        if (g == null) {
                            throw new OutOfMemoryException();
                        }
                        g.Vector = vector;
                    }
                    read++;
                }
                if (total != read) {
                    throw new InvalidDataException();
                }
            }
            return table;
        }
    }
}