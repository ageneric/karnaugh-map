using System;
using System.Collections.Generic;
using UnityEngine;

namespace BooleanExpressionEngine
{
    // Node - abstract class representing one node in the expression 
    public abstract class Node
    {
        public abstract bool Eval(Dictionary<char, bool> ctx);
    }

    // NodeBinary for binary operations AND, OR, XOR
    class NodeBinary : Node
    {
        // Constructor accepts the two nodes to be operated on
        // and function that performs the actual operation
        public NodeBinary(Node lhs, Node rhs, Func<bool, bool, bool> op) {
            _lhs = lhs;
            _rhs = rhs;
            _op = op;
        }

        Node _lhs;                              // Left hand side of the operation
        Node _rhs;                              // Right hand side of the operation
        Func<bool, bool, bool> _op;       // The callback operator

        public override bool Eval(Dictionary<char, bool> ctx) {
            // Evaluate both sides
            var lhsVal = _lhs.Eval(ctx);
            var rhsVal = _rhs.Eval(ctx);

            // Evaluate and return
            var result = _op(lhsVal, rhsVal);
            return result;
        }
    }

    // NodeUnary for unary operations NOT
    class NodeUnary : Node
    {
        // Constructor accepts the two nodes to be operated on and function
        // that performs the actual operation
        public NodeUnary(Node rhs, Func<bool, bool> op) {
            _rhs = rhs;
            _op = op;
        }

        Node _rhs;                          // Right hand side of the operation
        Func<bool, bool> _op;               // The callback operator

        public override bool Eval(Dictionary<char, bool> ctx) {
            // Evaluate RHS
            var rhsVal = _rhs.Eval(ctx);

            // Evaluate and return
            var result = _op(rhsVal);
            return result;
        }
    }

    // Represents a literal number in an expression.
    class NodeLiteral : Node
    {
        public NodeLiteral(bool number) {
            _number = number;
        }

        bool _number;

        public override bool Eval(Dictionary<char, bool> ctx) {
            return _number;
        }
    }

    // Represents a variable (or a constant) in an expression.
    public class NodeVariable : Node
    {
        public NodeVariable(char variableName) {
            _variableName = variableName;
        }

        char _variableName;

        public override bool Eval(Dictionary<char, bool> ctx) {
            try {
                Debug.Log(_variableName);
                return ctx[_variableName];
            }
            catch (KeyNotFoundException) {
                throw new SyntaxException("Invalid variable name: please use only 'A', 'B', 'C', 'D' and/or 'E'");
            }
        }
    }
}
