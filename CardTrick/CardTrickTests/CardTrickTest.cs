﻿using Cards;
using Cards.Manipulations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CardTrickTests
{
    [TestClass]
    public sealed class CardTrickTest
    {
        [TestMethod]
        public void Can_perform_trick()
        {
            //setup
            var deck = new Deck();
            var deckRing = new DeckRing();
            deckRing.Add(deck);

            //trick steps
            deck.Manipulate(new Cut(20));
            deck.Manipulate(new RiffleShuffle());
            var deck2 = Deck.NewEmptyDeck();

            deck.Manipulate(new Cut(26, deck2));

            Assert.AreEqual(26, deck.Count());
            Assert.AreEqual(26, deck2.Count());

            var card = deck.TakeCard(10);
            deck2.InsertCard(card, 15);

            Assert.AreEqual(25, deck.Count());
            Assert.AreEqual(27, deck2.Count());

            var trick = new Trick(deckRing);
            Assert.AreSame(card, trick.FindCard(deck2));
        }

        [TestMethod]
        public void Can_perform_trick_with_half_of_deck_not_containing_card()
        {
            //setup
            var deck = new Deck();
            var deckRing = new DeckRing();
            deckRing.Add(deck);

            //trick steps
            deck.Manipulate(new Cut(20));
            deck.Manipulate(new RiffleShuffle());
            var deck2 = Deck.NewEmptyDeck();

            deck.Manipulate(new Cut(26, deck2));

            Assert.AreEqual(26, deck.Count());
            Assert.AreEqual(26, deck2.Count());

            var card = deck.TakeCard(10);
            deck2.InsertCard(card, 15);

            Assert.AreEqual(25, deck.Count());
            Assert.AreEqual(27, deck2.Count());

            var trick = new Trick(deckRing);
            Assert.AreSame(card, trick.FindCard(deck));
        }

        [TestMethod]
        public void Can_perform_trick_using_method()
        {
            //setup
            var deck = new Deck();
            var model = new Model { ActualDeck = deck.Serialize(), DeckKnowledge = deck.Serialize() };
            var random = new Random(512);
            PerformTrick(model, random, 1);
        }

        [TestMethod]
        public void Can_perform_many_times_using_method()
        {
            for (int seed = 1; seed <= 2048; seed++)
            {
                //setup
                var deck = new Deck();
                var model = new Model { ActualDeck = deck.Serialize(), DeckKnowledge = deck.Serialize() };
                var random = new Random(seed);
                PerformTrick(model, random, seed);
            }
        }

        [TestMethod]
        [Ignore]
        public void Can_perform_trick_twice_with_learning()
        {
            for (int seed = 1; seed <= 40; seed++)
            {
                //setup
                var deck = new Deck();
                var model = new Model { ActualDeck = deck.Serialize(), DeckKnowledge = deck.Serialize() };

                var random = new Random(seed);

                for (int i = 1; i <= 2; i++)
                {
                    model = PerformTrick(model, random, seed * 100 + i);
                }
            }
        }

        [TestMethod]
        public void Can_perform_trick_twice_with_learning_using_perform()
        {
            for (int seed = 1; seed <= 512; seed++)
            {
                //setup
                var deck = new Deck();
                var model = new Model { ActualDeck = deck.Serialize(), DeckKnowledge = deck.Serialize() };

                var random = new Random(seed);

                for (int i = 1; i <= 2; i++)
                {
                    model = PerformTrickUsingPerform(model, random, seed * 100 + i);
                }
            }
        }


        [TestMethod]
        public void Can_perform_trick_20_times_with_learning_using_perform()
        {
            for (int seed = 1; seed <= 512; seed++)
            {
                //setup
                var deck = new Deck();
                var model = new Model { ActualDeck = deck.Serialize(), DeckKnowledge = deck.Serialize() };

                var random = new Random(seed);

                for (int i = 1; i <= 20; i++)
                {
                    model = PerformTrickUsingPerform(model, random, seed * 100 + i);
                }
            }
        }

        [TestMethod]
        [Ignore]
        public void Can_perform_trick_twice_without_learning()
        {
            for (int seed = 1; seed <= 40; seed++)
            {
                //setup
                var deck = new Deck();
                var model = new Model { ActualDeck = deck.Serialize(), DeckKnowledge = deck.Serialize() };

                var random = new Random(seed);

                for (int i = 1; i <= 2; i++)
                {
                    var newModel = PerformTrick(model, random, seed * 100 + i);
                    Assert.AreNotEqual(newModel.ActualDeck, model.ActualDeck);
                    model = new Model
                    {
                        ActualDeck = newModel.ActualDeck,
                        DeckKnowledge = model.DeckKnowledge
                    };
                }
            }
        }

        [TestMethod]
        public void Can_perform_trick_three_times_without_learning()
        {
            //setup
            var deck = new Deck();
            var model = new Model { ActualDeck = deck.Serialize(), DeckKnowledge = deck.Serialize() };
            var random = new Random(5981951);

            for (int i = 1; i <= 3; i++)
            {
                var newModel = PerformTrick(model, random, i);
                Assert.AreNotEqual(newModel.ActualDeck, model.ActualDeck);
                model = new Model
                {
                    ActualDeck = newModel.ActualDeck,
                    DeckKnowledge = model.DeckKnowledge
                };
            }
        }

        private class Model
        {
            public string ActualDeck { get; set; }
            public string DeckKnowledge { get; set; }
        }

        private Model PerformTrick(Model model, Random random, int run)
        {
            var trick = new Trick(model.DeckKnowledge);
            var trick2 = new Trick(model.DeckKnowledge);

            var deck = Deck.Load(model.ActualDeck);
            Assert.AreEqual(deck.Serialize(), deck.Serialize());
            //trick steps
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new RiffleShuffle());
            var newDeck = deck.Serialize();
            var deck2 = Deck.NewEmptyDeck();
            deck.Manipulate(new Cut(26, deck2));

            Assert.AreEqual(26, deck.Count());
            Assert.AreEqual(26, deck2.Count());

            var card = deck.TakeCard(random.Next(2, 24));
            deck2.InsertCard(card, random.Next(2, 24));

            Assert.AreEqual(card, trick.FindCard(deck), "Run " + run);
            Assert.AreEqual(card, trick2.FindCard(deck2), "Run " + run);

            return new Model { ActualDeck = newDeck, DeckKnowledge = deck.Serialize() + model.DeckKnowledge };
        }

        private Model PerformTrickUsingPerform(Model model, Random random, int run)
        {
            var deck = Deck.Load(model.ActualDeck);
            Assert.AreEqual(deck.Serialize(), deck.Serialize());
            //trick steps
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new Cut(random.Next(2, 50)));
            deck.Manipulate(new NonPerfectRiffleShuffle(random));
            var newDeck = deck.Serialize();
            var deck2 = Deck.NewEmptyDeck();
            deck.Manipulate(new Cut(26, deck2));

            Assert.AreEqual(26, deck.Count());
            Assert.AreEqual(26, deck2.Count());

            var card = deck.TakeCard(random.Next(2, 24));
            deck2.InsertCard(card, random.Next(2, 24));

            var deckToReturn = random.Next(0, 2) == 0 ? deck : deck2;

            var result = Trick.Perform(model.ActualDeck, deckToReturn.Serialize());

            Assert.AreEqual(card, result.Card, "Run " + run);

            return new Model { ActualDeck = newDeck, DeckKnowledge = result.NewKnowledge };
        }

        [TestMethod]
        public void RandomTest()
        {
            var rand = new Random();
            var randoms = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                randoms.Add(rand.Next(0, 2));
            }
            Assert.AreNotEqual(20, randoms.Sum());
            Assert.AreNotEqual(0, randoms.Sum());
        }
    }
}