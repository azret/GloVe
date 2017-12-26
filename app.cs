using System;
using System.Collections.Generic;
using System.Grams;
using System.IO;
using System.Language;
using System.Text;
using System.Threading;

static class App {
    public static Hash CoOccurrences(Hash digrams, IOrthography lang, int window, params string[] paths) {
        if (window <= 0 || window > 17) {
            throw new ArgumentOutOfRangeException();
        }
        if (digrams == null) {
            digrams = Hash.Huge();
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
                                    g.Vector[0] += 1f / d;
                                }
                            }
                        }
                    }
                };
            }
        );
        return digrams;
    }

    static void Train(Hash model, Hash data, int VECTOR) {

        float sgd(Gram w, Gram c, float target) {

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
                return f(x) * (dot(w.Vector, c.Vector) - (float)Math.Log((double)x));
            }

            float ƒ = J(target);

            if (float.IsNaN(ƒ)) {
                System.Diagnostics.Debugger.Break();
            }

            const float α = 0.05f;

            for (int k = 0; k < VECTOR; k++) {
                float δJw = ƒ * c.Vector[k + VECTOR];
                float δJc = ƒ * w.Vector[k];
                w.Vector[k] -= α * δJw;
                c.Vector[k + VECTOR] -= α * δJc;
            }

            return 0.5f * ƒ * ƒ;
        }

        bool canceled = false;

        Console.CancelKeyPress += (sender, e) => {
            Console.WriteLine("Stopping... Please wait...");
            e.Cancel = canceled = true;
        };

        System.Threading.Tasks.Parallel.For(0, Environment.ProcessorCount, new System.Threading.Tasks.ParallelOptions() {

        }, (episode, state) => {

            float E = 0f; int count = 0; Random r = new Random();

            for (int iter = 0; iter < 13; iter++) {

                Console.WriteLine($"Shuffling...");
                Gram[] shuffle = Shuffle(data);

                if (canceled) {
                    Console.WriteLine($"Stopping... [{Thread.CurrentThread.ManagedThreadId}]");
                    state.Stop();
                    return;
                }

                for (int m = 0; m < shuffle.Length; m++) {

                    var g = shuffle[m];

                    if (canceled) {
                        Console.WriteLine($"Stopping... [{Thread.CurrentThread.ManagedThreadId}]");
                        state.Stop();
                        return;
                    }

                    string[] window = g.Key.Split();

                    Gram w = model.Get(window[0]);
                    if (w == null) {
                        lock (model) {
                            w = model.Get(window[0]);
                            if (w == null) {
                                w = model.Put(window[0]);
                                w.Vector = new float[VECTOR * 2];
                                for (var j = 0; j < w.Vector.Length; j++)
                                    w.Vector[j] = (float)r.NextDouble() - 0.5f;
                            }
                        }
                    }

                    Gram c = model.Get(window[1]);
                    if (c == null) {
                        lock (model) {
                            c = model.Get(window[1]);
                            if (c == null) {
                                c = model.Put(window[1]);
                                c.Vector = new float[VECTOR * 2];
                                for (var j = 0; j < w.Vector.Length; j++)
                                    c.Vector[j] = (float)r.NextDouble() - 0.5f;
                            }
                        }
                    }

                    count++;

                    float e;

                    E += e = sgd(w, c, g.Vector[0]);

                    if (count % 75703 == 0) {
                        Console.WriteLine($"{episode}-{iter:n0} [{count:n0}] : {E / count} Vw('{w.Key}') * Vc('{c.Key}') = {g.Vector[0]} ~ {e}");
                    }
                }

                Console.WriteLine($"{episode:n0} [{count:n0}] : {E / count}");

            }

        });
    }

    struct Params {
        public float Log;
        public string Language;
        public string Output;
        public string Learnings;
        public string Input;
    }

    static Params Latin() {
        return new Params() {
            Language = "la",
            Input = @".\data\la\",
            Learnings = @".\data\DIGRAM.LA",
            Output = @".\data\GLOVE.LA",
        };
    }

    static Params English() {
        return new Params() {
            Language = "en",
            Input = @".\data\en\",
            Learnings = @".\data\en.co",
            Output = @".\data\en.vec",
        };
    }

    static unsafe void Main(string[] args) {

        Params options = English();

        var lang = Orthography.Create(options.Language);

        Hash model = Hash.Huge();

        if (File.Exists(options.Output)) {
            Console.WriteLine($"Loading {options.Output}...");
            model = Disk.Load(options.Output, null);
            Console.WriteLine($"Done.");
        }

        const int VECTOR = 37;

        bool train = false;

        if (train) {
            Hash digrams = null;

            if (!File.Exists(options.Learnings)) {
                Console.WriteLine($"Reading {options.Input}...");
                digrams = CoOccurrences(null, lang, window: 13, paths: options.Input);
                digrams.Save(options.Learnings);
                digrams.Clear();
                Console.WriteLine("Done.");
            }

            Console.WriteLine($"Loading {options.Learnings}...");
            digrams = Disk.Load(options.Learnings, (key, vector) => {
                return vector[0] >= options.Log;
            });
            Console.WriteLine($"Done.");

            Train(model, digrams, VECTOR);
            Console.WriteLine($"Saving {options.Output}...");
            Console.WriteLine($"Please wait...");

            Disk.Save(model, options.Output);
            Console.WriteLine($"Done.");
        }

        Hash space = Hash.Huge();

        Console.WriteLine($"Preparing vectors...");
        Console.WriteLine($"Done.");

        model.ForEach((g) => {
            float[] avg = new float[VECTOR];
            for (int j = 0; j < VECTOR; j++) {
                avg[j] = (g.Vector[j] + g.Vector[j + VECTOR]) * 0.5f;
            }
            space.Put(g.Key).Vector = avg;
        });

        model.Clear();

        Analogy(lang, VECTOR, space);
    }
    
    static unsafe void Analogy(IOrthography lang, int VECTOR, Hash space) {
        Console.Write("\r\n");
        while (true) {
            Console.Write("W:\\>");

            string[] input = Console.ReadLine().Split(new char[] {
                    '\r', '\n', '\t', '\f', '\v', ' ',
                }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (input.Length == 0) continue;

            Hash bans = Hash.Small();

            float[] query = new float[VECTOR];

            bool bOK = true;

            int mult = +1; int num = 0;

            Console.Write("\r\n");
            for (int i = 0; i < input.Length; i++) {
                var s = input[i];
                if (s == "+") {
                    mult = +1;
                    continue;
                } else if (s == "-") {
                    mult = -1;
                    continue;
                }
                string W = lang.Hash(s);
                Gram V = space[W];
                if (V == null) {
                    bOK = false;
                    Console.WriteLine($"Word Not Found. '{W}'");
                    break;
                }
                if (mult > 0) {
                    if (num > 0) {
                        Console.Write($" + ");
                    }
                    Console.Write($"V('{W}')");
                } else if (mult < 0) {
                    if (num > 0) {
                        Console.Write($" ");
                    }
                    Console.Write($"- V('{W}')");
                }
                bans.Put(W);
                for (int j = 0; j < VECTOR; j++) {
                    if (mult > 0) {
                        query[j] += V.Vector[j];
                    } else if (mult < 0) {
                        query[j] -= V.Vector[j];
                    }
                }
                mult = +1;
                num++;
            }
            Console.Write("\r\n");
            Console.Write("\r\n");

            if (!bOK) {
                continue;
            }

            List<Tuple<string, float>> results = Closest(lang,
                VECTOR, space, query, true, bans);

            int take = 128;
            for (int n = 0; n < results.Count && n < take; n++) {
                Console.Write(results[n].Item1);
                Console.Write(" ");
                if ((n + 1) % 11 == 0) {
                    Console.Write("\r\n");
                }
            }

            Console.Write("\r\n\r\n");
        }
    }

    static unsafe List<Tuple<string, float>> Closest(IOrthography lang, int VECTOR, Hash space,
        float[] query, bool ignoreStopWords, Hash bans) {
        List<Tuple<string, float>> results = new List<Tuple<string, float>>();
        space.ForEach((g) => {
            float Dot(float[] a, float[] b) {
                float y = 0f;
                for (int j = 0; j < VECTOR; j++) {
                    y += a[j] * b[j];
                }
                return y;
            }
            float Norm(float[] a) {
                float y = 0f;
                for (int j = 0; j < VECTOR; j++) {
                    y += a[j] * a[j];
                }
                return (float)Math.Sqrt(y);
            }
            float Distance(float[] a, float[] b) {
                return Dot(a, b) / (Norm(a) * Norm(b));
            }
            if (ignoreStopWords) {
                if (lang.IsStopWord(g.Key)) {
                    return;
                }
            }
            if (bans != null) {
                if (bans.Get(g.Key) != null) {
                    return;
                }
            }
            results.Add(new Tuple<string, float>(g.Key, Distance(query, g.Vector)));
        });
        results.Sort((a, b) => {
            if (a.Item2 > b.Item2) {
                return -1;
            } else if (a.Item2 < b.Item2) {
                return +1;
            }
            return 0;
        });
        return results;
    }

    static unsafe Gram[] Shuffle(Hash digrams) {
        Gram[] shuffle = new Gram[digrams.Count]; int m = 0;
        digrams.ForEach((g) => {
            shuffle[m++] = g;
        });
        Array.Resize(ref shuffle, m);
        Random random = new Random();
        for (int i = 0; i < shuffle.Length; i++) {
            Gram tmp = shuffle[i];
            int r = random.Next(i, shuffle.Length);
            shuffle[i] = shuffle[r];
            shuffle[r] = tmp;
        }
        return shuffle;
    }
}