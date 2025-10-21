namespace Evalr.Core;

internal static class NormalizeTokens
{
    public static List<string> Normalize(List<string> tokens)
    {
        var NORMALIZATION_MAP = Constants.NormalizationMap();
        var normalizedTokens = new List<string>();

        foreach (var token in tokens)
        {
            if (NORMALIZATION_MAP.ContainsKey(token))
            {
                normalizedTokens.Add(NORMALIZATION_MAP[token]);
            }
            else
            {
                normalizedTokens.Add(token);
            }
        }
        return normalizedTokens;
    }
}
