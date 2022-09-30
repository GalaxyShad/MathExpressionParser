using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MathExpressionParser
{
    [TestClass]
    public class TesterTokenizer
    {
        [TestMethod]
        public void CheckFirstDouble()
        {
            var tokenizer = new Tokenizer("6345.1234+87.23 - 8/2");
            var token = tokenizer.CurrentToken;

            token.Value.Should().Be("6345.1234");
            token.Type.Should().Be(TokenType.Number);
        }

        [TestMethod]
        public void CheckFirstOperand()
        {
            var tokenizer = new Tokenizer("6345.1234+87.23 - 8/2");
            var token = tokenizer.NextToken();

            token.Value.Should().Be("+");
            token.Type.Should().Be(TokenType.Operand);
        }
        [TestMethod]
        public void CheckSingleNumber()
        {
            var tokenizer = new Tokenizer("6345.1234");
            var token = tokenizer.CurrentToken;

            token.Value.Should().Be("6345.1234");
            token.Type.Should().Be(TokenType.Number);
        }

        [TestMethod]
        public void CheckEof()
        {
            var tokenizer = new Tokenizer("6345.1234+87.23 - 8/2");
            for(var i = 0; i < 20; i++)
                tokenizer.NextToken();

            var token = tokenizer.NextToken();

            token.Type.Should().Be(TokenType.EOF);
        }
        [TestMethod]
        public void CheckAll()
        {
            var tokenizer = new Tokenizer("6345.1234+87.23 - 8/2");

            var correctTokens = new List<Token>()
            {
                new Token("6345.1234", TokenType.Number),
                new Token("+", TokenType.Operand),
                new Token("87.23", TokenType.Number),
                new Token("-", TokenType.Operand),
                new Token("8", TokenType.Number),
                new Token("/", TokenType.Operand),
                new Token("2", TokenType.Number),
            };

            var token = tokenizer.CurrentToken;
            foreach (var correctToken in correctTokens)
            {
                token.Type.Should().Be(correctToken.Type);
                token.Value.Should().Be(correctToken.Value);

                token = tokenizer.NextToken();
            }
        }
        [TestMethod]
        public void CheckBrackets()
        {
            var tokenizer = new Tokenizer("1.2*(-3)+4-(5+6/(7*8))");

            var correctTokens = new List<Token>()
            {
                new Token("1.2", TokenType.Number),
                new Token("*", TokenType.Operand),
                new Token("(", TokenType.Operand),
                new Token("-", TokenType.Operand),
                new Token("3", TokenType.Number),
                new Token(")", TokenType.Operand),
                new Token("+", TokenType.Operand),
                new Token("4", TokenType.Number),
                new Token("-", TokenType.Operand),
                new Token("(", TokenType.Operand),
                new Token("5", TokenType.Number),
                new Token("+", TokenType.Operand),
                new Token("6", TokenType.Number),
                new Token("/", TokenType.Operand),
                new Token("(", TokenType.Operand),
                new Token("7", TokenType.Number),
                new Token("*", TokenType.Operand),
                new Token("8", TokenType.Number),
                new Token(")", TokenType.Operand),
                new Token(")", TokenType.Operand),

            };

            var token = tokenizer.CurrentToken;
            foreach (var correctToken in correctTokens)
            {
                token.Type.Should().Be(correctToken.Type);
                token.Value.Should().Be(correctToken.Value);

                token = tokenizer.NextToken();
            }
        }
    }

    public enum TokenType
    {
        Number,
        Operand,
        EOF,
        Unknown
    }

    public class Token
    {
        public string Value = "";
        public TokenType Type;

        public Token(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }
    }

    public class Tokenizer
    {
        public Token CurrentToken { get; private set; }

        private string _expression;
        private int _pos = -1;

        private readonly string _operands = "+-/*()";
        public Tokenizer(string expression)
        {
            _expression = expression.Replace(" ", "");
            CurrentToken = ProccesToken();
        }

        public Token NextToken()
        {
            CurrentToken = ProccesToken();
            return CurrentToken;
        }

        private Token ProccesToken()
        {
            _pos++;

            if (_pos >= _expression.Length)
                return new Token("", TokenType.EOF);

            string buff = "";
            if (char.IsDigit(_expression[_pos]))
            {
                buff += _expression[_pos];
                _pos++;

                if (_pos >= _expression.Length)
                    return new Token(buff, TokenType.Number);

                while (char.IsDigit(_expression[_pos]) || _expression[_pos] == '.')
                {
                    buff += _expression[_pos];
                    _pos++;

                    if (_pos >= _expression.Length)
                        return new Token(buff, TokenType.Number);
                }

                _pos--;
                return new Token(buff, TokenType.Number);
            }

            if (_operands.Contains(_expression[_pos]))
            {
                buff += _expression[_pos];
                return new Token(buff, TokenType.Operand);
            }



            return new Token("", TokenType.Unknown);
        }


    }



}