using Calculator.Operations;

var ops = OpRegistry.CreateDefault();

Console.WriteLine(ops.TryGet("+", out var plus) ? plus!.Apply(2, 3) : double.NaN);
Console.WriteLine(ops.TryGet("-", out var sub) ? sub!.Apply(10, 4, 1) : double.NaN);
Console.WriteLine(ops.TryGet("*", out var mul) ? mul!.Apply(6, 7) : double.NaN);
Console.WriteLine(ops.TryGet("/", out var div) ? div!.Apply(20, 5, 2) : double.NaN);
Console.WriteLine(ops.TryGet("^", out var pow) ? pow!.Apply(2, 8) : double.NaN);
Console.WriteLine(ops.TryGet("%", out var mod) ? mod!.Apply(10, 3) : double.NaN);
Console.WriteLine(ops.TryGet("sqrt", out var sqrt) ? sqrt!.Apply(9) : double.NaN);
Console.WriteLine(ops.TryGet("abs", out var abs) ? abs!.Apply(-12.5) : double.NaN);
Console.WriteLine(ops.TryGet("!", out var fact) ? fact!.Apply(5) : double.NaN);
Console.WriteLine(ops.TryGet("sin", out var sin) ? sin!.Apply(30) : double.NaN);
Console.WriteLine(ops.TryGet("cos", out var cos) ? cos!.Apply(60) : double.NaN);
Console.WriteLine(ops.TryGet("tan", out var tan) ? tan!.Apply(45) : double.NaN);
Console.WriteLine(ops.TryGet("ln", out var ln) ? ln!.Apply(Math.E) : double.NaN);
Console.WriteLine(ops.TryGet("log", out var log) ? log!.Apply(1000) : double.NaN);
Console.WriteLine(ops.TryGet("exp", out var exp) ? exp!.Apply(1) : double.NaN);
