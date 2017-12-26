using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace System.Text {
    public static class Document {
        public static void Scan(string search, string[] paths, Action<string, Action<string>> read, Action<string, IList<string>> doc) {
            Scan(Environment.ProcessorCount, paths, search, read, doc);
        }
        public static void Scan(string[] paths, Action<string, Action<string>> read, Action<string, IList<string>> doc) {
            Scan(Environment.ProcessorCount, paths, "*.*", read, doc);
        }
        public static void Scan(int MaxDegreeOfParallelism, string[] paths, string search, Action<string, Action<string>> read, Action<string, IList<string>> process) {
            ISet<string> files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in paths) {
                FileAttributes attr = FileAttributes.Offline;
                try {
                    attr = File.GetAttributes(path);
                } catch (System.IO.FileNotFoundException) {
                }
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                    foreach (var file in Directory.EnumerateFiles(path, search, SearchOption.AllDirectories)) {
                        if (files.Contains(file)) {
                            continue;
                        }
                        files.Add(file);
                    }
                } else {
                    files.Add(path);
                }
            }
            if (process != null) {
                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, (file) => {
                    process(file, Scan(file, read));
                });
            }
        }
        public static void Scan(string plain, Action<string, Action<string>> read, Action<string, IList<string>> doc) {
            if (doc != null) {
                doc(null, Scan(read, plain));
            }
        }
        public static IList<string> Scan(string file, Action<string, Action<string>> read) { return Scan(read, File.ReadAllText(file)); }
        public static IList<string> Scan(Action<string, Action<string>> read, string plain) {
            bool IsPunctuation(char c) {
                switch (c) {
                    case 'ʻ': return true;
                }
                return char.IsPunctuation(c);
            }            
            IList<string> doc = new List<string>();
            for (int i = 0; i < plain.Length;) {
                if (char.IsWhiteSpace(plain[i]) || IsPunctuation(plain[i])) {
                    i++;
                    while (i < plain.Length
                            && (char.IsWhiteSpace(plain[i]))) {
                        i++;
                    }
                } else {
                    int start = i;
                    while (i < plain.Length
                            && !(char.IsWhiteSpace(plain[i]))) {
                        i++;
                    }
                    int end = i;
                    while (start < end
                            && (IsPunctuation(plain[start]) || !char.IsLetter(plain[start]))) {
                        start++;
                    }
                    while (end > start
                            && (IsPunctuation(plain[end - 1]) || !char.IsLetter(plain[end - 1]))) {
                        end--;
                    }
                    int len = end - start;
                    if (len > 0) {
                        string s = plain.Substring(start, len);
                        if (read != null) {
                            read(s, (w) => {
                                if (!String.IsNullOrWhiteSpace(w)) {
                                    doc.Add(w);
                                }
                            });
                        } else {
                            doc.Add(s);
                        }
                    }
                }
            }
            return doc;
        }
    }
}