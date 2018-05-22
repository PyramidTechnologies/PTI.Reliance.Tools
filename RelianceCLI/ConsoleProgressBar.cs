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
            var perc = (decimal)complete;

            Console.CursorVisible = false;
            var left = Console.CursorLeft;
            var chars = (int)Math.Floor(perc / (1 / (decimal)_mBarSize));
            var p1 = string.Empty;
            var p2 = string.Empty;

            for (var i = 0; i < chars; i++)
            {
                p1 += _mFillSymbol;
            }

            for (var i = 0; i < _mBarSize - chars; i++)
            {
                p2 += _mFillSymbol;
            }

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
