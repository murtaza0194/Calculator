using Calculator.Lex;

namespace Calculator.Parse;

public static class ShuntingYard
{
    private sealed class OpInfo(int prec, bool rightAssoc, bool isPrefix, bool isPostfix)
    {
        public int Prec { get; } = prec;
        public bool RightAssoc { get; } = rightAssoc;
        public bool IsPrefix { get; } = isPrefix;
        public bool IsPostfix { get; } = isPostfix;
    }

    private static readonly Dictionary<string, OpInfo> Ops = new(StringComparer.Ordinal)
    {
        ["!"] = new OpInfo(6, false, false, true),
        ["^"] = new OpInfo(5, true, false, false),
        ["neg"] = new OpInfo(4, true, true, false),
        ["sqrt"] = new OpInfo(4, true, true, false),
        ["sin"] = new OpInfo(4, true, true, false),
        ["cos"] = new OpInfo(4, true, true, false),
        ["tan"] = new OpInfo(4, true, true, false),
        ["ln"] = new OpInfo(4, true, true, false),
        ["log"] = new OpInfo(4, true, true, false),
        ["exp"] = new OpInfo(4, true, true, false),
        ["*"] = new OpInfo(3, false, false, false),
        ["/"] = new OpInfo(3, false, false, false),
        ["%"] = new OpInfo(3, false, false, false),
        ["+"] = new OpInfo(2, false, false, false),
        ["-"] = new OpInfo(2, false, false, false),
    };

    private static bool IsUnaryMinus(Token? prev)
    {
        if (prev is null) return true;
        return prev.Value.Type switch
        {
            TokenType.Number => false,
            TokenType.RParen => false,
            _ => true
        };
    }
    
    public static List<Token> ToRpn(List<Token> input)
    {
        var output = new List<Token>();
        var stack  = new Stack<Token>();
        Token? prev = null;

        for (int i = 0; i < input.Count; i++)
        {
            var t = input[i];

            if (t.Type == TokenType.Number)
            {
                output.Add(t);
                prev = t;
                continue;
            }

            if (t.Type == TokenType.Op)
            {
                var sym = t.Text;
                if (sym == "-" && IsUnaryMinus(prev))
                    sym = "neg";
                if (!Ops.TryGetValue(sym, out var info))
                    throw new Exception($"Unknown operator '{sym}'.");
                while (stack.Count > 0 && stack.Peek().Type == TokenType.Op)
                {
                    var topSym = stack.Peek().Text;
                    var topInfo = Ops[topSym];

                    bool higher = topInfo.Prec > info.Prec;
                    bool equalAndLeft = topInfo.Prec == info.Prec && !info.RightAssoc;

                    if (higher || equalAndLeft)
                        output.Add(stack.Pop());
                    else
                        break;
                }
                stack.Push(new Token(TokenType.Op, sym));
                prev = new Token(TokenType.Op, sym);
                continue;
            }
            if (t.Type == TokenType.LParen)
            {
                stack.Push(t);
                prev = t;
                continue;
            }
            if (t.Type == TokenType.RParen)
            {
                while (stack.Count > 0 && stack.Peek().Type != TokenType.LParen)
                    output.Add(stack.Pop());

                if (stack.Count == 0) throw new Exception("Mismatched parentheses.");
                stack.Pop();
                if (stack.Count > 0 && stack.Peek().Type == TokenType.Op)
                {
                    var topSym = stack.Peek().Text;
                    if (Ops[topSym].IsPrefix)
                        output.Add(stack.Pop());
                }
                prev = t;
                continue;
            }
            if (t.Type == TokenType.Equals)
            {
                prev = t;
                continue;
            }
            throw new Exception($"Unexpected token: {t}");
        }

        while (stack.Count > 0)
        {
            var s = stack.Pop();
            if (s.Type == TokenType.LParen) throw new Exception("Mismatched parentheses.");
            output.Add(s);
        }

        return output;
    }
}