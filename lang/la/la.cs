using System.Collections.Generic;
using System.Text;

namespace System.Language {
    public static partial class Orthography {
        static Func<char, string> ASCII = (char c) => {
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
        class Latin : IOrthography {
            static Func<char, string> ASCII = (char c) => {
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
            ISet<string> stops;
            public Latin() {
                stops = Stops();
            }
            public bool IsStopWord(string hash) {
                return stops.Contains(hash);
            }
            ISet<string> Stops() {
                string Interjections = @"
a the there where why who of from at in about because some other
off like with whom how I you he she their me we us

attat, attatae ecastor ecceedepol, pol en ecce
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
                ISet<string> stops = new HashSet<string>();
                foreach (var s in Conjunctions.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(s);
                }
                foreach (var s in Pronouns.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(s);
                }
                foreach (var s in Interjections.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(s);
                }
                foreach (var s in Esse.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(s);
                }
                return stops;
            }
            public string Hash(string s) {
                if (s == null) {
                    return null;
                }
                StringBuilder r = new StringBuilder();
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
        class Lewis : IOrthography {
            public Lewis() {
            }
            public bool IsStopWord(string hash) {
                return false;
            }
            public string Hash(string s) {
                if (s == null) {
                    return null;
                }
                StringBuilder r = new StringBuilder();
                if (s != null) {
                    for (int i = 0; i < s.Length; i++) {
                        var c = ASCII(s[i]).ToLowerInvariant();
                        r.Append(c);
                    }
                }
                string k = r.ToString();
                return k;
            }
        }
    }
}