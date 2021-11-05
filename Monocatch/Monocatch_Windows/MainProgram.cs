using System;
using Monocatch;

namespace Monocatch_Windows
{
    public static class MainProgram
    {
        [STAThread]
        static void Main()
        {
            using var game = new GameMaster();
            game.Run();
        }
    }
}
