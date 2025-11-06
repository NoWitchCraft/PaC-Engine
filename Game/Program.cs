using System;
using Engine;

namespace Game
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var app = new GameApp();
            var host = new EngineHost(app, targetFps: 60);

            Console.WriteLine("Press Esc to quit");
            host.Run(shouldExit: () =>
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Escape) return true;
                }
                return false;
            });
        }
    }
}