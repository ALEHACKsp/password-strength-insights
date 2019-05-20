using System;
using System.Collections.Generic;

namespace PsiUserInterface
{
    public class PasswordStrength
    {
        class PermutationSet
        {
            public bool Resolved { get; set; }
            public Func<char, bool> Resolver { get; }
            public int PermutationCount { get; }

            public PermutationSet(Func<char, bool> resolver, int permutations)
            {
                this.Resolved = false;
                this.Resolver = resolver;
                this.PermutationCount = permutations;
            }
        };

        private static List<string> WordList = new List<string>()
        {
            "board", "boar", "boa", "bo",
        };
                
        public static float EvaluatePassword(string password)
        {
            if (password == null)
                return 0.0f;
            else
            {
                float word_entropy = EvaluateWordEntropy(ref password);
                float char_entropy = EvaluateCharEntropy(password);

                return word_entropy + char_entropy;
            }
        }

        private static float EvaluateWordEntropy(ref string password)
        {
            List<Tuple<string, int>> matches = FindWords(password.ToLower(), 0);

            if (matches == null)
                return 0.0f;
            else
            {
                int delta = 0;

                foreach (var match in matches)
                {
                    password = password.Remove(match.Item2 - delta, match.Item1.Length);
                    delta += match.Item1.Length;
                }

                return (float)(matches.Count * Math.Log(Convert.ToDouble(WordList.Count), 2.0));
            }
        }

        private static float EvaluateCharEntropy(string password)
        {
            if (password.Length == 0)
                return 0.0f;
            else
            { 
                var sets = new List<PermutationSet>()
                {
                    new PermutationSet(IsUpper, 26),
                    new PermutationSet(IsLower, 26),
                    new PermutationSet(IsDigit, 10),
                    new PermutationSet(IsSymbol, 33)
                };

                int permutations = 0;

                foreach (char c in password)
                {
                    foreach (var set in sets)
                    {
                        if (!set.Resolved && set.Resolver(c))
                        {
                            permutations += set.PermutationCount;
                            set.Resolved = true;
                        }
                    }
                }

                return (float)(password.Length * Math.Log(Convert.ToDouble(permutations), 2.0));
            }
        }

        private static List<Tuple<string, int>> FindWords(string s, int start)
        {
            if (s.Length > 1)
            {
                foreach (string w in WordList)
                {
                    int index = s.IndexOf(w);

                    if (index != -1)
                    {
                        var left = FindWords(s.Substring(0, index), 0);
                        var right = FindWords(s.Substring(index + w.Length), index + w.Length);

                        var words = new List<Tuple<string, int>>() { new Tuple<string, int>(w, start + index) };

                        if (left != null)
                            words.AddRange(left);

                        if (right != null)
                            words.AddRange(right);

                        return words;
                    }
                }
            }

            return null;
        }

        private static bool IsLower(char c)
        {
            return (c >= 'a' && c <= 'z');
        }

        private static bool IsUpper(char c)
        {
            return (c >= 'A' && c <= 'Z');
        }

        private static bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        private static bool IsSymbol(char c)
        {
            return ((c >= 0x20 && c <= 0x2f) || (c >= 0x3a && c <= 0x40) || (c >= 0x5b && c <= 0x60) || (c >= 0x7b && c <= 0x7e));
        }
    }
}
