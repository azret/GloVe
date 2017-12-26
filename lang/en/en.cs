using System.Collections.Generic;
using System.Text;

namespace System.Language {
    public static partial class Orthography {
        class English : IOrthography {
            ISet<string> stops;
            public English() {
                stops = Stops();
            }
            public bool IsStopWord(string hash) {
                return stops.Contains(hash);
            }
            static ISet<string> Stops() {
                string All = @"
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
would";
                ISet<string> stops = new HashSet<string>();
                foreach (var s in All.Split()) {
                    var k = s;
                    if (k != null) stops.Add(s);
                }
                return stops;
            }
            public string Hash(string s) {
                if (s == null) {
                    return null;
                }
                for (int i = 0; i < s.Length; i++) {
                    if (!char.IsLetter(s[i])) {
                        return null;
                    }
                }
                string k = s.ToLowerInvariant();
                if (IsStopWord(k)) {
                    return null;
                }
                return k;
            }
        }
    }
}