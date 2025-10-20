using System.Globalization;
using Calculator.Lex;
using Calculator.Operations;
namespace Calculator.Eval;

public static class Evaluator
{
    public static double EvaluateRpn(List<Token> rpn, OpRegistry registry)
    {
        var stack = new Stack<double>();

        foreach (var t in rpn)
        {
            if (t.Type == TokenType.Number)
            {
                if (!double.TryParse(t.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                    throw new Exception($"Invalid number '{t.Text}'.");
                stack.Push(v);
                continue;
            }

            if (t.Type == TokenType.Op)
            {
                if (!registry.TryGet(t.Text, out var op) || op is null)
                    throw new Exception($"Unknown op '{t.Text}'.");

                if (op.Arity == 1)
                {
                    if (stack.Count < 1) throw new Exception($"Operator '{t.Text}' missing operand.");
                    var a = stack.Pop();
                    var res = op.Apply(a);
                    stack.Push(res);
                }
                else
                {
                    if (stack.Count < 2) throw new Exception($"Operator '{t.Text}' missing operands.");
                    var b = stack.Pop();
                    var a = stack.Pop();
                    var res = op.Apply(a, b);
                    stack.Push(res);
                }
                continue;
            }

            throw new Exception($"Unexpected token in RPN: {t}");
        }

        if (stack.Count != 1) throw new Exception("Invalid expression.");
        return stack.Pop();
    }
}
