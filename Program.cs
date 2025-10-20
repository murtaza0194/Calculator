using Calculator.IO;
using Calculator.Lex;
using Calculator.Parse;
using Calculator.Eval;
using Calculator.Operations;

var registry = OpRegistry.CreateDefault();
var buffer   = new InputBuffer();

Console.WriteLine("CLI Scientific Calculator (DEG mode). Enter expression and end with '='. Type 'exit' to quit.");
while (true)
{
    Console.Write("calc> ");
    var line = Console.ReadLine();
    if (line is null) break;

    if (line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    var (ready, expr) = buffer.Accept(line);
    if (!ready) continue;
    try
    {
        var tokens = Tokenizer.Tokenize(expr);
        var rpn = ShuntingYard.ToRpn(tokens);
        var result = Evaluator.EvaluateRpn(rpn, registry);
        Display.ShowResult(result);
    }
    catch (Exception ex)
    {
        Display.ShowError(ex.Message);
    }
}