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
    }
}