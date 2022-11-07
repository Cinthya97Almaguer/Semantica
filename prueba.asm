;Archivo: prueba.cpp
;Fecha 06/11/2022 09:58:44 p. m.
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
JGE if1
inicioFor0
POP AX
MOViAX
MOV AX, 0
PUSH AX
POP AX
POP BX
CMP AX; BX
JLE 
