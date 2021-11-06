using System;
using Monocatch_Lib;

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
