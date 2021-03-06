using System.Collections.Generic;
using System.Diagnostics;

namespace Cards
{
    public sealed class DeckRing
    {
        private readonly List<Card> _cards = new List<Card>();

        public void Add(Deck deck)
        {
            _cards.AddRange(deck._cards);
        }

        public TrickPointer FindCard(Card card)
        {
            var idx = _cards.IndexOf(card);
            return new TrickPointer(this, idx);
        }

        public bool NextIs(int idx, Card card)
        {
            return Equals(Next(idx), card);
        }

        public Card Next(int idx)
        {
            return _cards[(idx + 1 + _cards.Count) % _cards.Count];
        }

        internal Card OfIndex(int _idx)
        {
            return Next(_idx - 1);
        }
    }
}