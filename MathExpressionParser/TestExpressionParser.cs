using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Diagnostics;
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

}