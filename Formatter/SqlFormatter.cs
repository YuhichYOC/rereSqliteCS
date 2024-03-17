/*
*
* SqlFormatter.cs
*
* Copyright 2022 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Text.RegularExpressions;

namespace Formatter {

    public class SqlFormatter {

        private class Element {
            internal int IndentLevel { private set; get; } = 0;

            internal int IndentLevelAfterMe { private set; get; } = 0;

            internal string Self { private set; get; } = string.Empty;

            internal string Cut(string sql, int indentLevel) {
                IndentLevel = indentLevel;
                sql = sql.Replace(@"\n", @" ");
                sql = sql.Trim();
                sql = CutKeywords(sql, indentLevel);
                sql = CutBrackets(sql, indentLevel);
                sql = CutOthers(sql, indentLevel);
                return sql;
            }

            private string CutKeywords(string sql, int indentLevel) {
                if (!string.IsNullOrEmpty(Self)) return sql;

                IndentLevel = indentLevel == 0 ? 0 : indentLevel - 1;
                IndentLevelAfterMe = indentLevel == 0 ? indentLevel + 1 : indentLevel;

                Match? m;
                m = Regex.Match(sql, @"^ *SELECT", RegexOptions.IgnoreCase);
                if (m.Success) {
                    Self = $"{Indent()}SELECT";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^ *FROM", RegexOptions.IgnoreCase);
                if (m.Success) {
                    Self = $"{Indent()}FROM";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^ *WHERE", RegexOptions.IgnoreCase);
                if (m.Success) {
                    Self = $"{Indent()}WHERE";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^ *GROUP +BY", RegexOptions.IgnoreCase);
                if (m.Success) {
                    Self = $"{Indent()}GROUP BY";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^ *ORDER +BY", RegexOptions.IgnoreCase);
                if (m.Success) {
                    Self = $"{Indent()}ORDER BY";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^ *HAVING", RegexOptions.IgnoreCase);
                if (m.Success) {
                    Self = $"{Indent()}HAVING";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^ *PARTITION +BY", RegexOptions.IgnoreCase);
                if (m.Success) {
                    Self = $"{Indent()}PARTITION BY";
                    return sql[m.Length..];
                }

                return sql;
            }

            private string CutBrackets(string sql, int indentLevel) {
                if (!string.IsNullOrEmpty(Self)) return sql;

                Match? m;
                m = Regex.Match(sql, @"^\(");
                if (m.Success) {
                    IndentLevel = indentLevel;
                    IndentLevelAfterMe = indentLevel + 1;
                    Self = $"{Indent()}(";
                    return sql[1..];
                }
                m = Regex.Match(sql, @"^\) +AS +(\w+)");
                if (m.Success) {
                    IndentLevel = indentLevel - 1;
                    IndentLevelAfterMe = indentLevel - 1;
                    Self = $"{Indent()}) AS {m.Groups[1].Value}";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^\)");
                if (m.Success) {
                    IndentLevel = indentLevel - 1;
                    IndentLevelAfterMe = indentLevel - 1;
                    Self = $"{Indent()})";
                    return sql[1..];
                }

                return sql;
            }

            private string CutOthers(string sql, int indent) {
                if (!string.IsNullOrEmpty(Self)) return sql;

                IndentLevel = indent;
                IndentLevelAfterMe = indent;

                Match? m;
                m = Regex.Match(sql, @"^,(\S+) *(=|<>|>=|<=) *(\S+)(AND|OR|,|ORDER|GROUP|WHERE|\()?");
                if (m.Success) {
                    Self = $"{Indent(2)}, {m.Groups[1].Value.Trim()} {m.Groups[2].Value} {m.Groups[3].Value.Trim()}";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^AND(\S+) *(=|<>|>=|<=) *(\S+)(AND|OR|,|ORDER|GROUP|WHERE|\()?");
                if (m.Success) {
                    Self = $"{Indent(4)}AND {m.Groups[1].Value.Trim()} {m.Groups[2].Value} {m.Groups[3].Value.Trim()}";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^OR(\S+) *(=|<>|>=|<=) *(\S+)(AND|OR|,|ORDER|GROUP|WHERE|\()?");
                if (m.Success) {
                    Self = $"{Indent(3)}OR {m.Groups[1].Value.Trim()} {m.Groups[2].Value} {m.Groups[3].Value.Trim()}";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^(\S+) *(=|<>|>=|<=) *(\S+)(AND|OR|,|ORDER|GROUP|WHERE|\()?");
                if (m.Success) {
                    Self = $"{Indent()}{m.Groups[1].Value.Trim()} {m.Groups[2].Value} {m.Groups[3].Value.Trim()}";
                    return sql[m.Length..];
                }

                m = Regex.Match(sql, @"^, *(\S+) +AS +(\w+)");
                if (m.Success) {
                    Self = $"{Indent(2)}, {m.Groups[1].Value} AS {m.Groups[2].Value}";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^(\S+) + AS +(\w+)");
                if (m.Success) {
                    Self = $"{Indent()}{m.Groups[1].Value} AS {m.Groups[2].Value}";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^, *(\w+|\*)");
                if (m.Success) {
                    Self = $"{Indent(2)}, {m.Groups[1].Value}";
                    return sql[m.Length..];
                }
                m = Regex.Match(sql, @"^(\w+|\*)");
                if (m.Success) {
                    Self = $"{Indent()}{m.Groups[1].Value}";
                    return sql[m.Length..];
                }

                if (sql.StartsWith(@";")) {
                    Self = @";";
                    return sql[1..];
                }

                return sql;
            }

            private string Indent(int decr = 0) => string.Empty.PadLeft(IndentLevel * 4 - decr, ' ');
        }

        private List<Element> elements = new List<Element>();

        public SqlFormatter(string sql) {
            int indent = 0;
            while (!string.IsNullOrEmpty(sql)) {
                var e = new Element();
                sql = e.Cut(sql, indent);
                indent = e.IndentLevelAfterMe;
                elements.Add(e);
            }
        }

        public override string ToString() => elements.Aggregate(string.Empty, (string seed, Element e) => seed += e.Self + "\n");
    }
}