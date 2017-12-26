namespace System.Language {
    public interface IOrthography {
        string Hash(string s);
        bool IsStopWord(string hash);
    }

    public static partial class Orthography {
        class Generic : IOrthography {
            public Generic() {
            }
            public bool IsStopWord(string hash) {
                return false;
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
                return s;
            }
        }
        public static IOrthography Create(string lang) {
            switch (lang) {
                case "la": return new Latin();
                case "en": return new English();
                default: return new Generic();
            }
        }
    }
}