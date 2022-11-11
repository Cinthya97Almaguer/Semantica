;Archivo: prueba.cpp
;Fecha 10/11/2022 10:02:59 p. m.
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
inicioWhile1
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
JMP inicioWhile1
finWhile1:
PRINTN ""
PRINT ""
POP AX ADDj, AX
JMP inicioFor1
finFor1:
inicioWhile1:
inicioWhile1:
inicioWhile1:
inicioWhile1:
inicioWhile1:
MOV AX, 0
PUSH AX
POP AX
inicioWhile1:
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
JGE finWhile1
JMP inicioWhile1
finWhile1:
