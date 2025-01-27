using System;

namespace CommitmentsService.Extensions;

public static class NumberExtensions
{
    public static string ToScaledString(this double number)
    {
        var magnitude = 0;
        while (!((number / Math.Pow(10,  3 * (magnitude + 1))) < 1))
        {
            magnitude++;
        }
        var scale = magnitude switch
        {
            0 => "",
            1 => "K",
            2 => "M",
            3 => "B",
            4 => "T",
            _ => throw new ArgumentOutOfRangeException(nameof(number))
        };
        var leadingDisplay = Math.Round(((decimal)number) / ((decimal)Math.Pow(10, 3 * magnitude)), 1);
        return $"{leadingDisplay}{scale}";
    }
}
