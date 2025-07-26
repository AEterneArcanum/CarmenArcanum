using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtEnumDef(ExprIdentifier EnumID, string[] Enumerations) : Statement;

    public class StmtEnumDefParsers : StatementParser
    {
        public StmtEnumDefParsers() : base (StatementPriorities.Definitions) { }
        public StmtEnumDefParsers(int priority = StatementPriorities.Definitions) : base(priority) { }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordDefineEnum || tokens[2].Type != TokenType.KeywordWith) return false;
            // Get ID from token 1
            if (tokens[1].Type != TokenType.StructIdentifier) return false;
            ExprIdentifier iden = new ExprIdentifier(tokens[1].Raw, IdentifierType.Structure);
            // Get enumeration list
            var lst = tokens[4..];
            // Get comma indices from list
            var cma = new List<int>();
            for (int i = 0; i < lst.Length; i++)
            {
                if (lst[i].Type == TokenType.PunctuationComma)
                {
                    cma.Add(i);
                }
            }
            int s = 0;
            var o = new List<string>();
            for (int i = 0; i < cma.Count; i++) 
            {
                var t = lst[s..cma[i]];
                if (t.Length != 1) return false;
                if (t[0].Type != TokenType.Unknown) return false;
                o.Add(t[0].Raw);
                s = cma[i + 1];
            }
            var w = lst[s..];
            if (w.Length != 1) return false;
            if (w[0].Type != TokenType.Unknown) return false;
            o.Add(w[0].Raw);

            result = new StmtEnumDef(iden, [.. o]);
            return true;
        }
    }
}
