using System;
using System.Linq;
using NUnit.Framework;

namespace VCSJones.FiddlerCert.UnitTests
{
    [TestFixture]
    public class TokenizerTests
    {
        [Test]
        public void TestBasicTokenization()
        {
            var input = "hello;world";
            var expectedTokens = new[] { "hello", "world" };
            Assert.That(Tokenizers.TokenizeString(input).ToArray(), Is.EqualTo(expectedTokens));
        }


        [Test]
        public void TestBasicTokenizationWithTrailingToken()
        {
            var input = "hello;world;";
            var expectedTokens = new[] { "hello", "world" };
            Assert.That(Tokenizers.TokenizeString(input).ToArray(), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldIgnoreInsignificantWhiteSpacePreToken()
        {
            var input = "   hello;   world";
            var expectedTokens = new[] { "hello", "world" };
            Assert.That(Tokenizers.TokenizeString(input).ToArray(), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldIgnoreInsignificantWhiteSpacePostToken()
        {
            var input = "   hello    ;world    ";
            var expectedTokens = new[] { "hello", "world" };
            Assert.That(Tokenizers.TokenizeString(input).ToArray(), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldNotIgnoreWhiteSpaceInToken()
        {
            var input = "   hell    o;worl    d";
            var expectedTokens = new[] { "hell    o", "worl    d" };
            Assert.That(Tokenizers.TokenizeString(input, ';').ToArray(), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldNotDelimitWhenInQuotes()
        {
            var input = "\"hello;world\"";
            var expectedTokens = new[] { "\"hello;world\"" };
            Assert.That(Tokenizers.TokenizeString(input).ToArray(), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldRemoveInsignificantWhiteSpaceInsideQuotes()
        {
            var input = "hello;\"    world    \"";
            var expectedTokens = new[] { "hello", "\"world\"" };
            Assert.That(Tokenizers.TokenizeString(input).ToArray(), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldThrowExceptionOnMalformedQuoting()
        {
            var input = "\"hello";
            Assert.That(() => Tokenizers.TokenizeString(input).ToArray(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ShouldThrowExceptionOnMalformedQuotingAtEnd()
        {
            var input = "hello\"";
            Assert.That(() => Tokenizers.TokenizeString(input).ToArray(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ShouldHandleComplexScenario()
        {
            var input = "\r   Hel\nlo  ;  \"   Wo \rrld  \"  ;  \"By  ;   e \" ";
            var expectedTokens = new[] {"Hello", "\"Wo rld\"", "\"By  ;   e\"" };
            Assert.That(Tokenizers.TokenizeString(input, ';'), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldHandleSignificantWhiteSpace()
        {
            var input = "\"By  ;   e \" ";
            var expectedTokens = new[] { "\"By  ;   e\"" };
            Assert.That(Tokenizers.TokenizeString(input), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldSampleHpkpHeader()
        {
            var input = "pin-sha256=\"jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8=\"; pin-sha256=\"7qVfhXJFRlcy/9VpKFxHBuFzvQZSqajgfRwvsdx1oG8=\"; pin-sha256=\"/sMEqQowto9yX5BozHLPdnciJkhDiL5+Ug0uil3DkUM=\"; max-age=5184000;";
            var expectedTokens = new[]
            {
                "pin-sha256=\"jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8=\"",
                "pin-sha256=\"7qVfhXJFRlcy/9VpKFxHBuFzvQZSqajgfRwvsdx1oG8=\"",
                "pin-sha256=\"/sMEqQowto9yX5BozHLPdnciJkhDiL5+Ug0uil3DkUM=\"",
                "max-age=5184000"
            };
            Assert.That(Tokenizers.TokenizeString(input).ToArray(), Is.EqualTo(expectedTokens));
        }

        [Test]
        public void ShouldThrowErrorOnMissingKey()
        {
            var input = "=hello";
            Assert.That(() => Tokenizers.TokenizeIdentifiers(input), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ShouldHandleValuelessIdentifier()
        {
            var input = "hello";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.That(tokenized.Identifier, Is.EqualTo("hello"));
            Assert.That(tokenized.IsQuoted, Is.False);
            Assert.That(tokenized.Value, Is.Null);
        }

        [Test]
        public void ShouldHandleSimpleIdentifierAndValue()
        {
            var input = "hello=world";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.That(tokenized.Identifier, Is.EqualTo("hello"));
            Assert.That(tokenized.IsQuoted, Is.False);
            Assert.That(tokenized.Value, Is.EqualTo("world"));
        }

        [Test]
        public void ShouldHandleSimpleIdentifierAndValueWhileIgnoringInsignificantWhiteSpace()
        {
            var input = "    hel lo  =   wor ld   ";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.That(tokenized.Identifier, Is.EqualTo("hel lo"));
            Assert.That(tokenized.IsQuoted, Is.False);
            Assert.That(tokenized.Value, Is.EqualTo("wor ld"));
        }

        [Test]
        public void ShouldHandleSimpleIdentifierAndValueWhileIgnoringInsignificantWhiteSpaceWithQuotes()
        {
            var input = "    hel lo  =   \"wor ld\"   ";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.That(tokenized.Identifier, Is.EqualTo("hel lo"));
            Assert.That(tokenized.IsQuoted, Is.True);
            Assert.That(tokenized.Value, Is.EqualTo("wor ld"));
        }

        [Test]
        public void IdentiferAndValueForHPKP()
        {
            var input = "pin-sha256=\"jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8=\"";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.That(tokenized.Identifier, Is.EqualTo("pin-sha256"));
            Assert.That(tokenized.IsQuoted, Is.True);
            Assert.That(tokenized.Value, Is.EqualTo("jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8="));
        }
    }
}
