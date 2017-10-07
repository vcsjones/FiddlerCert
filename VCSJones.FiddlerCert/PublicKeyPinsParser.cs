using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCSJones.FiddlerCert
{
    public enum PinAlgorithm
    {
        SHA1,
        SHA256,
        Other,
    }

    public static class PublicKeyPinsParser
    {
        public static PublicPinnedKeys Parse(string rawHeader)
        {
            try
            {
                var keys = new List<PinnedKey>();
                Uri reportUri = null;
                long? maxAge = null;
                bool? includeSubDomains = null;
                var separated = Tokenizers.TokenizeString(rawHeader);
                var identifiers = separated.Select(iv => Tokenizers.TokenizeIdentifiers(iv));
                foreach (var identifier in identifiers)
                {
                    if (identifier.Identifier.Equals("max-age", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!long.TryParse(identifier.Value, out long maxAgeParsed))
                        {
                            continue;
                        }
                        maxAge = maxAgeParsed;
                    }
                    else if (identifier.Identifier.Equals("pin-sha256", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!identifier.IsQuoted)
                        {
                            continue;
                        }
                        var decoded = Convert.FromBase64String(identifier.Value);
                        keys.Add(new PinnedKey(PinAlgorithm.SHA256, decoded));
                    }
                    else if (identifier.Identifier.Equals("pin-sha1", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!identifier.IsQuoted)
                        {
                            continue;
                        }
                        var decoded = Convert.FromBase64String(identifier.Value);
                        keys.Add(new PinnedKey(PinAlgorithm.SHA1, decoded));
                    }
                    else if (identifier.Identifier.StartsWith("pin-", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!identifier.IsQuoted)
                        {
                            continue;
                        }
                        var decoded = Convert.FromBase64String(identifier.Value);
                        keys.Add(new PinnedKey(PinAlgorithm.Other, decoded));
                    }
                    else if (identifier.Identifier.Equals("includesubdomains", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (identifier.Value != null)
                        {
                            continue;
                        }
                        includeSubDomains = true;
                    }

                    else if (identifier.Identifier.Equals("report-uri", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!identifier.IsQuoted)
                        {
                            continue;
                        }
                        if (!Uri.TryCreate(identifier.Value, UriKind.Absolute, out reportUri))
                        {
                            return null;
                        }
                    }
                }
                return new PublicPinnedKeys(keys.AsReadOnly(), maxAge == null ? null : (TimeSpan?)TimeSpan.FromSeconds(maxAge.Value), includeSubDomains, reportUri);
            }
            catch
            {
                return null;
            }
        }
    }

    public static class Tokenizers
    {
        public static IEnumerable<string> TokenizeString(string str, char delimiter = ';')
        {
            var quoted = false;
            const char QUOTE_TOKEN = '"';
            const char SPACE_TOKEN = ' ';
            char[] NEW_LINES = { '\r', '\n' };
            var currentToken = new StringBuilder();
            var whiteSpaceCount = 0;
            var nonQuotingCharacter = false;
            for (var i = 0; i < str.Length; i++)
            {
                var current = str[i];
                //Chew insignicant white space.
                if (current == SPACE_TOKEN && currentToken.Length == 0)
                {
                    continue;
                }
                //Chew insignicant new lines.
                if (NEW_LINES.Any(c => c == current))
                {
                    continue;
                }
                if (current == QUOTE_TOKEN)
                {
                    quoted = !quoted;
                    currentToken.Append(current);
                    whiteSpaceCount = 0;
                    continue;
                }
                else if (current == SPACE_TOKEN)
                {
                    whiteSpaceCount++;
                    continue;
                }
                if (quoted)
                {
                    if (nonQuotingCharacter)
                    {
                        currentToken.Append(new string(SPACE_TOKEN, whiteSpaceCount));
                    }
                    currentToken.Append(current);
                    whiteSpaceCount = 0;
                    nonQuotingCharacter = true;
                }
                else if (current == delimiter)
                {
                    yield return currentToken.ToString();
                    currentToken.Length = 0;
                    whiteSpaceCount = 0;
                    nonQuotingCharacter = false;
                }
                else
                {
                    currentToken.Append(new string(SPACE_TOKEN, whiteSpaceCount));
                    currentToken.Append(current);
                    whiteSpaceCount = 0;
                    nonQuotingCharacter = true;
                }
            }
            if (quoted)
            {
                throw new InvalidOperationException("Dangling open quote.");
            }
            else if (currentToken.Length > 0)
            {
                yield return currentToken.ToString();
            }
        }

        public static KeyValueIdentifier TokenizeIdentifiers(string str, char separator = '=')
        {
            var inQuoteContext = false;
            var valueQuoted = false;
            const char QUOTE_TOKEN = '"';
            const char SPACE_TOKEN = ' ';
            char[] NEW_LINES = { '\r', '\n' };
            var whiteSpaceCount = 0;
            var foundSeparator = false;
            var keyToken = new StringBuilder();
            var valueToken = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var current = str[i];
                //Chew insignicant white space.
                if (current == SPACE_TOKEN && keyToken.Length == 0)
                {
                    continue;
                }
                //Chew insignicant new lines.
                if (NEW_LINES.Any(c => c == current))
                {
                    continue;
                }
                if (current == separator && keyToken.Length == 0)
                {
                    throw new InvalidOperationException("No identifier.");
                }
                if (current == QUOTE_TOKEN && !foundSeparator)
                {
                    throw new InvalidOperationException("Quote inside of key value.");
                }
                if (current == separator)
                {
                    if (foundSeparator)
                    {
                        if (!inQuoteContext)
                        {
                            throw new InvalidOperationException("Separator in unquoted context.");
                        }
                        valueToken.Append(current);
                        continue;
                    }
                    whiteSpaceCount = 0;
                    foundSeparator = true;
                    continue;
                }
                if (current == SPACE_TOKEN)
                {
                    whiteSpaceCount++;
                    continue;
                }
                if (foundSeparator)
                {
                    if (current == QUOTE_TOKEN && valueToken.Length > 0 && !inQuoteContext)
                    {
                        throw new InvalidOperationException("No beginning quote.");
                    }
                    if (current == QUOTE_TOKEN && valueToken.Length == 0)
                    {
                        valueQuoted = true;
                        inQuoteContext = true;
                        whiteSpaceCount = 0;
                        continue;
                    }
                    if (current == QUOTE_TOKEN && valueToken.Length > 0)
                    {
                        inQuoteContext = false;
                        continue;
                    }
                    if (valueToken.Length > 0)
                    {
                        valueToken.Append(new string(SPACE_TOKEN, whiteSpaceCount));
                    }
                    whiteSpaceCount = 0;
                    valueToken.Append(current);
                }
                else
                {
                    if (keyToken.Length > 0)
                    {
                        keyToken.Append(new string(SPACE_TOKEN, whiteSpaceCount));
                    }
                    whiteSpaceCount = 0;
                    keyToken.Append(current);
                }

            }
            return new KeyValueIdentifier(keyToken.ToString(), foundSeparator ? valueToken.ToString() : null, valueQuoted);
        }
    }

    public struct KeyValueIdentifier
    {
        public KeyValueIdentifier(string identifier, string value, bool isQuoted)
        {
            IsQuoted = isQuoted;
            Value = value;
            Identifier = identifier;
        }

        public string Identifier { get; }
        public string Value { get; }
        public bool IsQuoted { get; }
    }

}