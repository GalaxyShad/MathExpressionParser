namespace MathExpressionParser;

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

    private readonly string _expression;
    private int _pos = -1;

    private const string _operands = "+-/*()";
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