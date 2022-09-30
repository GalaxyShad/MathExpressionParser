using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MathExpressionParser
{
    [TestClass]
    public class TesterMathExpression
    {
        [TestMethod]
        public void CheckDouble()
        {
            var mathExpression = new MathExpression();
            mathExpression.Parse("2.6543").Should().Be(2.6543);
        }

        [TestMethod]
        public void CheckPlus()
        {
            var mathExpression = new MathExpression();

            //mathExpression.DivideToPriorityTokens("2.50+6.50").Count.Should().Be(3);
            mathExpression.Parse("2.50+6.50").Should().Be(2.50+6.50);
        }

        [TestMethod]
        public void CheckOrder()
        {
            var mathExpression = new MathExpression();

            mathExpression.Parse("2 + 2 * 2 /2").
                Should().Be(2+2* 2 / 2);
        }

        [TestMethod]
        public void CheckBrackets()
        {
            var mathExpression = new MathExpression();

            mathExpression.Parse("(1.0 + (1.0 + 3.0 / (1.0 + 1.0))) * (2.0 / 2.0) + 8.0").
                Should().Be((1.0 + (1.0 + 3.0 / (1.0 + 1.0))) * (2.0 / 2.0) + 8.0);
        }
        [TestMethod]
        public void CheckFindSubExpr()
        {
            var mathExpression = new MathExpression();

            var sExpr = "(2+(7+90/(1+1))) * (2 /2) + 76";
            sExpr = mathExpression.FindSubExpression(sExpr);
            sExpr.Should().Be("2+(7+90/(1+1))");

            sExpr = mathExpression.FindSubExpression(sExpr);
            sExpr.Should().Be("7+90/(1+1)");

            sExpr = mathExpression.FindSubExpression(sExpr);
            sExpr.Should().Be("1+1");

            sExpr = mathExpression.FindSubExpression(sExpr);
            sExpr.Should().Be(null);
        }


    }

    public class PriorityToken : Token
    {
        public int Priority { get; private set; }
        public PriorityToken(string value, TokenType type, int priority) : base(value, type)
        {
            Priority = priority;
        }
    }

    public class MathExpression
    {
        //private List<PriorityToken> tokens = new();

        public List<PriorityToken> DivideToPriorityTokens(string sExpression)
        {
            var tokenizer = new Tokenizer(sExpression);

            List<PriorityToken> tokens = new List<PriorityToken>();

            for (var token = tokenizer.CurrentToken;
                 token.Type != TokenType.EOF;
                 token = tokenizer.NextToken())
            {
                if (token.Type == TokenType.Number)
                {
                    tokens.Add(new PriorityToken(token.Value, token.Type, 0));
                }
                else
                {
                    int priority = 0;
                    switch (token.Value)
                    {
                        case "+": priority = 1; break;
                        case "-": priority = 1; break;
                        case "*": priority = 2; break;
                        case "/": priority = 3; break;
                        //case "(": priority = 4; break;

                    }

                    tokens.Add(new PriorityToken(token.Value, token.Type, priority));
                }
            }

            return tokens;
        }

        public string? FindSubExpression(string sExpression)
        {
            var inCount = 0;
            var str = "";
            var isBracketFound = false;

            foreach (var sym in sExpression)
            {
                if (!isBracketFound)
                {
                    if (sym == '(')
                        isBracketFound = true;
                    else
                        continue;
                }

                str += sym;

                if (sym == '(') inCount++;

                if (sym == ')')
                {
                    
                    inCount--;
                    if (inCount == 0)
                        return str.Substring(1, str.Length - 2);
                }
                
                
            }

            return null;
        }

        public double Parse(string sExpression)
        {
            var sub = FindSubExpression(sExpression);
            var res = 0.0;
            while (sub != null)
            {
                res = Parse(sub);
                sExpression = sExpression.Replace($"({sub})", res.ToString(System.Globalization.CultureInfo.InvariantCulture));
                sub = FindSubExpression(sExpression);
            }
                

            List<PriorityToken> tokens = DivideToPriorityTokens(sExpression);

            while (tokens.Count > 1)
            {
                var token = tokens.Aggregate((t1, t2) => t1.Priority > t2.Priority ? t1 : t2);
                var index = tokens.IndexOf(token);
              
                double temp = 0;

                var right = double.Parse(tokens[index + 1].Value, CultureInfo.InvariantCulture);
                var left = double.Parse(tokens[index - 1].Value, CultureInfo.InvariantCulture);

                switch (token.Value)
                {
                    case "+": temp = left + right; break;
                    case "-": temp = left - right; break;
                    case "/": temp = left / right; break;
                    case "*": temp = left * right; break;
                }

                tokens.RemoveAt(index + 1);
                tokens.RemoveAt(index);

                tokens[index - 1].Type = TokenType.Number;
                tokens[index - 1].Value = temp.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
           

            return double.Parse(tokens[0].Value, CultureInfo.InvariantCulture);
        }

    }

    

}