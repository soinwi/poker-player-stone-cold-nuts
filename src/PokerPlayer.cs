using Newtonsoft.Json.Linq;

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
            if (my_cards[0].Rank == my_cards[1].Rank)
            {
                return player.Stack
            }
            else
            {
                return 0;
            }
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

