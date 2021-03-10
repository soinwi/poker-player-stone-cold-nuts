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
            return gameState.Current_Buy_In - gameState.Players[gameState.In_Action].Bet + gameState.Minimum_Raise;
        }
    }
}