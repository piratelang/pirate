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

    Binary operation:
        <value> <operator> <value>

        Operators:
            +, -, /, *, ^

    Comparison:
        <value> <double operator> <value>
        <value> <double operator> <value> <extension operator> <value> <double operator> <value>
        
        Double Operators:
            ==, !=, <<, >>, <=, >=

        Extension Operators:
            ||, &&

Value:
    String:
        "<String>"
    Char:
        '<Char>'
    Int:
        <Int>
    Variable Identifier:
        <identifier>
    Function Call:
        <function name>()