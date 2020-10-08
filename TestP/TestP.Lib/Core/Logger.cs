using System;
using System.Collections.Generic;
using System.Text;

namespace TestP.Lib.Core
{
    public static class Logger
    {
        public static Action<string> GameWriter { get; set; }
        public static Action<string> DecisionWriter { get; set; }
        public static Action<string> LearningWriter { get; set; }

        public static void LogGame(string message)
        {
            GameWriter?.Invoke(message);
        }

        public static void LogDecision(string message)
        {
            DecisionWriter?.Invoke(message);
        }

        public static void LogLearning(string message)
        {
            LearningWriter?.Invoke(message);
        }
    }
}
