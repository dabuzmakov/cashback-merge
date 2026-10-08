namespace CashBackMerge.Game.Extensions;

public static class DecimalExtensions
{
    public static int GetScale(this decimal number)
        => (decimal.GetBits(number)[3] >> 16) & 0xFF;
}
