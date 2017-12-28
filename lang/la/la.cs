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
            ISet<string> stops;
            public Latin() {
                stops = Stops();
            }
            public bool IsStopWord(string hash) {
                return stops.Contains(hash);
            }
            ISet<string> Stops() {
                string Import = @"
about
all
am
an
and
are
as
at
be
been
but
by
can
cannot
did
do
does
doing
done
for
from
had
has
have
having
if
in
is
it
its
of
on
that
the
these
they
this
those
to
too
want
wants
was
what
which
will
with
would

age
attat, attatae
ecastor
ecce
edepol, pol
ehem
eheu
eho
ei, hei
em, hem
eu
euge, eu, eugepae
euhoe
heia, eia
hercle, mehercle
heu
heus
hui
io
o
ohe, ohe iam
papae
pax
pro
pro pro 
st
vae, vae
vah

non sic iam ac adque alii an annon antequam ast at ateque atque atqui aut autem 
cum donec dum dummodo enim ergo et ed etenim etiamsi etiamtum etiamtunc etsi 
nam namque nanque ne nec necdum necne necunde nedum neque nequedum neve ni nisi nonnisi 
ita etiam idem item quoque igitur haud equidem neu

postquam priusquam prout 
quam quamquam quando quandoquidem quanquam quantumvis quasi 
quia quocirca quod quodsi quom quominus quoniam quum

sed set seu si sicut sine sin siquidem siue sive tamen tametsi
ut vt utei uti utrum utrumnam vel uel verumtamen 
veruntamen vero verum solo solum

quidem quodam quiddem quodem

cur num qua quando quare equid
quemadmodum quantum
qui quid quo quomodo

#

ubi unde modo modum modi vero tum ubi ibi adhuc ideo tantum deinde ideo sat satis nunc tam 
causa magis primum satis adeo semper hinc saepe huc tunc multa numquam minus olim 
opus prius haud maxime licet simul solum tamquam postea propterea

#

a ab abs absque
coram cum de e ex
prae pro tenus in sub subter super ad 
adversus adversum ante
apud circa circum circiter contra
erga extra infra
inter intra iuxta
ob per post praeter prope propter propterea
secundum supra trans ultra versus uersus

#
illic

numquid scilicet

quamuis
quamvis quidquid quondam quaeque quale qualiter quotiens 
	quaecumque quaedamquicumque qualem quales 
quamlibet quanta quanto quot

quadam quaedam quicquam quaeque quale qualitatem 
	quibusdam quin quoddam qualia quasdam quidque quisquis 
quodque quonam quosdam quidni equid quiddam quidam
 
quis	quid		qui	quae
cuius   cujus	quorum	quarum	quorum
cui	quibus
quem	quid	quos	quas	quae
quo	quibus quoquo

qui	quae	quod	quoad		qui	quae
cuius  cujus	quorum	quarum	quorum
cui	quibus
quem	quam	quod	quos	quas	quae
quo	qua	quo	quibus

sum	es	est	sumus	estis	sunt
eram	eras	erat	eramus	eratis	erant
ero	eris, ere	erit	erimus	eritis	erunt
fui	fuisti	fuit	fuimus	fuistis	fuerunt, fuere
fueram	fueras	fuerat	fueramus	fueratis	fuerant
fuero	fueris	fuerit	fuerimus	fueritis	fuerint

sim	sis	sit siet	simus	sitis	sint
essem	 forem	esses	 fores	esset	 foret	essemus	 foremus	essetis	 foretis	essent	 forent
fuerim	fueris	fuerit	fuerimus	fueritis	fuerint
fuissem	fuisses	fuisset	fuissemus	fuissetis	fuissent

es	este esto	esto	estote	sunto esse	fuisse	futurus esse fore futurus

possum	potes	potest	possumus	potestis	possunt
poteram	poteras	poterat	poteramus	poteratis	poterant
potero	poteris, potere	poterit	poterimus	poteritis	poterunt
potui	potuisti	potuit	potuimus	potuistis	potuerunt, potuere
potueram	potueras	potuerat	potueramus	potueratis	potuerant
potuero	potueris	potuerit	potuerimus	potueritis	potuerint

possim	possis	possit	possimus	possitis	possint

possem	posses	posset	possemus	possetis	possent
potuerim	potueris	potuerit	potuerimus	potueritis	potuerint
potuissem	potuisses	potuisset	potuissemus	potuissetis	potuissent
posse	potuisse
potens

habeo	habes	habet	habemus	habetis	habent
habebam	habebas	habebat	habebamus	habebatis	habebant
habebo	habebis	habebit	habebimus	habebitis	habebunt
habui	habuisti	habuit	habuimus	habuistis	habuerunt, habuere
habueram	habueras	habuerat	habueramus	habueratis	habuerant
habuero	habueris	habuerit	habuerimus	habueritis	habuerint
habeor	haberis, habere	habetur	habemur	habemini	habentur
habebar	habebaris, habebare	habebatur	habebamur	habebamini	habebantur
habebor	habeberis, habebere	habebitur	habebimur	habebimini	habebuntur
habeam	habeas	habeat	habeamus	habeatis	habeant
haberem	haberes	haberet	haberemus	haberetis	haberent
habuerim	habueris	habuerit	habuerimus	habueritis	habuerint
habuissem	habuisses	habuisset	habuissemus	habuissetis	habuissent
habear	habearis, habeare	habeatur	habeamur	habeamini	habeantur
haberer	habereris, haberere	haberetur	haberemur	haberemini	haberentur
habe	 	 	habete	 
habeto	habeto	 	habetote	habento
 	habere	 	 	habemini	 
habetor	habetor	 	 	habentor
habere	habuisse	habiturus esse	haberi  haberier	habitus esse	habitum iri
habens	 	habiturus	 	habitus	habendus
habere	habendi	habendo	habendum	habitum	habitu

facio	faciam	 	faciebam	facerem	faciam	 
facis	facias	fac 	faciebas	faceres	facies	facito 
facit	faciat	 	faciebat	faceret	faciet	facito 
facimus	faciamus	 	faciebamus	faceremus	faciemus	 
facitis	faciatis	facite 	faciebatis	faceretis	facietis	facitote 
faciunt	faciant	 	faciebant	facerent	facient	faciunto ­

fio	fiam	 	fiebam	fierem	fiam	 
fis	fias	fi	fiebas	fieres	fies	
fit fiet	fiat	 	fiebat	fieret	fiet	

fimus	fiamus	 	fiebamus	fieremus	fiemus	 
fitis	fiatis	fite	fiebatis	fieretis	fietis	fitote
fiunt	fiant	 	fiebant	fierent	fient	

feci	fecerim	feceram	fecissem	fecero
fecisti	feceris	feceras	fecisses	feceris
fecit	fecerit	fecerat	fecisset	fecerit
fecimus	fecerimus	feceramus	fecissemus	fecerimus
fecistis	feceritis	feceratis	fecissetis	feceritis
fecerunt	fecerint	fecerant	fecissent	fecerint

facere	fecisse	facturum faciens facturus
fieri	factum
factum iri	factus
faciendi	faciendus factum	factu

factum	facta
facti	factorum
facto	factis
factum	facta
facto	factis
factum	facta

ait aio

dico	dicam	 	dicebam	dicerem	dicam	 
dicis	dicas	dic 	dicebas	diceres	dices	dicito 
dicit	dicat	 	dicebat	diceret	dicet	dicito 
dicimus	dicamus	 	dicebamus	diceremus	dicemus	 
dicitis	dicatis	dicite 	dicebatis	diceretis	dicetis	dicitote 
dicunt	dicant	 	dicebant	dicerent	dicent	dicunto ­

dicor	dicar	 	dicebar	dicerer	dicar	 
diceris	dicaris	dicere 	dicebaris	dicereris	diceris	dicitor 
dicitur	dicatur	 	dicebatur	diceretur	dicetur	dicitor 
dicimur	dicamur	 	dicebamur	diceremur	dicemur	 
dicimini	dicamini	dicimini 	dicebamini	diceremini	dicemini	
dicuntur	dicantur	 	dicebantur	dicerentur	dicentur	dicuntor

dixi	dixerim	dixeram	dixissem	dixero
dixisti	dixeris	dixeras	dixisses	dixeris
dixit	dixerit	dixerat	dixisset	dixerit
diximus	dixerimus	dixeramus	dixissemus	dixerimus
dixistis	dixeritis	dixeratis	dixissetis	dixeritis
dixerunt	dixerint	dixerant	dixissent	dixerint

dicere	dixisse	dicturum dicens	dicturus
dici	dictum dicta dictum dictus
dicendi	dicendus dictum	dictu

dictus	dicta	dictum		dicti	dictae	dicta
dicti	dictae	dicti	dictorum	dictarum	dictorum
dicto	dicto	dictis
dictum	dictam	dictum	dictos	dictas	dicta
dicto	dicta	dicto	dictis
dicte	dicta	dictum	dicti	dictae	dicta

inquam	inquis	inquit	inquimus	inquitis	inquiunt
inquiebat	—	—	—
	—	inquies	inquiet	—	—	—
inquii	inquisti	inquit	—	—	—
	—	—	inquiat	—	—	—
	inque	—	—	—	—
inquito	inquito	—	—	— inquiens	

totus	tota	totum		toti	totae	tota
totius	totorum	totarum	totorum
toti	totis
totum	totam	totum	totos	totas	tota
toto	tota	toto	totis
tote	tota	totum	toti	totae	tota
tot 


#

nihil nil nichil
nilum nihilum nihilo

ūnus	ūna	ūnum		ūnī	ūnae	ūna
ūnīus	ūnōrum	ūnārum	ūnōrum
ūnī	ūnīs
ūnum	ūnam	ūnum	ūnōs	ūnās	ūna
ūnō	ūnā	ūnō	ūnīs
ūne	ūna	ūnum	ūnī	ūnae	ūna

res	res
rei	rerum
rei	rebus
rem	res
re	rebus
res	res

omnis	omne		omnes	omnia
omnis	omnium
omni	omnibus
omnem	omne	omnes	omnia
omni	omnibus
omnis	omne	omnes	omnia

nullus	nulla	nullum		nulli	nullae	nulla
nullius	nullorum	nullarum	nullorum
nulli	nullis
nullum	nullam	nullum	nullos	nullas	nulla
nullo	nulla	nullo	nullis
nulle	nulla	nullum	nulli	nullae	nulla

nūllus	nūlla	nūllum		nūllī	nūllae	nūlla
nūllīus	nūllōrum	nūllārum	nūllōrum
nūllī	nūllīs
nūllum	nūllam	nūllum	nūllōs	nūllās	nūlla
nūllō	nūllā	nūllō	nūllīs
nūlle	nūlla	nūllum	nūllī	nūllae	nūlla

nēmō
nēminis
nēminī
nēminem
nēmine
nēmō";
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
mihi mihi mi michi tibi sibi
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

hic hec haec hoc hi hii hae haec
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

aliquod

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

quicquid
que
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
                foreach (var s in Import.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(k);
                }
                foreach (var s in Conjunctions.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(k);
                }
                foreach (var s in Pronouns.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(k);
                }
                foreach (var s in Interjections.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(k);
                }
                foreach (var s in Esse.Split()) {
                    var k = Hash(s);
                    if (k != null) stops.Add(k);
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