namespace Day07
{
    internal static class CamelCardsGame
    {
        public enum HandKind
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            FullHouse,
            FourOfAKind,
            FiveOfAKind
        }

        public enum CardType
        {
            Joker,
            C2,
            C3,
            C4,
            C5,
            C6,
            C7,
            C8,
            C9,
            Ten,
            Jack,
            Queen,
            King,
            Ace
        }

        private static CardType CharToCardType(char ch, bool useJoker)
        {
            switch (ch)
            {
                case '2': return CardType.C2;
                case '3': return CardType.C3;
                case '4': return CardType.C4;
                case '5': return CardType.C5;
                case '6': return CardType.C6;
                case '7': return CardType.C7;
                case '8': return CardType.C8;
                case '9': return CardType.C9;
                case 'T': return CardType.Ten;
                case 'J': return useJoker ? CardType.Joker : CardType.Jack;
                case 'Q': return CardType.Queen;
                case 'K': return CardType.King;
                case 'A': return CardType.Ace;
            }

            throw new Exception("Unknown card " + ch);
        }
        public class Hand
        {
            public Hand(string text, bool useJoker)
            {
                Cards = text.Take(5).Select(ch => CharToCardType(ch, useJoker)).ToArray();
                Bid = int.Parse(text.Substring(6));
                Kind = GetHandKind(Cards);
            }

            private HandKind GetHandKind(CardType[] cards)
            {
                int jokersCount = cards.Count(c => c == CardType.Joker);
                var groups = cards.Where(c => c != CardType.Joker)
                    .GroupBy(c => c)
                    .Select(g => g.Count())
                    .OrderDescending().ToList();

                groups.Add(0);
                groups[0] += jokersCount;

                HandKind kind = groups switch
                {
                    [5, ..] => HandKind.FiveOfAKind,
                    [4, ..] => HandKind.FourOfAKind,
                    [3, 2, ..] => HandKind.FullHouse,
                    [3, 1, ..] => HandKind.ThreeOfAKind,
                    [2, 2, ..] => HandKind.TwoPair,
                    [2, 1, ..] => HandKind.OnePair,
                    [1, ..] => HandKind.HighCard,
                    _ => throw new Exception("Unknown HandKind " + cards)
                };

                return kind;
            }

            public HandKind Kind { get; private set; }
            public CardType[] Cards { get; private set; }
            public int Bid { get; private set; }
        }

        public class HandComparer() : IComparer<Hand>
        {
            int IComparer<Hand>.Compare(Hand? x, Hand? y)
            {
                int result = x!.Kind.CompareTo(y!.Kind);

                int cardIndex = 0;

                while(result == 0 && cardIndex < x.Cards.Length)
                {
                    result = x.Cards[cardIndex].CompareTo(y.Cards[cardIndex]);
                    cardIndex++;
                }

                return result;
            }
        }
    }
}
