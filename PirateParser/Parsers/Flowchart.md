<!-- 15-11-2022 | 22w45a -->
```mermaid
stateDiagram-v2
    direction LR
    state Fork <<fork>>
    [*] --> Fork

    Fork --> ForLoopStatementParser : TypeControlKeyword.FOR
    state ForLoopStatementParser {
        direction LR
        [*] --> ForLoopStatementNode : Checks for syntax. for [variable assignment] to [body]
    }

    Fork --> WhileLoopStatementParser : TypeControlKeyword.WHILE
    state WhileLoopStatementParser {
        direction LR
        [*] --> WhileLoopStatementNode : Checks for syntax. while [comparison operation] [body]
    }

    Fork --> IfStatementParser : TypeControlKeyword.IF
    state IfStatementParser {
        direction LR
        [*] --> IfStatementNode : Checks for syntax. if [comparison operation] [body]
    }

    Fork --> VariableDeclarationParser : TokenTypeKeyword
    state VariableDeclarationParser {
        direction LR
        [*] --> VariableDeclarationNode : Checks for syntax [type] [identifier] = [value]
    }

    Fork--> VariableAssignmentParser : TokenSyntax.IDENTIFIER
    state VariableAssignmentParser {
        direction LR
        state EqualsOperator <<choice>>
        [*] --> EqualsOperator : Checks for operator following Identifier
        EqualsOperator --> VariableassignemntNode : Equals was found
    }
    EqualsOperator --> Operator
    
    Fork --> OperationParser : TokenValue
    state OperationParser{
        direction LR
        state Operator <<choice>>
        [*] --> Operator : Start with Value token or Identifier
        Operator --> ValueNode : No following operator

        state DoubleOperator <<choice>>
        Operator --> DoubleOperator : A following operator found
        DoubleOperator --> BinaryOperationNode : No Comparison operator
        DoubleOperator --> ComparisonOperationNode : Comparison Operator found
    }
```