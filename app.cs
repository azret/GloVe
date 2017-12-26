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
            digrams = new Hash();
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

    static void Train(Hash model, Gram[] data, int VECTOR) {
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

                if (canceled) {
                    Console.WriteLine($"Stopping... [{Thread.CurrentThread.ManagedThreadId}]");
                    state.Stop();
                    return;
                }

                for (int m = 0; m < data.Length; m++) {

                    var g = data[r.Next(0, data.Length)];
                    if (canceled) {
                        Console.WriteLine($"Stopping... [{Thread.CurrentThread.ManagedThreadId}]");
                        state.Stop();
                        return;
                    }

                    Gram w, c;
                    lock (model) {
                        
                        string[] window = g.Key.Split();
                        w = model.Get(window[0]);
                        if (w == null) {
                            w = model.Put(window[0]);
                            w.Vector = new float[VECTOR * 2];
                            for (var j = 0; j < w.Vector.Length; j++)
                                w.Vector[j] = (float)r.NextDouble() - 0.5f;
                        }
                        c = model.Get(window[1]);
                        if (c == null) {
                            c = model.Put(window[1]);
                            c.Vector = new float[VECTOR * 2];
                            for (var j = 0; j < w.Vector.Length; j++)
                                c.Vector[j] = (float)r.NextDouble() - 0.5f;
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

    static unsafe void Main(string[] args) {

        /*
         * All Word Vectors (Vw & Vc)
         */

        string MODEL = @".\data\GLOVE.LA";

        /*
         * All Co-occurrences found by when reading books...
         */

        string DIGRAMS = @".\data\DIGRAM.LA";


        /*
         * Books to read/learn from...
         */

        string BOOKS = @".\data\la\"; string PROSE = @".\data\la\prose";

        var lang = Orthography.Create();

        Hash model = new Hash();

        if (File.Exists(MODEL)) {
            Console.WriteLine($"Loading {MODEL}...");
            model = Disk.Load(MODEL, null);
            Console.WriteLine($"Done.");
        }

        const int VECTOR = 37;

        bool train = false; bool reread = false;

        if (train) {
            Hash digrams = null;
            
            /* Read all available books once... */
            if (!File.Exists(DIGRAMS)) {
                Console.WriteLine($"Reading {DIGRAMS}...");
                digrams = CoOccurrences(null, lang, window: 11, paths: BOOKS);
                digrams.Save(DIGRAMS);
                digrams.Clear();
                Console.WriteLine("Done.");
            }

            if (reread) {
                /* Read again but better books... */
                if (File.Exists(DIGRAMS)) {
                    Console.WriteLine($"Loading {DIGRAMS}...");
                    digrams = Disk.Load(DIGRAMS, null);
                }
                Console.WriteLine($"Reading {PROSE}...");
                digrams = CoOccurrences(digrams, lang, window: 11, paths: PROSE);
                digrams.Save(DIGRAMS);
                digrams.Clear();
                Console.WriteLine("Done.");
            }

            Console.WriteLine($"Loading {DIGRAMS}...");
            digrams = Disk.Load(DIGRAMS, (key, vector) => {
                return vector[0] > 0.5;
            });
            Console.WriteLine($"Done.");
            Console.WriteLine($"Shuffling...");
            Gram[] shuffle = Shuffle(digrams);
            Train(model, shuffle, VECTOR);
            Console.WriteLine($"Saving {MODEL}...");
            Console.WriteLine($"Please wait...");
            Disk.Save(model, MODEL);
            Console.WriteLine($"Done.");
        }

        Hash space = new Hash();

        Console.WriteLine($"Merging input and context vectors...");

        model.ForEach((g) => {
            float[] avg = new float[VECTOR];
            for (int j = 0; j < VECTOR; j++) {
                avg[j] = g.Vector[j] + g.Vector[j + VECTOR];
            }
            space.Put(g.Key).Vector = avg;
        });

        model.Clear();

        while (true) {
            Console.Write("W:\\>");
            string key = lang.Hash(Console.ReadLine());
            Gram query = space[key];
            if (query == null) {
                Console.WriteLine("Not Found.");
                continue;
            }
            bool ignoreStopWords = !lang.IsStopWord(key);
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
                results.Add(new Tuple<string, float>(g.Key, Distance(query.Vector, g.Vector)));
            });
            results.Sort((a, b) => {
                if (a.Item2 > b.Item2) {
                    return -1;
                } else if (a.Item2 < b.Item2) {
                    return +1;
                }
                return 0;
            });
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

namespace System.Language {
    public interface IOrthography {
        string Hash(string s);
        bool IsStopWord(string hash);
    }

    public static class Orthography {
        class Latin : IOrthography {
            Hash stops;
            public Latin() {
                stops = Stops();
            }
            public bool IsStopWord(string hash) {
                return stops.Get(hash) != null;
            }
            Hash Stops() {
                string Interjections = @"
attat, attatae ecastor ecceedepol, pol
ehem eheu eho ei, hei em, hem eu euge, eu, eugepae
euhoe heia, eia hercle, mehercle heu heus hui io o
ohe, ohe iam papae pax pro st vae, vae vah";

                string Conjunctions = @"
non ne nec neu ni haud nev neque neue neve
et ed atque ac quoque etiam quidem
sic ita idem item enim
sed aut siue sive sev at autem vel uel absque tamen agitur sin
si nisi tum tunc etsi etiamsi igitur an
cum dum ut vt nam namque vtque utque utique uti cumque
ad in de a ab abs e ex inter
pro erga per intra propter propterea post
contra apud aput sine ob ante usque";

                string Pronouns = @"
ego ego tu
mei tui sui
mihi mihi mi tibi sibi
me te se sese
me te se sese
ego tu
meus tuus suus
nos vos nostri nostrum vestri vestrum sui
nobis vobis sibi
nos vos se sese
nobis vobis se sese
nos vos
noster vester voster suus
mecum tecum vobiscum nobiscum

suus sua suum sui suae sua
sui suae sui suorum suarum suorum
suo suo suis
suum suam suum suos suas sua
suo sua suo suis
sue sua suum sui suae sua

meus mea meum mei meae mea
mei meae mei meorum mearum meorum
meo meo meis
meum meam meum meos meas mea
meo mea meo meis
mi mea meum mei meae mea

noster nostra nostrum nostri nostrae nostra
nostri nostrae nostri nostrorum nostrarum nostrorum
nostro nostro nostris
nostrum nostram nostrum nostros nostras nostra
nostro nostra nostro nostris
noster nostra nostrum nostri nostrae nostra

vester vestra vestrum vestri vestrae vestra
vestri vestrae vestri vestrorum vestrarum vestrorum
vestro vestro vestris
vestrum vestram vestrum vestros vestras vestra
vestro vestra vestro vestris
vester vestra vestrum vestri vestrae vestra

tuus tua tuum tui tuae tua
tui tuae tui tuorum tuarum tuorum
tuo tuo tuis
tuum tuam tuum tuos tuas tua
tuo tua tuo tuis
tue tua tuum tui tuae tua

is ea id it ei ii eae ea
eius ejus eorum earum eorum
ei eis iis
eum eam id eos eas ea
eo ea eo eis iis

iste ista istud isti istae ista
istius istorum istarum istorum
isti istis
istum istam istud istos istas ista
isto ista isto istis

ille illa illud illi illae illa
illius illorum illarum illorum
illi illis
illum illam illud illos illas illa
illo illa illo illis

ipse ipsa ipsum ipsi ipsae ipsa
ipsius ipsorum ipsarum ipsorum
ipsi ipsis
ipsum ipsam ipsum ipsos ipsas ipsa
ipso ipsa ipso ipsis

hic haec hoc hi hii hae haec
huius huiusce hujus horum harum horum
huic his
hunc hanc hoc hos has haec
hoc hac hoc his

aliquis aliquid aliqui aliquae
alicuius
aliquorum aliquarum aliquorum
alicui aliquibus
aliquem aliquid aliquos aliquas aliquae
aliquo aliquibus

idem eadem idem eidem idem eaedem eadem
eiusdem ejusdem eorundem earundem eorundem
eidem eisdem isdem
eundem eandem idem eosdem easdem eadem
eodem eadem eodem eisdem isdem
alius alia aliud alii aliae alia
alius alius alius aliorum aliarum aliorum
alii alii alii aliis aliis aliis
alium aliam aliud alios alias alia
alio alia alio aliis aliis aliis
alie alia aliud alii aliae alia

quis cujus cuius cui quem quo quae cujus cui quam 
qua quid quod  cujus cuius cui quid quod quo qui cujus cui quem 
quo quae cujus cuius cui quam qua quod cujus cui quod quo
qui quorum quibus quos quibus quae quarum 
quibus quas quibus quae quorum quibus quae quibus qui 
quorum quibus quos quibus quae quarum quibus quas 
quibus quae quorum quibus quae quibus";

                const string Esse = @"sum es est sumus estis sunt
eram eras erat eramus eratis erant
ero eris ere erit erimus eritis erunt
fui fuisti fuit fuimus fuistis fuerunt, fuere
fueram fueras fuerat fueramus fueratis fuerant
fuero fueris fuerit fuerimus fueritis fuerint
sim sis sit siet simus sitis sint
essem  forem esses  fores esset  foret essemus  
foremus essetis  foretis essent  forent
fuerim fueris fuerit fuerimus fueritis fuerint
fuissem fuisses fuisset fuissemus fuissetis fuissent
es este esto esto estote sunto esse fuisse futurus esse fore futurus";
                Hash stops = new Hash(7919);
                foreach (var s in Conjunctions.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Put(s);
                }
                foreach (var s in Pronouns.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Put(s);
                }
                foreach (var s in Interjections.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Put(s);
                }
                foreach (var s in Esse.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Put(s);
                }
                return stops;
            }
            public string Hash(string s) {
                StringBuilder r = new StringBuilder();
                Func<char, string> ASCII = (char c) => {
                    switch (c) {
                        case 'Æ': return "AE";
                        case 'æ': return "ae";
                        case 'Œ': return "OE";
                        case 'œ': return "oe";
                        case 'a': return "a";
                        case 'e': return "e";
                        case 'i': return "i";
                        case 'o': return "o";
                        case 'u': return "u";
                        case 'y': return "y";
                        case 'A': return "A";
                        case 'E': return "E";
                        case 'I': return "I";
                        case 'O': return "O";
                        case 'U': return "U";
                        case 'Y': return "Y";
                        case 'â': return "a";
                        case 'ê': return "e";
                        case 'î': return "i";
                        case 'ô': return "o";
                        case 'û': return "u";
                        case 'Â': return "A";
                        case 'Ê': return "E";
                        case 'Î': return "I";
                        case 'Ô': return "O";
                        case 'Û': return "U";
                        case 'à': return "a";
                        case 'è': return "e";
                        case 'ì': return "i";
                        case 'ò': return "o";
                        case 'ù': return "u";
                        case 'À': return "A";
                        case 'È': return "E";
                        case 'Ì': return "I";
                        case 'Ò': return "O";
                        case 'Ù': return "U";
                        case 'á': return "a";
                        case 'é': return "e";
                        case 'í': return "i";
                        case 'ó': return "o";
                        case 'ú': return "v";
                        case 'ý': return "y";
                        case 'Á': return "A";
                        case 'É': return "E";
                        case 'Í': return "I";
                        case 'Ó': return "O";
                        case 'Ú': return "U";
                        case 'Ý': return "Y";
                        case 'ă': return "a";
                        case 'ĕ': return "e";
                        case 'ĭ': return "i";
                        case 'ŏ': return "o";
                        case 'ŭ': return "u";
                        case 'ў': return "y";
                        case 'Ă': return "A";
                        case 'Ĕ': return "E";
                        case 'Ĭ': return "I";
                        case 'Ŏ': return "O";
                        case 'Ŭ': return "U";
                        case 'Ў': return "Y";
                        case 'ā': return "a";
                        case 'ē': return "e";
                        case 'ī': return "i";
                        case 'ō': return "o";
                        case 'ū': return "u";
                        case 'ȳ': return "y";
                        case 'Ā': return "A";
                        case 'Ē': return "E";
                        case 'Ī': return "I";
                        case 'Ō': return "O";
                        case 'Ū': return "U";
                        case 'Ȳ': return "Y";
                    }
                    return c.ToString();
                };
                if (s != null) {
                    for (int i = 0; i < s.Length; i++) {
                        var c = ASCII(s[i]).ToLowerInvariant();
                        switch (c) {
                            case "j": c = "i"; break;
                            case "v": c = "u"; break;
                        }
                        r.Append(c);
                    }
                }
                Func<string, string> PURE = (key) => {
                    for (int i = 0; i < key.Length; i++) {
                        if (!char.IsLetter(key[i])) {
                            return null;
                        }
                    }
                    int vowels = 0; int consonants = 0; int unknown = 0;
                    for (int i = 0; i < key.Length; i++) {
                        switch (key[i]) {
                            case 'a':
                            case 'e':
                            case 'i':
                            case 'o':
                            case 'u':
                            case 'y':
                            case 'A':
                            case 'E':
                            case 'I':
                            case 'O':
                            case 'U':
                            case 'Y':
                                vowels++;
                                break;
                            case 'b':
                            case 'c':
                            case 'd':
                            case 'f':
                            case 'g':
                            case 'h':
                            case 'j':
                            case 'k':
                            case 'l':
                            case 'm':
                            case 'n':
                            case 'p':
                            case 'q':
                            case 'r':
                            case 's':
                            case 't':
                            case 'v':
                            case 'x':
                            case 'z':
                                consonants++;
                                break;
                            default:
                                unknown++;
                                break;
                        }
                    }
                    if (vowels <= 0 || unknown > 0) {
                        return null;
                    }

                    bool hasTriplets = false;
                    for (int i = 0; i < key.Length; i++) {
                        if (i + 1 < key.Length && i + 2 < key.Length) {
                            if (key[i] == key[i + 1] && key[i + 1] == key[i + 2]) {
                                hasTriplets = true;
                                break;
                            }
                        }
                    }
                    if (hasTriplets) {
                        return null;
                    }

                    bool isNumeral = true;
                    for (int i = 0; i < s.Length; i++) {
                        switch (s[i]) {
                            case 'i':
                            case 'v':
                            case 'x':
                            case 'l':
                            case 'c':
                            case 'd':
                            case 'm':
                            case 'I':
                            case 'V':
                            case 'X':
                            case 'L':
                            case 'C':
                            case 'D':
                            case 'M':
                                break;
                            default:
                                isNumeral = false;
                                break;
                        }
                        if (!isNumeral) {
                            break;
                        }
                    }

                    if (isNumeral) {
                        switch (key) {
                            case "cilici":
                            case "cilicii":
                            case "cilii":
                            case "cilix":
                            case "ciuili":
                            case "dic":
                            case "dici":
                            case "didi":
                            case "didici":
                            case "didii":
                            case "dili":
                            case "dilixi":
                            case "dimidi":
                            case "dimidii":
                            case "diui":
                            case "dixi":
                            case "id":
                            case "illi":
                            case "illic":
                            case "illici":
                            case "illidi":
                            case "illim":
                            case "illud":
                            case "illum":
                            case "iudicium":
                            case "iudicum":
                            case "iuli":
                            case "iulii":
                            case "iulium":
                            case "lici":
                            case "lili":
                            case "lilii":
                            case "limi":
                            case "limici":
                            case "liui":
                            case "liuii":
                            case "luci":
                            case "lucii":
                            case "lucilium":
                            case "lucium":
                            case "mili":
                            case "milii":
                            case "milium":
                            case "milli":
                            case "mimi":
                            case "mimici":
                            case "mixi":
                            case "uici":
                            case "uidi":
                            case "uili":
                            case "uilici":
                            case "uim":
                            case "uiui":
                            case "uix":
                            case "uixi":
                                break;
                            default:
                                return null;
                        }
                    }
                    return key;
                };
                string k = PURE(r.ToString());
                if (k == null) {
                    return null;
                }
                switch (k) {
                    case "utcumque":
                    case "adusque":
                    case "inique":
                    case "qualiscumque":
                    case "quotcumque":
                    case "susque":
                    case "quandocumque":
                    case "antique":
                    case "ubiquaque":
                    case "plerique":
                    case "simulatque":
                    case "quantumcumque":
                    case "adaeque":
                    case "quoque":
                    case "utercumque":
                    case "denique":
                    case "utroque":
                    case "plerumque":
                    case "utrimque":
                    case "abusque":
                    case "aeque":
                    case "ubicumque":
                    case "quousque":
                    case "neque":
                    case "que":
                    case "atque":
                    case "quantuscumque":
                    case "quotusquisque":
                    case "ubiquomque":
                    case "oblique":
                    case "quinque":
                    case "usque":
                    case "quocumque":
                    case "itaque":
                    case "utrobique":
                    case "quotuscunque":
                    case "plerusque":
                    case "ubique":
                    case "namque":
                    case "quisque":
                    case "peraeque":
                    case "undique":
                    case "quacumque":
                    case "absque":
                    case "utique":
                    case "quicumque":
                    case "quantuluscumque":
                    case "quotienscumque":
                    case "quandoque":
                    case "uterque":
                    case "quomodocumque":
                    case "utrubique":
                    case "cumque":
                        break;
                    default:
                        if (k != "que" && k.EndsWith("que")) {
                            k = k.Substring(0, k.Length - "que".Length);
                        }
                        break;
                }
                return k;
            }
        }

        public static IOrthography Create() {
            return new Latin();
        }
    }
}