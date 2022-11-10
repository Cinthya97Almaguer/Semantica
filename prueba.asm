;Archivo: prueba.cpp
;Fecha 08/11/2022 11:10:50 p. m.
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
PRINTN "Introduce la altura de la piramide: "
CALL scan_num
MOV altura, CX
PUSH AX
MOV AX, 2
PUSH AX
POP AX
POP BX
CMP AX; BX
JLE if1
inicioFor0:
PUSH AX
POP AX
MOViAX
PUSH AX
MOV AX, 0
PUSH AX
POP AX
POP BX
CMP AX; BX
JLE 
