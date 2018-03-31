grammar Sequence;

//
// Parser
//


start
	:	document EOF
	;

document
	:	line*
	;

line
	:	statement
	|	NEWLINE
	;

statement
	:	participant
	|	note
	|	title
	|	signal
	;

participant
	:	PARTICIPANT alias=ACTOR
	|	PARTICIPANT name=ACTOR AS alias=ACTOR
	;

note
	:	NOTE placement actor message
	|	NOTE OVER actorPair message
	;

title
	:	TITLE message
	;

actorPair
	:	actor
	|	actor COMMA actor
	;

placement
	:	LEFT_OF
	|	RIGHT_OF
	;

signal
	:	actor signalType actor message
	;

actor
	:	ACTOR
	;

signalType
	:	lineType arrowType
	|	lineType
	;

lineType
	:	LINE
	|	DOTLINE
	;

arrowType
	:	ARROW
	|	OPENARROW
	;

message
	:	MESSAGE
	;

//
// Lexer rules
//

NEWLINE                    : ('\r'? '\n' | '\r')+ ;
WHITESPACE                 : [ \t]+ -> channel(HIDDEN) ;
COMMENT                    : '#' ~[\r\n]* -> channel(HIDDEN) ;
PARTICIPANT                : 'participant' ;
LEFT_OF                    : ('left' ' '+ 'of') ;
RIGHT_OF                   : ('right' ' '+ 'of') ;
OVER                       : 'over' ;
NOTE                       : 'note' ;
TITLE                      : 'title' ;
AS                         : 'as' ;
COMMA                      : ',' ;
DOTLINE                    : '--' ;
LINE                       : '-' ;
OPENARROW                  : '>>' ;
ARROW                      : '>' ;
MESSAGE                    : ':' ~[\r\n]+ ;

ACTOR                      : ACTOR_IDENTIFIER | ACTOR_QUOTED ;
fragment ACTOR_IDENTIFIER  : [a-z0-9_]+ ;
fragment ACTOR_QUOTED      : '"' ~['"']+ '"' ;
