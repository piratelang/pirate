Definition:
    Function Definition:
        func <identifier>(<expression.assignment>): <type>
        {
            
        }

Control Statement:
    If Statement:
        if (<Expression.Comparison>)
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

Expression:
    Assignment:
        <type> <identifier> = <value>
        var <identifier> = <value>
    Comparison:
        <value> <double operator> <value>
        <value> <double operator> <value> <extension operator> <value> <double operator> <value>
        Double Operators:
            ==, !=, <<, >>, <=, >=
        Extension Operators:
            ||, &&
    Binary operation:
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