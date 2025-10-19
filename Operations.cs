namespace Calculator.Operations;

public interface IOperation
{
    string Symbol { get; }
    int Arity { get; }
    double Apply(params double[] args);

}

public abstract class OpBase : IOperation
{
    public abstract string Symbol { get; }
    public abstract int Arity { get; }
    public abstract double Apply(params double[] args);

    protected static void RequireExact(double[] args, int expected)
    {
        if (args is null) throw new ArgumentNullException(nameof(args));
        if (args.Length != expected)
            throw new ArgumentException($"Expected {expected} argument(s), got {args.Length}.");
    }

    protected static void RequireAtLeast(double[] args, int min)
    {
        if (args is null) throw new ArgumentNullException(nameof(args));
        if (args.Length < min)
            throw new ArgumentException($"Expected at least {min} argument(s), got {args.Length}.");
    }

    protected static bool IsInteger(double x, double eps = 1e-9)
        => Math.Abs(x - Math.Round(x)) <= eps;
}

internal static class Angle
{
    public static double DegToRad(double deg) => deg * Math.PI / 180.0;
}

public sealed class AddOp : OpBase
{
    public override string Symbol => "+";
    public override int Arity => 2;
    public override double Apply(params double[] args)
    {
        RequireAtLeast(args, 2);
        double s = 0;
        foreach (var a in args) s += a;
        return s;
    }
}

public sealed class SubOp : OpBase
{
    public override string Symbol => "-";
    public override int Arity => 2;
    public override double Apply(params double[] args)
    {
        RequireAtLeast(args, 2);
        double res = args[0];
        for (int i = 1; i < args.Length; i++) res -= args[i];
        return res;
    }
}

public sealed class MulOp : OpBase
{
    public override string Symbol => "*";
    public override int Arity => 2;
    public override double Apply(params double[] args)
    {
        RequireAtLeast(args, 2);
        double res = 1;
        foreach (var a in args) res *= a;
        return res;
    }
}

public sealed class DivOp : OpBase
{
    public override string Symbol => "/";
    public override int Arity => 2;
    public override double Apply(params double[] args)
    {
        RequireAtLeast(args, 2);
        double res = args[0];
        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] == 0) throw new DivideByZeroException("Cannot divide by zero.");
            res /= args[i];
        }
        return res;
    }
}

public sealed class ModOp : OpBase
{
    public override string Symbol => "%";
    public override int Arity => 2;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 2);
        if (args[1] == 0) throw new DivideByZeroException("Cannot modulo by zero.");
        return args[0] % args[1];
    }
}

public sealed class PowOp : OpBase
{
    public override string Symbol => "^";
    public override int Arity => 2;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 2);
        return Math.Pow(args[0], args[1]);
    }
}

public sealed class SqrtOp : OpBase
{
    public override string Symbol => "sqrt";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        if (args[0] < 0) throw new ArgumentException("sqrt domain error (x must be >= 0).");
        return Math.Sqrt(args[0]);
    }
}

public sealed class AbsOp : OpBase
{
    public override string Symbol => "abs";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        return Math.Abs(args[0]);
    }
}

public sealed class FactOp : OpBase
{
    public override string Symbol => "!";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        double x = args[0];
        if (!IsInteger(x)) throw new ArgumentException("factorial domain error (n must be an integer).");
        int n = (int)Math.Round(x);
        if (n < 0) throw new ArgumentException("factorial domain error (n must be >= 0).");
        if (n > 170) throw new OverflowException("factorial overflow on double (n too large).");

        double res = 1;
        for (int i = 2; i <= n; i++) res *= i;
        return res;
    }
}

public sealed class SinOp : OpBase
{
    public override string Symbol => "sin";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        return Math.Sin(Angle.DegToRad(args[0]));
    }
}
public sealed class CosOp : OpBase
{
    public override string Symbol => "cos";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        return Math.Cos(Angle.DegToRad(args[0]));
    }
}
public sealed class TanOp : OpBase
{
    public override string Symbol => "tan";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        return Math.Tan(Angle.DegToRad(args[0]));
    }
}

public sealed class LnOp : OpBase
{
    public override string Symbol => "ln";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        if (args[0] <= 0) throw new ArgumentException("ln domain error (x must be > 0).");
        return Math.Log(args[0]);
    }
}

public sealed class Log10Op : OpBase
{
    public override string Symbol => "log";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        if (args[0] <= 0) throw new ArgumentException("log10 domain error (x must be > 0).");
        return Math.Log10(args[0]);
    }
}

public sealed class ExpOp : OpBase
{
    public override string Symbol => "exp";
    public override int Arity => 1;
    public override double Apply(params double[] args)
    {
        RequireExact(args, 1);
        return Math.Exp(args[0]);
    }
}

public sealed class OpRegistry
{
    private readonly Dictionary<string, IOperation> _map =
        new(StringComparer.Ordinal);

    public OpRegistry Register(IOperation op)
    {
        if (op is null) throw new ArgumentNullException(nameof(op));
        _map[op.Symbol] = op;
        return this;
    }

    public bool TryGet(string symbol, out IOperation? op) => _map.TryGetValue(symbol, out op);
    public static OpRegistry CreateDefault() => new OpRegistry()
        .Register(new AddOp())
        .Register(new SubOp())
        .Register(new MulOp())
        .Register(new DivOp())
        .Register(new ModOp())
        .Register(new PowOp())
        .Register(new SqrtOp())
        .Register(new AbsOp())
        .Register(new FactOp())
        .Register(new SinOp())
        .Register(new CosOp())
        .Register(new TanOp())
        .Register(new LnOp())
        .Register(new Log10Op())
        .Register(new ExpOp());
}