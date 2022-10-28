;Archivo: prueba.cpp
;Fecha 27/10/2022 08:56:39 p. m.
#make_COM#
Include emu8086.Inc
ORG 100h

;Variables: 
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	a DW ?
	d DW ?
	altura DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
inicioFor0:
MOV AX, 0
PUSH AX
POP AX
MOV i , AX
MOViAX
MOV AX, 1
PUSH AX
POP AX
POP BX
COMP AX, BX
JGE 
