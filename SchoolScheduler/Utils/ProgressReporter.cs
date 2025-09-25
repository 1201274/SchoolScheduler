using System;
using System.Diagnostics;

namespace SchoolScheduler.Utils
{
    public class ProgressReporter
    {
        private readonly int _total;
        private int _lastPercent = -1;
        private readonly Stopwatch _stopwatch;

        public ProgressReporter(int total)
        {
            _total = total;
            _stopwatch = Stopwatch.StartNew();
            Console.CursorVisible = false;
        }

        public void Report(int current, string message = "")
        {
            
            int percent = (int)((current + 1) * 100.0 / _total);
            if (percent > _lastPercent)
            {
                _lastPercent = percent;
                double elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;
                double estimatedTotal = elapsedSeconds / (percent / 100.0);
                double remaining = estimatedTotal - elapsedSeconds;

                
                int minutes = Math.Clamp((int)(remaining / 60), 0, int.MaxValue);
                int seconds = Math.Clamp((int)Math.Round(remaining % 60), 0, 59);

                string etaDisplay = $"{minutes:D2}:{seconds:D2}";

                int barLength = 50;
                int progress = (int)((percent / 100.0) * barLength);
                Console.Write("Progress: [");
                Console.Write(new string('#', progress).PadRight(barLength));
                Console.Write($"] {percent}% | ETA: {etaDisplay} | {message}".PadRight(Console.WindowWidth - barLength - 20) + "\r");
            }
        }
    }
}