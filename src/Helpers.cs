using System.Linq;

namespace Nancy.Simple
{
    public static class Helpers
    {
        public static int GetNumberOfRemainingPlayers(this GameState gameState)
        {
            return gameState.Players.Where(p => p.Id != gameState.Players[gameState.In_Action].Id)
                .Count(p => p.Status == PlayerStatus.active);
        }

        public static int GetMinimumRaiseBet(this GameState gameState)
        {
            return gameState.Current_Buy_In - gameState.GetOurPlayer().Bet + gameState.Minimum_Raise;
        }

        public static int GetProposedRaise(this GameState gameState)
        {
            return gameState.Current_Buy_In - gameState.GetOurPlayer().Bet;
        }

        public static bool ProposedRaiseBelow(this GameState gameState, decimal percent)
        {
            var stack = (decimal) gameState.GetOurPlayer().Stack;
            var proposedBet = gameState.GetProposedRaise();

            return proposedBet / stack <= percent;
        }

        private static Player GetOurPlayer(this GameState gameState)
        {
            return gameState.Players[gameState.In_Action];
        }

        public static int GetNumericCardValue(string rank)
        {
            if (int.TryParse(rank, out var value))
            {
                return value;
            }

            switch (rank)
            {
                case "J":
                    return 11;
                case "Q":
                    return 12;
                case "K":
                    return 13;
                case "A":
                    return 14;
                default:
                    return 0;
            }
        }

        public static bool DidBetHigherThanBigBlind(this GameState gameState)
        {
            return gameState.GetOurPlayer().Bet > gameState.Small_Blind * 2;
        }
    }
}