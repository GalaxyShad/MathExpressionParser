using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MathExpressionParser;

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
    public List<PriorityToken> DivideToPriorityTokens(string sExpression)
    {
        var tokenizer = new Tokenizer(sExpression);

        var tokens = new List<PriorityToken>();

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

            switch (sym)
            {
                case '(':
                    inCount++;
                    break;
                case ')':
                {
                    inCount--;
                    if (inCount == 0)
                        return str.Substring(1, str.Length - 2);
                    break;
                }
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
                

        var tokens = DivideToPriorityTokens(sExpression);

        while (tokens.Count > 1)
        {
            var token = tokens.Aggregate((t1, t2) => t1.Priority > t2.Priority ? t1 : t2);
            var index = tokens.IndexOf(token);
              
            double temp = 0;

            var right = double.Parse(tokens[index + 1].Value, CultureInfo.InvariantCulture);
            var left = double.Parse(tokens[index - 1].Value, CultureInfo.InvariantCulture);

            temp = token.Value switch
            {
                "+" => left + right,
                "-" => left - right,
                "/" => left / right,
                "*" => left * right,
                _ => temp
            };

            tokens.RemoveAt(index + 1);
            tokens.RemoveAt(index);

            tokens[index - 1].Type = TokenType.Number;
            tokens[index - 1].Value = temp.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
           

        return double.Parse(tokens[0].Value, CultureInfo.InvariantCulture);
    }

}