using System.IO;
using UnityEngine;

namespace BooleanExpressionEngine
{
    public class Tokenizer
    {
        public Tokenizer(TextReader reader) {
            _reader = reader;
            NextChar();
            NextToken();
        }

        TextReader _reader;
        char _currentChar;

        public Token Token { get; private set; }

        public bool Number { get; private set; }

        public char Identifier { get; private set; }

        // Read the next character from the input strem
        // and store it in _currentChar, or load '\0' if EOF
        void NextChar() {
            int ch = _reader.Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        // Read the next token from the input stream
        public void NextToken() {
            // Debug.Log(_currentChar);

            // Skip whitespace
            while (char.IsWhiteSpace(_currentChar)) {
                NextChar();
            }

            // Check through special operation characters
            switch (_currentChar) {
                case '\0':
                    Token = Token.EndOfExpression;
                    return;

                case '+':
                    NextChar();
                    Token = Token.OR;
                    return;

                case '^':
                    NextChar();
                    Token = Token.XOR;
                    return;

                case '*':
                case '.':
                    NextChar();
                    Token = Token.AND;
                    return;

                case '!':
                case '¬':
                    NextChar();
                    Token = Token.NOT;
                    return;

                case '(':
                    NextChar();
                    Token = Token.OpenBracket;
                    return;

                case ')':
                    NextChar();
                    Token = Token.CloseBracket;
                    return;
            }

            // Single character identifier
            if (char.IsLetter(_currentChar)) {
                Identifier = _currentChar;
                NextChar();
                Token = Token.Identifier;
                return;
            }

            // Literal false?
            if (_currentChar == '0') {
                Number = false;
                NextChar();
                Token = Token.Number;
                return;
            }

            // Literal true?
            if (_currentChar == '1') {
                Number = true;
                NextChar();
                Token = Token.Number;
                return;
            }

            throw new InvalidDataException("Unexpected character: " + _currentChar);
        }
    }
}
