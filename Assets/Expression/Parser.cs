using System;
using System.IO;

// Modified from SimpleExpressionEngine by Brad Robinson.
// Source: https://medium.com/@toptensoftware/writing-a-simple-math-expression-engine-in-c-d414de18d4ce

namespace BooleanExpressionEngine
{
    public class Parser
    {
        public Parser(Tokenizer tokenizer) {
            _tokenizer = tokenizer;
        }

        Tokenizer _tokenizer;

        // Parse an entire expression and check EOF was reached
        public Node ParseExpression() {
            var expr = ParseBinaryAddition();

            // Check that all tokens are consumed
            if (_tokenizer.Token != Token.EndOfExpression)
                throw new SyntaxException("Unexpected characters at end of expression");

            return expr;
        }

        // Parse an sequence of OR, XOR on any higher priority terms.
        Node ParseBinaryAddition() {
            // Parse the left hand side for higher priority operators.
            var lhs = ParseBinaryMultiplication();

            while (true) {
                Func<bool, bool, bool> op = null;
                if (_tokenizer.Token == Token.OR) {
                    op = (a, b) => a | b;
                }
                else if (_tokenizer.Token == Token.XOR) {
                    op = (a, b) => a ^ b;
                }

                // Return when binary operator is not found.
                if (op == null)
                    return lhs;

                _tokenizer.NextToken();

                // Parse the right hand side of the expression
                var rhs = ParseBinaryMultiplication();

                // Create a binary node and use it as the left-hand side from now on
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

        // Parse an sequence of AND on any higher priority terms.
        Node ParseBinaryMultiplication() {
            // Parse the left hand side for unary operations.
            var lhs = ParseUnaryNegation();

            while (true) {
                Func<bool, bool, bool> op = null;
                if (_tokenizer.Token == Token.AND) {
                    op = (a, b) => a & b;
                }

                // Return when binary operator is not found.
                if (op == null)
                    return lhs;

                _tokenizer.NextToken();

                // Parse the right hand side for unary operations.
                var rhs = ParseUnaryNegation();

                // Create a binary node and use it as the left-hand side from now on
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }


        // Parses the unary operator, NOT.
        Node ParseUnaryNegation() {
            if (_tokenizer.Token == Token.NOT) {
                _tokenizer.NextToken();

                // Parse the expression on the right (i.e. another negation)
                // Note this recurses to self to support negative of a negative
                var rhs = ParseUnaryNegation();

                // Create unary node
                return new NodeUnary(rhs, (a) => !a);
            }

            // No positive/negative operator so parse a leaf node
            return ParseLeaf();
        }

        // Parse a leaf node (i.e. parentheses, or a literal / variable bit value)
        Node ParseLeaf() {
            // Is it a number?
            if (_tokenizer.Token == Token.Number) {
                var node = new NodeLiteral(_tokenizer.Number);
                _tokenizer.NextToken();
                return node;
            }

            // Parenthesis?
            else if (_tokenizer.Token == Token.OpenBracket) {
                // Skip '('
                _tokenizer.NextToken();

                // Parse a top-level expression
                var node = ParseBinaryAddition();

                // Check and skip ')'
                if (_tokenizer.Token != Token.CloseBracket)
                    throw new SyntaxException("Missing close parenthesis");
                _tokenizer.NextToken();

                // Return
                return node;
            }

            // Variable
            else if (_tokenizer.Token == Token.Identifier) {
                var node = new NodeVariable(_tokenizer.Identifier);
                _tokenizer.NextToken();
                return node;
            }

            // Exception in case where token order doesn't make sense
            throw new SyntaxException($"Unexpected token: {_tokenizer.Token}");
        }


        // Static helper to parse a string
        public static Node Parse(string str) {
            var parser = new Parser(new Tokenizer(new StringReader(str)));
            return parser.ParseExpression();
        }
    }
}
