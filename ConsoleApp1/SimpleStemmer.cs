using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class PorterStemmer
    {

        private static readonly Regex Vowel = new Regex("[аеиоуыэюя]", RegexOptions.Compiled);
        private static readonly Regex ReRv = new Regex("[аеиоуыэюя][^аеиоуыэюя]", RegexOptions.Compiled);
        private static readonly Regex ReStep1 = new Regex("(ась|ас|авши|ав|ал|айте|ай|аем|аете|ает|ают|ала|ало|али|ать|ая|ам|а)$", RegexOptions.Compiled);
        private static readonly Regex ReStep2 = new Regex("(ивши|ив|ивш|ившись|ывши|ыв|ывш|ывшись|ил|ила|ило|или|ий|ие|ье|ьи|ь|ую|ю)$", RegexOptions.Compiled);
        private static readonly Regex ReStep3 = new Regex("(ыся|ысь|ись|ическ|ическ|иян|иями|иях|иям|ии|ий|ью|ья|ье|ьё|ясь|ями|ях|ям|ят|ян|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яюсь|яют|ялся|ялсь|яется|яет|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется|яет|яют|яющ|ящ|ял|яла|яло|яли|яй|яется)");
        public string Stem(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }

            int rvPos = FindRv(word);
            if (rvPos == -1)
            {
                return word;
            }

            string original = word;
            word = ApplyRules(word, ReStep1, rvPos);
            if (original == word)
            {
                word = ApplyRules(word, ReStep2, rvPos);
            }

            word = ReStep3.Replace(word, "", 1);

            return word;
        }

        private int FindRv(string word)
        {
            Match rvMatch = ReRv.Match(word);
            return rvMatch.Success ? rvMatch.Index + rvMatch.Length : -1;
        }

        private string ApplyRules(string word, Regex regex, int rvPos)
        {
            MatchCollection matches = regex.Matches(word, rvPos);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    return word.Substring(0, match.Index);
                }
            }

            return word;
        }
    }
}
