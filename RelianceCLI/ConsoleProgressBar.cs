using System;
using PTIRelianceLib;

namespace RelianceCLI
{
    internal class ConsoleProgressBar : ProgressMonitor
    {
        private readonly char _mFillSymbol;
        private readonly int _mBarSize;

        public ConsoleProgressBar(char fillSymbol = '=')
        {
            _mFillSymbol = fillSymbol;
            _mBarSize = Console.WindowWidth - 10;
            OnFlashMessage += (s, o) => Console.WriteLine(o.Message);
            OnFlashProgressUpdated += (s, o) => ReportProgress(o.Progress);
        }

        public override void ReportProgress(double progress)
        {
            DrawProgressBar(progress);
        }

        private void DrawProgressBar(double complete)
        {
            // Clamp to (0,1) range
            complete = Math.Max(Math.Min(complete, 1.0), 0.0);
            var perc = (decimal)complete;

            Console.CursorVisible = false;
            var left = Console.CursorLeft;
            var chars = (int)Math.Floor(perc / (1 / (decimal)_mBarSize));

            var p1 = new string(_mFillSymbol, chars);
            var p2 = new string(_mFillSymbol, _mBarSize - chars);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(p1);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(p2);

            Console.ResetColor();
            Console.Write(" {0:N2}%", perc * 100);
            Console.CursorLeft = left;
        }
    }
}
