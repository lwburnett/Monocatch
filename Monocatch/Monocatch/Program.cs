using System;

namespace Monocatch
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var game = new GameMaster();
            game.Run();
        }
    }
}
