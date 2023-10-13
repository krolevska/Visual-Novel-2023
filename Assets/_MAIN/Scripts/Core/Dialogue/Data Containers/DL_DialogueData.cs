using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using UnityEngine;
using System;
namespace Dialoque
{
    public class DL_DialogueData
    {
        public List<Dialogue_Segment> segments;
        private const string segmentIdentifierPattern = @"\{[CAca]\}|\{[Ww][CAca]\s\d*\.?\d*\}";
        public DL_DialogueData(string rawDialogue)
        {
            segments = RipSegments(rawDialogue);
        }

        public List<Dialogue_Segment> RipSegments(string rawDialog)
        {
            List<Dialogue_Segment> segments = new List<Dialogue_Segment>();
            MatchCollection matches = Regex.Matches(rawDialog, segmentIdentifierPattern);

            int lastIndex = 0;
            // Find the first or only segment in the file

            Dialogue_Segment segment = new Dialogue_Segment();
            segment.dialogue = (matches.Count == 0 ? rawDialog : rawDialog.Substring(0, matches[0].Index));
            segment.startSignal = Dialogue_Segment.StartSignal.NONE;
            segment.signalDelay = 0;
            segments.Add(segment);

            if (matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new Dialogue_Segment();
                // Get the start signal for the segment
                string signalMatch = match.Value; //{A}
                signalMatch = signalMatch.Substring(1, match.Length - 2); //A
                string[] signalSplit = signalMatch.Split(' ');

                segment.startSignal = (Dialogue_Segment.StartSignal)Enum.Parse(typeof(Dialogue_Segment.StartSignal), signalSplit[0].ToUpper());

                // Get the signal delay
                if (signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], out segment.signalDelay);

                // Get dialogue for the segment
                int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialog.Length;

                segment.dialogue = rawDialog.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;

                segments.Add(segment);
            }
            return segments;
        }
        public struct Dialogue_Segment
        {
            public string dialogue;
            public StartSignal startSignal;
            public float signalDelay;
            public enum StartSignal { NONE, C, A, WA, WC}

            public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
        }
    }
}