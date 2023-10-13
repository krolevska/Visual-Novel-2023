using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dialoque
{
    public class DL_SpeakerData
    {
        public string name, castName;

        public string displayName => castName != string.Empty ? castName : name;
        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpressions { get; set; }

        private const string nameCastID = " as ";
        private const string positionCastID = " at ";
        private const string expressionCastID = " [";
        private const char axisDelimeter = ':';
        private const char expressionLayerJoiner = ',';
        private const char expressionLayerDelimeter = ':';

        public DL_SpeakerData(string rawSpeaker)
        {
            string pattern = @$"{nameCastID}|{positionCastID}|{expressionCastID.Insert(expressionCastID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);
            // Populate this data to avoid null refs 
            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();

            // if no matches than its a name
            if (matches.Count == 0)
            {
                name = rawSpeaker;

                return;
            }
            // otherwise isolate speaker name from casting data
            else
            {
                int index = matches[0].Index;
                name = rawSpeaker.Substring(0, index);
            }

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;
                if (match.Value == nameCastID)
                {
                    startIndex = match.Index + nameCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                else if (match.Value == positionCastID)
                {
                    startIndex = match.Index + positionCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(axisDelimeter, System.StringSplitOptions.RemoveEmptyEntries);

                    float.TryParse(axis[0], out castPosition.x);
                    if (axis.Length > 1)
                        float.TryParse(axis[1], out castPosition.y);
                }

                else if (match.Value == expressionCastID)
                {
                    startIndex = match.Index + expressionCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    CastExpressions = castExp.Split(expressionLayerJoiner)
                        .Select(x =>
                        {
                            var parts = x.Trim().Split(expressionLayerDelimeter);
                            return (int.Parse(parts[0]), parts[1]);
                        }).ToList();
                }
            }
        }
    }
}