using System.Globalization;

namespace Calculator.Lex;

public enum TokenType { Number, Op, LParen, RParen, Equals }

public readonly record struct Token(TokenType Type, string Text);

public static class Tokenizer
{
    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        int i = 0;
        while (i < input.Length)
        {
            char c = input[i];
            if (char.IsWhiteSpace(c)) { i++; continue; }
            if (char.IsDigit(c) || c == '.')
            {
                int start = i;
                bool dotSeen = (c == '.');
                i++;
                while (i < input.Length)
                {
                    char d = input[i];
                    if (char.IsDigit(d)) { i++; }
                    else if (d == '.' && !dotSeen) { dotSeen = true; i++; }
                    else break;
                }
                tokens.Add(new Token(TokenType.Number, input.Substring(start, i - start)));
                continue;
            }
            if (char.IsLetter(c))
            {
                int start = i;
                i++;
                while (i < input.Length && char.IsLetter(input[i])) i++;
                var name = input.Substring(start, i - start).ToLowerInvariant();
                tokens.Add(new Token(TokenType.Op, name));
                continue;
            }

            switch (c)
            {
                case '+': case '-': case '*': case '/': case '%': case '^': case '!':
                    tokens.Add(new Token(TokenType.Op, c.ToString()));
                    i++; break;
                case '(':
                    tokens.Add(new Token(TokenType.LParen, "(")); i++; break;
                case ')':
                    tokens.Add(new Token(TokenType.RParen, ")")); i++; break;
                case '=':
                    tokens.Add(new Token(TokenType.Equals, "=")); i++; break;
                default:
                    throw new Exception($"Unexpected character '{c}' at position {i}.");
            }
        }
        return tokens;
    }
}