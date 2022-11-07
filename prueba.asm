;Archivo: prueba.cpp
;Fecha 07/11/2022 09:49:41 a. m.
#make_COM#
Include 'emu8086.inc'
ORG 100h

;Variables: 
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	a DW ?
	d DW ?
	altura DW ?
	cinco DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
	k DW ?
PRINTN "Introduce la altura de la piramide: "
CALL scan_num
MOV altura, CX
MOV AX, 2
PUSH AX
POP AX
POP BX
CMP AX; BX
JLE if1
inicioFor0
POP AX
MOViAX
MOV AX, 0
PUSH AX
POP AX
POP BX
CMP AX; BX
JLE 
MOV AX, 1
PUSH AX
POP, AX
MOV AX, 0
PUSH AX
POP AX
MOVjAX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP AX
POP BX
CMP AX; BX
JGE 
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
CMP AX; BX
JNE if2
PRINTN "*"
PRINTN "-"
if2:
MOV AX, 1
PUSH AX
POP, AX
PRINTN "
"
MOV AX, 0
PUSH AX
POP AX
POP BX
CMP AX; BX
JLE 
MOV AX, 1
PUSH AX
POP, AX
MOV AX, 0
PUSH AX
POP AX
MOVjAX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP AX
POP BX
CMP AX; BX
JGE 
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
CMP AX; BX
JNE if3
PRINTN "*"
PRINTN "-"
if3:
MOV AX, 1
PUSH AX
POP, AX
PRINTN "
"
finFor1
MOV AX, 0
PUSH AX
POP AX
MOVkAX
PRINTN "-"
MOV AX, 2
PUSH AX
POP, AX
MOV AX, 2
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
POP AX
POP BX
CMP AX; BX
JGE 
PRINTN "
"
PRINTN "
Error: la altura debe de ser mayor que 2
"
if1:
MOV AX, 1
PUSH AX
MOV AX, 1
PUSH AX
POP AX
POP BX
CMP AX; BX
JE if4
PRINTN "Esto no se debe imprimir"
MOV AX, 2
PUSH AX
MOV AX, 2
PUSH AX
POP AX
POP BX
CMP AX; BX
JNE if5
PRINTN "Esto tampoco"
if5:
if4:
MOV AX, 258
PUSH AX
POP AX
MOVaAX
PRINTN "Valor de variable int 'a' antes del casteo: "
POP AX
POP AX
POP AX
MOVyAX
PRINTN "
Valor de variable char 'y' despues del casteo de a: "
POP AX
PRINTN "
A continuacion se intenta asignar un int a un char sin usar casteo: 
"
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
