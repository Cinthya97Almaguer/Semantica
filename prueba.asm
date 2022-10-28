;Archivo: prueba.cpp
;Fecha 28/10/2022 09:49:00 a. m.
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
PRINTN "Ingresa el radio del circulo"
CALL scan_num
MOV radio, CX
RET
DEFINE_SCAN_NUM
END
