using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "Stone Cold Nuts";

		public static int BetRequest(JObject gameState)
		{
            try
            {
                
                //TODO: Use this method to return the value You want to bet
                var game_state = gameState.ToObject<GameState>();
                var player = game_state.Players[game_state.In_Action];

                var my_cards = player.Hole_Cards;
                List<Card> allcards = new List<Card>(player.Hole_Cards);
                List<Card> comcards = new List<Card>(game_state.Community_Cards);
                allcards.AddRange(comcards);

                bool isHeadsUp = game_state.GetNumberOfRemainingPlayers() == 1;

                // All In: FullHouse
                if (CardRecognizer.CheckFullHouse(allcards) && CardRecognizer.CheckFullHouse(comcards) == false)
                {
                    return GetHighBetOrCall(player.Stack, game_state, player);
                }
                // Poker 
                else if (CardRecognizer.CheckQuads(allcards) && CardRecognizer.CheckQuads(comcards) == false)
                {
                    return GetHighBetOrCall(player.Stack / 2, game_state, player);
                }
                // Flush
                else if (CardRecognizer.CheckFlush(allcards) && CardRecognizer.CheckFlush(comcards) == false)
                {
                    return GetHighBetOrCall(player.Stack / 4, game_state, player);
                }
                //Raus, wenn bereits 4 gleiche Farben auf dem Tisch liegen
                else if (CardRecognizer.CheckFlushWithFour(comcards))
                {
                    return 0;
                }
                // Mitgehen, wenn schon mehr des halben Stacks gesetzt
                else if (player.Bet > player.Stack / 2)
                {

                    if (isHeadsUp && game_state.Bet_Index < 5)
                    {
                        return game_state.GetMinimumRaiseBet();
                    }
                    else
                    {
                        return GetCallBet(game_state, player);
                    }
                }
                else if (CardRecognizer.CheckTrips(allcards) && CardRecognizer.CheckTrips(comcards) == false)
                {
                    return GetHighBetOrCall(player.Stack / 8, game_state, player);
                }
                else if (CardRecognizer.CheckTwoPair(allcards) && CardRecognizer.CheckTwoPair(comcards) == false)
                {
                    return GetHighBetOrCall(player.Stack / 8, game_state, player);
                }
                //Raus, wenn bereits 3 gleiche Farben auf dem Tisch liegen
                else if (CardRecognizer.CheckFlushWithTree(comcards) && comcards.Count == 5)
                {
                    return 0;
                }
                // Bei zwei gleichen Karten in der Hand
                else if (my_cards[0].Rank == my_cards[1].Rank)
                {
                    // Nur beim ersten Mal erhöhen
                    if (game_state.ProposedRaiseBelow(0.5m) && player.Bet <= 2 * game_state.Small_Blind)
                    {
                        return GetHighBetOrCall(player.Stack / 16, game_state, player);
                    }
                    else if (CardRecognizer.CheckPairHeigh(allcards) >= 10)
                    {
                        return GetCallBet(game_state, player);
                    }
                    else
                    {
                        return 0;
                    }
                }
                //Pairly
                else if (CardRecognizer.CheckPair(allcards) && CardRecognizer.CheckPair(comcards) == false)
                {
                    //Hat es in den Comm-Cards noch was höheres
                    bool comHasHigher = Helpers.AnyHigherThan(game_state.Community_Cards, CardRecognizer.CheckPairHeigh(allcards));

                    // Hohes Paar, grösser Dame
                    if (CardRecognizer.CheckPairHeigh(allcards) >= 12)
                    {
                        if (game_state.Bet_Index < 5)
                        {
                            return game_state.GetMinimumRaiseBet();
                        }
                        else
                        {
                            return GetCallBet(game_state, player);
                        }
                    }
                    else
                    {

                        if (isHeadsUp && game_state.Bet_Index < 5)
                        {
                            return game_state.GetMinimumRaiseBet();
                        }
                        else if (comHasHigher)
                        {
                            return 0;
                        }
                        else
                        {
                            return GetCallBet(game_state, player);
                        }
                    }
                }
                // Wenn Karten höher als 9
                else if (my_cards.All(c => Helpers.GetNumericCardValue(c.Rank) >= 10))
                {
                    if (isHeadsUp && game_state.Bet_Index < 5)
                    {
                        return GetHighBetOrCall(player.Stack / 8, game_state, player);
                    }
                    else if (GetCallBet(game_state, player) <= player.Stack / 8)
                    {
                        return GetCallBet(game_state, player);
                    }
                    else
                    {
                        return 0;
                    }
                }
                // Gleiche Farbe
                else if (my_cards[0].Suit == my_cards[1].Suit && game_state.GetNumberOfRemainingPlayers() <= 4 && game_state.ProposedRaiseBelow(0.05m))
                {
                    if (game_state.Bet_Index < 5)
                    {
                        return game_state.GetMinimumRaiseBet();
                    }
                    else
                    {
                        return GetCallBet(game_state, player);
                    }
                }
                else
                {   //Mitgehen, wenn nur der BIG drin ist
                    if (game_state.Current_Buy_In <= 2 * game_state.Small_Blind || game_state.Current_Buy_In < player.Stack /50)
                    {
                        return GetCallBet(game_state, player);
                    }
                    //raus
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Console.Write(VERSION + ' ' + ex.Message);
                return 0;
            }
		}

        private static int GetCallBet(GameState game_state, Player player)
        {
            return game_state.Current_Buy_In - player.Bet;
        }

        private static int GetHighBetOrCall(int basedOnStack, GameState game_state, Player player)
        {
            var callBet = GetCallBet(game_state, player);
            var bet = new[]{callBet, basedOnStack}.Max();

            return bet;
        }

        public static void ShowDown(JObject gameState)
		{
			//TODO: Use this method to showdown
		}
	}
    public class GameState
    {
        public Player[] Players;
        public string Tournament_Id;
        public int Round;
        public int Small_Blind;
        public int Current_Buy_In;
        public int Pot;
        public int Minimum_Raise;
        public int Bet_Index;
        public int Orbits;
        public int Dealer;
        public Card[] Community_Cards;
        public int In_Action;
    }

    public class Player
    {
        public int Id;
        public string Name;
        public int Stack;
        public PlayerStatus Status;
        public string Version;
        public int Bet;
        public Card[] Hole_Cards;
    }

    public enum PlayerStatus
    {
        active,
        folded,
        @out
    }

    public class Card
    {
        public string Rank;
        public string Suit;
    }
}

