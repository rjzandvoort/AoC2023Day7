using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Eventing.Reader;

namespace Day7
{
    internal class Program
    {
        private static Dictionary<char, long> CardValues;

        private static int CompareHands(Hand a, Hand b)
        {
            if (a.Type > b.Type)
                return 1;
            if (b.Type > a.Type)
                return -1;
            // Equal
            for (int i = 0; i < a.OHand.Length; i++)
            {                
                if (CardValues[a.OHand[i]] > CardValues[b.OHand[i]])
                    return 1;
                if (CardValues[a.OHand[i]] < CardValues[b.OHand[i]])
                    return -1;

            }
            return -1;
        }
        
        private class Hand
        {
            public long Bet;
            public string TheHand;
            public string OHand;
            public string OJHand;
            public long Value;

            public Hand(string hand, long bet, bool isJoker)
            {
                Bet = bet;
                OHand = hand;
                OJHand = hand;

                if (isJoker)
                {
                    int nrJ = hand.Count(h => h == 'J');
                    hand = hand.Replace("J", "");


                    if (hand.Length > 0)
                    {
                        hand = SortHand(hand);
                        hand = hand.PadRight(5, hand[0]);
                    }
                    else
                    {
                        hand = "AAAAA";
                    }
                }
                TheHand = SortHand(hand);
                DetermineHandType();
            }

            private void DetermineHandType()
            {
                if (TheHand.Distinct().Count() == 1)
                {
                    Type = HandType.Five;
                }
                else if (TheHand.Distinct().Count() == 2)
                {
                    // Full House of Four of a kind
                    if (TheHand.Count(c => c == TheHand[0]) == 4)
                    {
                        Type = HandType.Four;

                    }
                    else
                    {
                        Type = HandType.Full;

                    }
                }
                else if (TheHand.Distinct().Count() == 3)
                {
                    // Could be 
                    if (TheHand.Count(c => c == TheHand[0]) == 3)
                    {
                        Type = HandType.Three;
                    }
                    else
                    {
                        Type = HandType.TwoPair;
                    }
                }
                else if (TheHand.Distinct().Count() == 4)
                {
                    Type = HandType.Pair;
                }
                else
                {
                    Type = HandType.High;
                }
            }


            public override string ToString()
            {
                return TheHand + " " + OHand + " " + OJHand + " " + Type + " " + Value;
            }

            public HandType Type;

            public enum HandType
            {
                Five = 10,
                Four = 9,
                Full = 8,
                Three = 7,
                TwoPair = 6,
                Pair = 5,
                High = 4
            }

            

            public static string SortHand(string input)
            {
                return string.Concat(input
                    .GroupBy(c => c)                   
                    .OrderByDescending(g => g.Count()) 
                    .ThenByDescending(g => CardValues[g.Key])
                    .SelectMany(g => g));
            }


        }
        static void Main(string[] args)
        {
            // Values part 1
            //string cards = "AKQJT98765432";
            // Values part 2
            string cards = "AKQT98765432J";

            long maxVal = 200;
            CardValues = new Dictionary<char, long>();
            foreach(var c in cards)
            {
                CardValues.Add(c, maxVal);
                maxVal -= 10;
            }

            var txt = File.ReadAllText("input.txt");
            
            txt = txt.Replace("\r", "");
            var hands = new List<Hand>();
            foreach (var line in txt.Split('\n'))
            {
                if (String.IsNullOrEmpty(line))
                    continue;
                var lps = line.Split(' ');

                // For part two last param has to be true
                var hand = new Hand(lps[0], long.Parse(lps[1]),true );
                hands.Add(hand);                
            }
            
            var oHands = hands;
            oHands.Sort(CompareHands);
            Console.WriteLine("---- ORDERED ----");
            long cnt = 1;
            long totalVal = 0;
            foreach(var h in oHands)
            {
                Console.WriteLine(h.ToString());
                totalVal += cnt * h.Bet;
                cnt++;
            }
            Console.WriteLine(oHands.Count());
            Console.WriteLine("VALUE : " + totalVal);

            Console.ReadLine();
        }
    }
}
