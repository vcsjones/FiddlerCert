using System;
using System.Linq;
using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class TokenizerTests
    {
        [Fact]
        public void TestBasicTokenization()
        {
            var input = "hello;world";
            var expectedTokens = new[] { "hello", "world" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input));
        }


        [Fact]
        public void TestBasicTokenizationWithTrailingToken()
        {
            var input = "hello;world;";
            var expectedTokens = new[] { "hello", "world" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input));
        }

        [Fact]
        public void ShouldIgnoreInsignificantWhiteSpacePreToken()
        {
            var input = "   hello;   world";
            var expectedTokens = new[] { "hello", "world" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input));
        }

        [Fact]
        public void ShouldIgnoreInsignificantWhiteSpacePostToken()
        {
            var input = "   hello    ;world    ";
            var expectedTokens = new[] { "hello", "world" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input));
        }

        [Fact]
        public void ShouldNotIgnoreWhiteSpaceInToken()
        {
            var input = "   hell    o;worl    d";
            var expectedTokens = new[] { "hell    o", "worl    d" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input, ';'));
        }

        [Fact]
        public void ShouldNotDelimitWhenInQuotes()
        {
            var input = "\"hello;world\"";
            var expectedTokens = new[] { "\"hello;world\"" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input));
        }

        [Fact]
        public void ShouldRemoveInsignificantWhiteSpaceInsideQuotes()
        {
            var input = "hello;\"    world    \"";
            var expectedTokens = new[] { "hello", "\"world\"" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input).ToArray());
        }

        [Fact]
        public void ShouldThrowExceptionOnMalformedQuoting()
        {
            var input = "\"hello";
            Assert.Throws<InvalidOperationException>(() => Tokenizers.TokenizeString(input).ToArray());
        }

        [Fact]
        public void ShouldThrowExceptionOnMalformedQuotingAtEnd()
        {
            var input = "hello\"";
            Assert.Throws<InvalidOperationException>(() => Tokenizers.TokenizeString(input).ToArray());
        }

        [Fact]
        public void ShouldHandleComplexScenario()
        {
            var input = "\r   Hel\nlo  ;  \"   Wo \rrld  \"  ;  \"By  ;   e \" ";
            var expectedTokens = new[] {"Hello", "\"Wo rld\"", "\"By  ;   e\"" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input, ';'));
        }

        [Fact]
        public void ShouldHandleSignificantWhiteSpace()
        {
            var input = "\"By  ;   e \" ";
            var expectedTokens = new[] { "\"By  ;   e\"" };
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input));
        }

        [Fact]
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
            Assert.Equal(expectedTokens, Tokenizers.TokenizeString(input));
        }


        [Fact]
        public void ShouldHandleValuelessIdentifier()
        {
            var input = "hello";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.Equal("hello", tokenized.Identifier);
            Assert.False(tokenized.IsQuoted);
            Assert.Null(tokenized.Value);
        }

        [Fact]
        public void ShouldHandleSimpleIdentifierAndValue()
        {
            var input = "hello=world";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.Equal("hello", tokenized.Identifier);
            Assert.False(tokenized.IsQuoted);
            Assert.Equal("world", tokenized.Value);
        }

        [Fact]
        public void ShouldHandleSimpleIdentifierAndValueWhileIgnoringInsignificantWhiteSpace()
        {
            var input = "    hel lo  =   wor ld   ";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.Equal("hel lo", tokenized.Identifier);
            Assert.False(tokenized.IsQuoted);
            Assert.Equal("wor ld", tokenized.Value);
        }

        [Fact]
        public void ShouldHandleSimpleIdentifierAndValueWhileIgnoringInsignificantWhiteSpaceWithQuotes()
        {
            var input = "    hel lo  =   \"wor ld\"   ";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.Equal("hel lo", tokenized.Identifier);
            Assert.True(tokenized.IsQuoted);
            Assert.Equal("wor ld", tokenized.Value);
        }

        [Fact]
        public void IdentiferAndValueForHPKP()
        {
            var input = "pin-sha256=\"jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8=\"";
            var tokenized = Tokenizers.TokenizeIdentifiers(input);
            Assert.Equal("pin-sha256", tokenized.Identifier);
            Assert.True(tokenized.IsQuoted);
            Assert.Equal("jV54RY1EPxNKwrQKIa5QMGDNPSbj3VwLPtXaHiEE8y8=", tokenized.Value);
        }
    }
}
