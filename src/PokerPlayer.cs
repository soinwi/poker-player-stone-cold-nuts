using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;


namespace Nancy.Simple
{
	public static class PokerPlayer
	{
		public static readonly string VERSION = "Stone Cold Nuts";

		public static int BetRequest(JObject gameState)
		{
            //TODO: Use this method to return the value You want to bet
            var game_state = gameState.ToObject<GameState>();
            var player = game_state.Players[game_state.In_Action];

            var my_cards = player.Hole_Cards;
            List<Card> mycards = new List<Card>(player.Hole_Cards);
            List<Card> comcards = new List<Card>(game_state.Community_Cards);
            mycards.AddRange(comcards);
            // All In Stone Cold Nuts
            if (CardRecognizer.CheckFullHouse(mycards))
            {
                return GetHighBetOrCall(player.Stack , game_state, player);
            }
            else if (CardRecognizer.CheckQuads(mycards))
            {
                return GetHighBetOrCall(player.Stack / 2, game_state, player);
            }
            else if (CardRecognizer.CheckFlush(mycards))
            {
                return GetHighBetOrCall(player.Stack / 4, game_state, player);
            }
            // Bei zwei gleichen Karten in der Hand
            else if (my_cards[0].Rank == my_cards[1].Rank )
            {
                if (game_state.ProposedRaiseBelow(0.5m) && player.Bet <= 2 * game_state.Small_Blind)
                {
                    return GetHighBetOrCall(player.Stack / 16, game_state, player);
                }
                else 
                {
                    return GetCallBet(game_state, player);
                }
            }
            // Gleiche Farbe
            else if (my_cards[0].Suit == my_cards[1].Suit && game_state.GetNumberOfRemainingPlayers()<=4 && game_state.ProposedRaiseBelow(0.05m) )
            {
                return game_state.GetMinimumRaiseBet();
            }
            else
            {
                if (game_state.Current_Buy_In <= 2 * game_state.Small_Blind)
                {
                    return game_state.Current_Buy_In - player.Bet;
                }
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

