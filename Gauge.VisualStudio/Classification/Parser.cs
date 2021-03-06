﻿// Copyright [2014, 2015] [ThoughtWorks Inc.](www.thoughtworks.com)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;

namespace Gauge.VisualStudio.Classification
{
    static class Parser
    {
        internal const char DummyChar = '~';

        private static readonly Regex ScenarioHeadingRegex = new Regex(@"(\#\#.*)$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex ScenarioHeadingRegexAlt = new Regex(@".+[\n\r]-+", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex SpecHeadingRegex = new Regex(@"(\#.*)$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex SpecHeadingRegexAlt = new Regex(@".+[\n\r]=+", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static readonly Regex StepRegex = new Regex(@"[ ]*\*([\w ]*(?<stat>""[\w ]+"")*(?<dyn>\<[\w ]+\>)*)*", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex TagsRegex = new Regex(@"\s*tags\s*:\s*(?<tag>[\w\s]+)(,(?<tag>[\w\s]+))*", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static List<Token> ParseMarkdownParagraph(string text, int offset = 0)
        {
            var tokens = new List<Token>();
            if (text.Trim().Length == 0)
                return tokens;

            tokens.AddRange(ParseSpecs(text));
            tokens.AddRange(ParseScenarios(text));
            tokens.AddRange(ParseSteps(text));
            tokens.AddRange(ParseTags(text));
            return tokens;
        }

        public static string GetSpecificationName(string text)
        {
            var match = SpecHeadingRegex.Match(text);
            if (match.Success)
                return match.Value;
            match = SpecHeadingRegexAlt.Match(text);
            return match.Success ? match.Value : string.Empty;
        }

        public static IEnumerable<string> GetScenarios(string text)
        {
            var matches = ScenarioHeadingRegex.Matches(text);
            foreach (Match match in matches)
            {
                yield return match.Value;
            }
            matches = ScenarioHeadingRegexAlt.Matches(text);
            foreach (Match match in matches)
            {
                yield return match.Value;
            }
        }

        public static bool ParagraphContainsMultilineTokens(string text)
        {
            return SpecHeadingRegexAlt.IsMatch(text) || ScenarioHeadingRegexAlt.IsMatch(text);
        }

        public enum TokenType
        {
            Comment,
            Specification, 
            Scenario,
            Step,
            Tag,
            TagValue,
            StaticParameter,
            DynamicParameter
        }

        public struct Token
        {
            public Token(TokenType type, Span span) { TokenType = type; Span = span; }

            public TokenType TokenType;
            public Span Span;
        }

        private static IEnumerable<Token> ParseScenarios(string text)
        {
            // Multiple ifs? Somehow I feel this is more explicit than having complex regex.

            var matches = ScenarioHeadingRegex.Matches(text);
            foreach (Match match in matches)
            {
                yield return new Token(TokenType.Scenario, new Span(match.Index, match.Length));
            }

            matches = ScenarioHeadingRegexAlt.Matches(text);
            foreach (Match match in matches)
            {
                yield return new Token(TokenType.Scenario, new Span(match.Index, match.Length));
            }
        }

        private static IEnumerable<Token> ParseSpecs(string text)
        {
            var matches = SpecHeadingRegex.Matches(text);
            foreach (Match match in matches)
            {
                yield return new Token(TokenType.Specification, new Span(match.Index, match.Length));
            }
            matches = SpecHeadingRegexAlt.Matches(text);
            foreach (Match match in matches)
            {
                yield return new Token(TokenType.Specification, new Span(match.Index, match.Length));
            }
        }

        private static IEnumerable<Token> ParseSteps(string text)
        {
            var matches = StepRegex.Matches(text);
            foreach (Match match in matches)
            {
                yield return new Token(TokenType.Step, new Span(match.Index, match.Length));
                foreach (Capture capture in match.Groups["stat"].Captures)
                {
                    yield return new Token(TokenType.StaticParameter, new Span(capture.Index, capture.Length));    
                }
                foreach (Capture capture in match.Groups["dyn"].Captures)
                {
                    yield return new Token(TokenType.DynamicParameter, new Span(capture.Index, capture.Length));    
                }
            }
        }

        private static IEnumerable<Token> ParseTags(string text)
        {
            var matches = TagsRegex.Matches(text);
            foreach (Match match in matches)
            {
                yield return new Token(TokenType.Tag, new Span(match.Index, match.Length));
                foreach (Capture capture in match.Groups["tag"].Captures)
                {
                    yield return new Token(TokenType.TagValue, new Span(capture.Index, capture.Length));                    
                }
            }
        }
    }
}
