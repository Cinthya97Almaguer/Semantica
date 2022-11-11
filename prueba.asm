;Archivo: prueba.cpp
;Fecha 10/11/2022 11:40:52 p. m.
#make_COM#
Include 'emu8086.inc'
ORG 100h

;Variables: 
	area DD 0
	radio DD 0
	pi DD 0
	resultado DD 0
	a DW 0
	d DW 0
	altura DW 0
	cinco DW 0
	x DD 0
	y DB 0
	i DW 0
	j DW 0
	k DW 0
PRINT "Introduce la altura de la piramide: "
CALL SCAN_NUM
MOV altura, CX
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP AX
POP BX
CMP AX, BX
JLE if1
MOV AX, altura
PUSH AX
POP AX
inicioFor1:
MOV AX, i
PUSH AX
MOV AX, 0
PUSH AX
POP AX
POP BX
CMP AX, BX
JLE finFor1
MOV AX, 1
PUSH AX
MOV AX, 0
PUSH AX
POP AX
inicioWhile1:
MOV AX, j
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, i
PUSH AX
POP BX 
POP AX 
SUB AX, BX
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE finWhile1
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX, 0
PUSH AX
POP AX
POP BX
CMP AX, BX
JNE if2
PRINT "*"
JMP else2
if2:
PRINT "-"
else2:
MOV AX, 1
PUSH AX
POP AX ADDj, AX
JMP inicioWhile1:
finWhile1:
PRINTN ""
PRINT ""
POP AX ADDj, AX
JMP inicioFor1
finFor1:
MOV AX, 0
PUSH AX
POP AX
inicioDoWhile1:
PRINT "//"
MOV AX, 2
PUSH AX
POP AX ADDk, AX
MOV AX, k
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
POP AX
POP BX
CMP AX, BX
JGE finDoWhile1
JMP inicioDoWhile1
finDoWhile1:
PRINTN ""
PRINT ""
JMP else1
if1:
PRINTN ""
PRINTN "Error: la altura debe de ser mayor que 2"
PRINT ""
else1:
MOV AX, 1
PUSH AX
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX, BX
JE if3
PRINT "Esto no se debe imprimir"
MOV AX, 2
PUSH AX
MOV AX, 2
PUSH AX
POP AX
POP BX
CMP AX, BX
JNE if4
PRINT "Esto tampoco"
JMP else4
if4:
else4:
JMP else3
if3:
else3:
MOV AX, 258
PUSH AX
POP AX
PRINT "Valor de variable int 'a' antes del casteo: "
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX, a
PUSH AX
POP AX
MOV y , AX
PRINTN ""
PRINT "Valor de variable char 'y' despues del casteo de a: "
MOV AX, y
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ""
PRINTN "A continuacion se intenta asignar un int a un char sin usar casteo: "
PRINT ""
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
