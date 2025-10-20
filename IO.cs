using System.Text;

namespace Calculator.IO;

public sealed class InputBuffer
{
    private readonly StringBuilder _buf = new();

    public (bool ready, string expr) Accept(string line)
    {
        if (line is null) return (false, string.Empty);
        _buf.Append(line);

        var s = _buf.ToString();
        var idx = s.LastIndexOf('=');
        if (idx >= 0)
        {
            var expr = s[..idx];
            _buf.Clear();
            return (true, expr);
        }
        return (false, string.Empty);
    }

    public void Clear() => _buf.Clear();
}

public static class Display
{
    public static void ShowResult(double value) =>
        Console.WriteLine(value.ToString("G15", System.Globalization.CultureInfo.InvariantCulture));

    public static void ShowError(string message) =>
        Console.WriteLine($"Error: {message}");
}
