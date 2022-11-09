Definition:
    Function Definition:
        func <identifier>(<expression.assignment>): <type>
        {
            
        }

Control Statement:
    If Statement:
        if (<Expression.Comparison> <extension operator> <Expression.Comparison>)
        {
            <then>
        }
    Else Statement:
        else
        {
            
        }
    While Loop:
        while (<Expression.Comparison>)
        {

        }
    For Loop:
        for(<expression.assignment.int> to <value.int>)
        {

        }
Assignment:
    Assignment:
        <type> <identifier> = <value>
        var <identifier> = <value>

Operation:
    Comparison:
        <value> <comparisonoperator> <value>
        <value> <comparisonoperator> <value>
            Double Operators:
                ==, !=, <<, >>, <=, >=
            Extension Operators:
                ||, &&
    Binary:
        <value> <operator> <value>
            Operators:
                +, -, /, *, ^

Value:
    String:
        "<String>"
    Float:
        <int>.<int>
    Char:
        '<Char>'
    Int:
        <Int>
    Variable Identifier:
        <identifier>
    Function Call:
        <function name>()