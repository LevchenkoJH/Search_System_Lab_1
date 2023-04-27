using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Stemmer
    {
        private static string vowel = @"[аеиоуыэюя]";
        private static string non_vowel = @"[^аеиоуыэюя]";

        Regex reRv = new Regex(vowel);
        Regex reR1 = new Regex(vowel + non_vowel);
        Regex re_perflective_gerund = new Regex(@"(((P<ignore>[ая])(в|вши|вшись))|(ив|ивши|ившись|ыв|ывши|ывшись))$");
        Regex re_adjective = new Regex(@"(ее|ие|ые|ое|ими|ыми|ей|ий|ый|ой|ем|им|ым|ом|его|ого|ему|ому|их|ых|ую|юю|ая|яя|ою|ею)$");
        Regex re_participle = new Regex(@"(((?<ignore>[ая])(ем|нн|вш|ющ|щ))|(ивш|ывш|ующ))$");
        Regex re_reflexive = new Regex(@"(ся|сь)$");
        Regex re_verb = new Regex(@"(((P<ignore>[ая])(ла|на|ете|йте|ли|й|л|ем|н|ло|но|ет|ют|ны|ть|ешь|нно))|(ила|ыла|ена|ейте|уйте|ите|или|ыли|ей|уй|ил|ыл|им|ым|ен|ило|ыло|ено|ят|ует|уют|ит|ыт|ены|ить|ыть|ишь|ую|ю))$");
        Regex re_noun = new Regex(@"(а|ев|ов|ие|ье|е|иями|ями|ами|еи|ии|и|ией|ей|ой|ий|й|иям|ям|ием|ем|ам|ик|ом|о|у|ах|иях|ях|ы|ь|ию|ью|ю|ия|ья|я)$");
        Regex re_superlative = new Regex(@"(ейш|ейше)$");
        Regex re_derivational = new Regex(@"(ост|ость)$");
        Regex re_i = new Regex(@"и$");
        Regex re_nn = new Regex(@"((?<=н)н)$");
        Regex re = new Regex(@"ь$");


        private int FindRv(string word)
        {
            //Console.WriteLine("Зашли", word);
            MatchCollection rvMatch = reRv.Matches(word);
            if (rvMatch is null)
            {
                return word.Length;
            }
            return rvMatch.Last().Index;
        }

        private int FindR2(string word)
        {
            MatchCollection r1Match = this.reR1.Matches(word);

            if (r1Match is null)
            {
                return word.Length;
            }

            MatchCollection r2Match = this.reR1.Matches(word, r1Match.Last().Index);


            if (r2Match is null)
            {
                return word.Length;
            }

            return r2Match.Last().Index;
        }

        private Tuple<bool, string> Cut(string word, Regex ending, int pos)
        {
            Match match = ending.Match(word, pos);
            if (match.Success)
            {
                return Tuple.Create(true, word.Substring(0, match.Index));
            }
            else
            {
                return Tuple.Create(false, word);
            }
        }

        private string Step1(string word, int rv_pos)
        {
            Tuple<bool, string> dummy = this.Cut(word, re_perflective_gerund, rv_pos);
            if (dummy.Item1 is true)
            {
                return dummy.Item2;
            }

            dummy = this.Cut(dummy.Item2, re_reflexive, rv_pos);
            dummy = this.Cut(dummy.Item2, re_adjective, rv_pos);

            if (dummy.Item1 is true)
            {
                dummy = this.Cut(dummy.Item2, re_participle, rv_pos);
                return dummy.Item2;
            }

            dummy = this.Cut(dummy.Item2, re_verb, rv_pos);
            if (dummy.Item1 is true)
            {
                return dummy.Item2;
            }

            dummy = this.Cut(dummy.Item2, re_noun, rv_pos);
            return dummy.Item2;
        }

        private string Step2(string word, int rv_pos)
        {
            Tuple<bool, string> dummy = this.Cut(word, re_i, rv_pos);
            return dummy.Item2;
        }

        private string Step3(string word, int r2_pos)
        {
            Tuple<bool, string> dummy = this.Cut(word, re_derivational, r2_pos);
            return dummy.Item2;
        }

        private string Step4(string word, int rv_pos)
        {
            Tuple<bool, string> dummy = this.Cut(word, re_superlative, rv_pos);
            dummy = this.Cut(dummy.Item2, re_nn, rv_pos);
            if (!dummy.Item1)
            {
                dummy = this.Cut(dummy.Item2, re, rv_pos);
            }
            return dummy.Item2;
        }

        public string Stem(string word)
        {
            if (word.Length == 1 || word.Length == 2 || word == " " || word == "")
            {
                return word;
            }

            int rv_pos = this.FindRv(word);
            int r2_pos = this.FindR2(word);

            string speech = this.Step1(word, rv_pos);
            speech = this.Step2(speech, rv_pos);
            speech = this.Step3(speech, r2_pos);
            speech = this.Step4(speech, rv_pos);

            return speech;
        }
    }
}
