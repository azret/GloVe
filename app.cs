using System;
using System.Collections.Generic;
using System.Grams;
using System.IO;
using System.Language;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

static class App {
    public static Hash CoOccurrences(Hash digrams, IOrthography lang, int window, params string[] paths) {
        if (window <= 0 || window > 17) {
            throw new ArgumentOutOfRangeException();
        }
        if (digrams == null) {
            digrams = Hash.Max();
        }
        Document.Scan(paths,
            read: (s, emit) => {
                string k = lang.Hash(s);
                if (k != null && k.Length > 0) {
                    emit(k);
                }
            },
            doc: (file, doc) => {
                for (int i = 0; i < doc.Count; i++) {
                    string w = doc[i];
                    for (int j = i - ((window + 1) / 2); j < i + ((window + 1) / 2) + 1; j++) {
                        if (j >= 0 && j < doc.Count && i != j) {
                            string c = doc[j];
                            if (w != c) {
                                if (Gram.Compare(w, c) > 0) {
                                    string t = w;
                                    w = c;
                                    c = t;
                                }
                                string k = (w + " " + c);
                                lock (digrams) {
                                    float d = ((float)Math.Abs(i - j));
                                    Gram g = digrams.Get(k);
                                    if (g == null) {
                                        g = digrams.Put(k);
                                        if (g == null) {
                                            throw new OutOfMemoryException();
                                        }
                                        g.Vector = new float[] {
                                            0f
                                        };
                                    }
                                    System.Diagnostics.Debug.Assert(g.Vector != null && g.Vector.Length == 1);
                                    g.Vector[0] += 0.5f / d;
                                }
                            }
                        }
                    }
                };
            }
        );
        return digrams;
    }

    static void Train(Hash model, Gram[] shuffle, int VECTOR) {

        void Prepare() {
            Random r = new Random();
            for (int i = 0; i < shuffle.Length; i++) {
                string[] window = shuffle[i].Key.Split();

                Gram w = model.Get(window[0]);
                if (w == null) {
                    w = model.Put(window[0]);
                    w.Vector = new float[VECTOR * 2];
                    for (var j = 0; j < w.Vector.Length; j++)
                        w.Vector[j] = (float)r.NextDouble() - 0.5f;
                }

                w.Norm = 0;

                Gram c = model.Get(window[1]);
                if (c == null) {
                    c = model.Put(window[1]);
                    c.Vector = new float[VECTOR * 2];
                    for (var j = 0; j < w.Vector.Length; j++)
                        c.Vector[j] = (float)r.NextDouble() - 0.5f;
                }

                c.Norm = 0;
            }
        }

        Prepare();

        float sgd(Gram w, Gram c, float Pwc) {

            float dot(float[] Vw, float[] Vc) {
                System.Diagnostics.Debug.Assert(Vw.Length == 2 * VECTOR);
                System.Diagnostics.Debug.Assert(Vc.Length == 2 * VECTOR);
                var y = 0f;
                for (int k = 0; k < VECTOR; k++) {
                    y += Vw[k] * Vc[k + VECTOR];
                }
                return y;
            }

            float f(float x) {
                float y; float Xmax = 100f;
                if (x < Xmax) {
                    y = (float)Math.Pow(x / Xmax, 0.75);
                } else {
                    y = 1;
                }
                return y;
            }

            float J(float x) {
                return (dot(w.Vector, c.Vector) - (float)Math.Log((double)x));
            }

            float ʝ = J(Pwc), ƒ = f(Pwc);

            if (float.IsNaN(ƒ)) {
                System.Diagnostics.Debugger.Break();
            }

            const float α = 0.05f;

            for (int k = 0; k < VECTOR; k++) {
                const float μ = 0.09f;
                float δJw = ƒ * ʝ * c.Vector[k + VECTOR];
                float δJc = ƒ * ʝ * w.Vector[k];
                w.Vector[k] -= α * δJw;
                c.Vector[k + VECTOR] -= α * δJc;
            }

            return 0.5f * ƒ * (ʝ * ʝ);
        }

        for (int iter = 0; iter < 113 * 113; iter++) {

            if (canceled) {
                Console.WriteLine($"Stopping... [{Thread.CurrentThread.ManagedThreadId}]");
                break;
            }

            float E = 0f; int count = 0;

            Shuffle(shuffle);

            System.Threading.Tasks.Parallel.ForEach(shuffle, new System.Threading.Tasks.ParallelOptions() {

            }, (co, state) => {

                if (canceled) {
                    Console.WriteLine($"Stopping... [{Thread.CurrentThread.ManagedThreadId}]");
                    state.Stop();
                    return;
                }

                string[] window = co.Key.Split();

                (Gram w, Gram c) = (model.Get(window[0]), model.Get(window[1]));

                float e;

                E += e = sgd(w, c, co.Vector[0]);

                int n = Interlocked.Increment(ref count);

                E += e = sgd(c, w, co.Vector[0]);

                n = Interlocked.Increment(ref count);

                //if (n % 75703 == 0) {
                //    Console.WriteLine($"{iter:n0} [{n:n0}] : {E / n} Vw('{w.Key}') * Vc('{c.Key}') = {co.Vector[0]} ~ {e}");
                //}

            });

            Console.WriteLine($"{iter:n0} : {count} - {E / count}");

        }

    }

    struct Options {
        public float Log;
        public bool Train;
        public string Language;
        public string Output;
        public string Learnings;
        public string Input;
    }

    static Options ParseParams(string[] args) {
        Options Latin() {
            return new Options() {
                Log = 0.01f,
                Language = "la",
                Input = @".\data\la\",
                Learnings = @".\data\la.co",
                Output = @".\data\la.vec",
            };
        }

        Options Cicero() {
            return new Options() {
                Log = 0.01f,
                Language = "la",
                Input = @".\data\la\classical\prose\classical\cicero\",
                Learnings = @".\data\cic.co",
                Output = @".\data\cic.vec",
            };
        }

        Options English() {
            return new Options() {
                Log = 0.1f,
                Language = "en",
                Input = @".\data\en\",
                Learnings = @".\data\en.co",
                Output = @".\data\en.vec",
            };
        }

        Options options = English();
        options.Train = false;
        return options;
    }

    static bool canceled = false;

    static void Main(string[] args) {
        Console.CancelKeyPress += (sender, e) => {
            Console.WriteLine("Stopping... Please wait...");
            e.Cancel = canceled = true;
        };

        Options options = ParseParams(args);

        var lang = Orthography.Create(options.Language);

        Hash model = Hash.Max();

        if (File.Exists(options.Output)) {
            Console.WriteLine($"Loading {options.Output}...");
            model = Disk.Load(options.Output, null);
            Console.WriteLine($"Done.");
        }

        const int VECTOR = 47;

        if (options.Train) {
            Train(options, lang, model, VECTOR);
        }

        Console.WriteLine($"Preparing vectors...");

        model.ForEach((g) => {
            float[] avg = new float[VECTOR];
            for (int j = 0; j < VECTOR; j++) {
                avg[j] = (/*Vw*/ g.Vector[j] + /*Vc*/ g.Vector[j + VECTOR]) * 0.5f;
            }
            g.Vector = avg;
        });

        Norm(model.All());

        Console.WriteLine($"Done.");

        Analogy(lang, VECTOR, model);
    }

    private static unsafe void Train(Options options, IOrthography lang, Hash model, int VECTOR) {
        Hash digrams = null;

        if (!File.Exists(options.Learnings)) {
            Console.WriteLine($"Reading {options.Input}...");
            digrams = CoOccurrences(null, lang, window: 13, paths: options.Input);
            var sort = digrams.List();
            Array.Sort(sort, Gram.Score);
            sort.Save(options.Learnings);
            digrams.Clear();
            digrams = null;
            sort = null;
            Console.WriteLine("Done.");
        }

        Console.WriteLine($"Loading {options.Learnings}...");
        digrams = Disk.Load(options.Learnings, (key, vector) => {
            return vector[0] >= options.Log;
        });

        Console.WriteLine($"Done.");

        Gram[] shuffle = digrams.List();
        digrams.Clear();
        digrams = null;

        Train(model, shuffle, VECTOR);
        Console.WriteLine($"Saving {options.Output}...");
        Console.WriteLine($"Please wait...");
        Disk.Save(model.All(), options.Output);
        Console.WriteLine($"Done.");
    }

    static unsafe void Analogy(IOrthography lang, int VECTOR, Hash space) {
        Gram[] W = space.List();
        Console.Write("\r\n");
        while (true) {
            Console.Write("W:\\>");

            Hash bans = Hash.Small();

            string[] input = Console.ReadLine().Split(new char[] {
                    '\r', '\n', '\t', '\f', '\v', ' ',
                }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (input.Length == 0) continue;

            if (input.Length == 1 && input[0] == "cls") {
                Console.Clear();
                continue;
            }

            Gram query = new Gram(string.Join(' ', input), new float[VECTOR]);

            float multiplier = +1.0f; int stops = 0; bool ok = true;

            for (int i = 0; i < input.Length; i++) {
                var s = input[i];
                if (s == "+") {
                    multiplier = +1;
                    continue;
                } else if (s == "-") {
                    multiplier = -1;
                    continue;
                }
                string w = lang.Hash(s);
                if (lang.IsStopWord(w)) {
                    stops++;
                }
                Gram Vw = space[w];
                if (Vw == null) {
                    ok = false;
                    Console.Write($"\r\nWord Not Found. '{w}'\r\n");
                    break;
                }
                bans.Put(w);
                query.Add(Vw.Vector, multiplier);
                multiplier = +1;
            }

            Console.Write("\r\n");

            if (!ok) {
                continue;
            }

            query.Norm = Norm(query.Vector);

            var nn = Find(W, query, (key) => {
                if (bans.Has(key)) {
                    return true;
                }
                if (stops > 0) {
                    return false;
                }
                return lang.IsStopWord(key);
            });

            Comparison<(string key, float distance)> Score = (a, b) => {
                if (a.distance > b.distance) {
                    return -1;
                } else if (a.distance < b.distance) {
                    return +1;
                }
                return 0;
            };
            Array.Sort(nn, Score);
            for (int n = 0; n < nn.Length; n++) {
                Console.Write(nn[n].key);
                Console.Write(" ");
                if ((n + 1) % 7 == 0) {
                    Console.Write("\r\n");
                }
            }

            Console.Write("\r\n\r\n");
        }
    }

    static unsafe void Graph(IOrthography lang, int VECTOR, Gram[] W) {
        int score((string key, float distance) a, (string key, float distance) b) {
            if (a.distance > b.distance) {
                return -1;
            } else if (a.distance < b.distance) {
                return +1;
            }
            return 0;
        }
        Console.Clear();
        float total = W.Length; int processed = 0; DateTime sw = DateTime.Now;
        Parallel.ForEach(W, (q) => {
            var results = Find(W, q, (key) => {
                return lang.IsStopWord(key);
            });
            Array.Sort(results, score);
            int p = Interlocked.Increment(ref processed);
            if (p % 31 == 0) {
                lock (Console.Out) {
                    double secs = (double)(DateTime.Now - sw).Seconds;
                    if (secs > 1e-7) {
                        Console.Clear();
                        Console.Write($"\rSpeed: {((double)p / secs):n2}ps ({p}/{total}) ~ {(secs / 3600) * (double)total / (double)p:n2}h");
                    }
                }
            }
        });
    }

    static float Norm(float[] a) {
        float y = 0f;
        for (int j = 0; j < a.Length; j++) {
            y += a[j] * a[j];
        }
        y = (float)Math.Sqrt(y);
        return y;
    }

    static void Norm(IEnumerable<Gram> grams) {
        foreach (var g in grams) {
            g.Norm = Norm(g.Vector);
        }
    }

    static unsafe (string key, float distance)[] Find(Gram[] W,
        Gram q, Func<string, bool> bans) {
        (string key, float distance)[] best = new (string key, float distance)[q.Vector.Length * 2];
        for (int i = 0; i < W.Length; i++) {
            Gram w = W[i];
            float Dot(float[] a, float[] b) {
                float y = 0f;
                for (int j = 0; j < a.Length; j++) {
                    y += a[j] * b[j];
                }
                return y;
            }
            if (bans != null) {
                if (bans(w.Key)) {
                    continue;
                }
            }
            (string key, float distance) re = (
                w.Key,
                    Dot(q.Vector, w.Vector) / (q.Norm * w.Norm)
                );
            int h = 0;
            for (int j = 1; j < best.Length; j++) {
                if (best[j].distance < best[h].distance) {
                    h = j;
                }
            }
            if (best[h].distance < re.distance) {
                best[h] = re;
            }
        }
        return best;
    }

    static unsafe void Shuffle(Gram[] shuffle) {
        Random r = new Random();
        for (int i = 0; i < shuffle.Length; i++) {
            Gram j = shuffle[i];
            int n = r.Next(i, shuffle.Length);
            shuffle[i] = shuffle[n];
            shuffle[n] = j;
        }
    }
}