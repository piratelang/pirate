grammar Pirate;

// Main program entry point
program: statement+ EOF;

// Statements
statement
    : functionDeclaration
    | variableDeclaration
    | variableAssignment
    | ifStatement
    | whileStatement
    | forStatement
    | foreachStatement
    | expressionStatement
    | externStatement
    | comment
    ;

// Function declaration
functionDeclaration
    : 'func' IDENTIFIER '(' parameterList? ')' ':' type '{' statement* returnStatement? '}'
    ;

parameterList
    : parameter (',' parameter)*
    ;

parameter
    : type IDENTIFIER
    | 'var' IDENTIFIER
    ;

returnStatement
    : 'return' expression ';'?
    ;

// Variable declarations
variableDeclaration
    : type IDENTIFIER '=' expression ';'?
    | 'var' IDENTIFIER '=' expression ';'?
    ;

variableAssignment
    : IDENTIFIER '=' expression ';'?
    | IDENTIFIER '[' expression ']' '=' expression ';'?
    ;

// Control flow statements
ifStatement
    : 'if' expression '{' statement* '}' elifStatement* elseStatement?
    ;

elifStatement
    : 'elif' expression '{' statement* '}'
    ;

elseStatement
    : 'else' '{' statement* '}'
    ;

whileStatement
    : 'while' expression '{' statement* '}'
    ;

forStatement
    : 'for' 'var' IDENTIFIER '=' expression 'to' expression '{' statement* '}'
    ;

foreachStatement
    : 'foreach' '(' 'var' IDENTIFIER 'in' expression ')' '{' statement* '}'
    ;

// External function import
externStatement
    : 'extern' IDENTIFIER ('.' IDENTIFIER)* ';'?
    ;

// Expression statement
expressionStatement
    : expression ';'?
    ;

// Expressions
expression
    : expression binaryOperator expression           # BinaryExpression
    | expression comparisonOperator expression       # ComparisonExpression
    | expression logicalOperator expression          # LogicalExpression
    | '(' expression ')'                            # ParenthesizedExpression
    | functionCall                                  # FunctionCallExpression
    | IDENTIFIER '[' expression ']'                # ArrayAccessExpression
    | IDENTIFIER                                    # IdentifierExpression
    | literal                                       # LiteralExpression
    ;

// Function call
functionCall
    : IDENTIFIER '(' argumentList? ')'
    ;

argumentList
    : expression (',' expression)*
    ;

// Literals
literal
    : STRING                                        # StringLiteral
    | INTEGER                                       # IntegerLiteral
    | FLOAT                                         # FloatLiteral
    | CHAR                                          # CharLiteral
    | BOOLEAN                                       # BooleanLiteral
    | arrayLiteral                                  # ArrayLiteralExpression
    ;

arrayLiteral
    : '[' (expression (',' expression)*)? ']'
    ;

// Operators
binaryOperator
    : '+'
    | '-'
    | '*'
    | '/'
    | '^'
    ;

comparisonOperator
    : '=='
    | '!='
    | '<'
    | '<='
    | '>'
    | '>='
    ;

logicalOperator
    : '&&'
    | '||'
    | 'and'
    | 'or'
    ;

// Types
type
    : 'int'                                         # PrimitiveType
    | 'string'                                      # PrimitiveType
    | 'float'                                       # PrimitiveType
    | 'char'                                        # PrimitiveType
    | 'bool'                                        # PrimitiveType
    | 'void'                                        # PrimitiveType
    | type '[' ']'                                  # ArrayType
    ;

// Comments
comment
    : SINGLE_LINE_COMMENT
    | MULTI_LINE_COMMENT
    ;

// Lexer rules
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;

INTEGER: [0-9]+;

FLOAT: [0-9]+ '.' [0-9]+;

STRING: '"' (~["\r\n] | '\\' .)* '"';

CHAR: '\'' (~['\r\n] | '\\' .) '\'';

BOOLEAN: 'true' | 'false';

// Comments
SINGLE_LINE_COMMENT: '//' ~[\r\n]* -> skip;
MULTI_LINE_COMMENT: '/*' .*? '*/' -> skip;

// Whitespace
WS: [ \t\r\n]+ -> skip;

// Keywords (reserved words)
FUNC: 'func';
VAR: 'var';
IF: 'if';
ELIF: 'elif';
ELSE: 'else';
WHILE: 'while';
FOR: 'for';
FOREACH: 'foreach';
IN: 'in';
TO: 'to';
RETURN: 'return';
EXTERN: 'extern';
TRUE: 'true';
FALSE: 'false';

// Type keywords
INT: 'int';
STRING_TYPE: 'string';
FLOAT_TYPE: 'float';
CHAR_TYPE: 'char';
BOOL_TYPE: 'bool';
VOID: 'void';

// Logical operators
AND: 'and';
OR: 'or';