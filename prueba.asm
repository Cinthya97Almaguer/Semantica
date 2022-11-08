;Archivo: prueba.cpp
;Fecha 07/11/2022 11:23:38 p. m.
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
MOV i , AX
MOViAX
MOV AX, 0
PUSH AX
POP AX
POP BX
CMP AX; BX
JLE 
MOV AX, 1
PUSH AX
MOV AX, 0
PUSH AX
POP AX
MOV j , AX
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
