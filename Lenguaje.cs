//BRIONES ALMAGUER CINTHYA CRISTINA
using System;
using System.Collections.Generic;
/*Requerimiento 1.- ACTUALIZACION:
                    a) Agregar el residuo en la division en el porfactor 
                    b) Agregar en Instruccion los incrementos de termino y factor 
                       a++,a--,a+=1,a-=1,a*=1,a/=1;a%=1 [a+=(5+8)] 
                       en donde el uno puede ser cualquier numero o una expresion
                    c) Programar el destructor para ejecutar el metodo cerrarArchivo 
                       Existe una libreria especial para esto, trabajar en Lexico??
                    
    Requerimiento 2.- Actualizacion parte 2
                    a) Marcar errores semanticos cuando los incrementos de termino o de factor
                       superen el rango de la variable char c=255, c++; error semantico
                    b) Considerar el inciso b y c para el For.
                    c) Funcione el Do y el While.
    Requerimiento 3.- 
                    a) Considerar las variables y los casteo de las expresiones matematicas
                       en ensamblador
                    b) Considerar el residuo de la division en ensamblador
                    c) Programar el Printf y el Scanf en ensamblador
    Requerimiento 4.-                 
                    a) Programar el else en ensamblador
                    b) Programar el for en ensamblador
    Requerimiento 5.-                 
                    a) Programar el while en ensamblador
                    b) Programar el do() while en ensamblador

*/

namespace Semantica
{

    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato Dominante;
        int cIf, cFor, cWhile, cDoWhile;
        string incrementoEMU = "";
        public Lenguaje()
        {
            cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = 0;
        }

        public void Dispose()
        {
            //https://learn.microsoft.com/es-es/dotnet/visual-basic/programming-guide/language-features/objects-and-classes/object-lifetime-how-objects-are-created-and-destroyed
            // Dispose of unmanaged resources.
            // Dispose(true);
            // Suppress finalization.
            Console.WriteLine("\nDestructor");
            cerrar();
            GC.SuppressFinalize(this);
        }

        ~Lenguaje()
        {
            /*Console.WriteLine("Destructor");
            cerrar();*/
            Dispose();
        }

        //PASAMOS EL NOMBRE Y TIPO DE DATO Y QUE LO AGREGUE A LA LISTA
        private void addVariable(string nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("Variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipoDato() + " " + v.getValor());
            }
        }

        private void Variablesasm()
        {
            asm.WriteLine();
            asm.WriteLine(";Variables: ");
            foreach (Variable v in variables)
            {
                //asm.WriteLine("\t" + v.getNombre() + " DW ?"); //+ v.getTipoDato() + " " + v.getValor());
                switch (v.getTipoDato())
                {
                    case Variable.TipoDato.Int:
                        asm.WriteLine("\t" + v.getNombre() + " DW " + v.getValor());
                        break;
                    case Variable.TipoDato.Char:
                        asm.WriteLine("\t" + v.getNombre() + " DB " + v.getValor());
                        break;
                    default:
                        asm.WriteLine("\t" + v.getNombre() + " DD " + v.getValor());
                        break;
                }
            }
        }

        private bool existeVariable(string nombre) //SABER SI EXISTE LA VARIABLE
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                    return true;
            }
            return false;
        }

        private void modVariable(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }

        private float getValor(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombreVariable)
                {
                    return v.getValor();
                }
            }
            return 0;
        }

        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombreVariable)
                {
                    return v.getTipoDato();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("Include 'emu8086.inc'");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            Variablesasm();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
            asm.WriteLine("END");
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool EMU)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, EMU);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool EMU)
        {
            Instruccion(evaluacion, EMU);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, EMU);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool EMU)
        {
            Instruccion(evaluacion, EMU);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, EMU);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool EMU)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion, EMU);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion, EMU);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion, EMU);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion, EMU);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion, EMU);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion, EMU);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion, EMU);
            }
            else
            {
                Asignacion(evaluacion, EMU);
            }
        }

        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <= 256)
            {
                return Variable.TipoDato.Char;
            }
            else
            {
                if (resultado <= 65536)
                {
                    return Variable.TipoDato.Int;
                }
            }
            return Variable.TipoDato.Float;
        }

        private bool evaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }


        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion, bool EMU)
        {
            string nombreVariable = getContenido();
            if (!existeVariable(nombreVariable))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
            }

            match(Tipos.Identificador);

            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                //REQUERIMITNO 1.b)
                Variable.TipoDato tipoDato = getTipo(nombreVariable);
                float nuevoValor = getValor(nombreVariable);
                Dominante = Variable.TipoDato.Char;
                modVariable(nombreVariable, Incremento(evaluacion, nombreVariable, EMU));
                //asm.WriteLine(incrementoEMU);
                if (EMU)
                {
                    asm.WriteLine(incrementoEMU);
                }
                match(";");
                //REQUERIMITNO 1.c)
            }
            else
            {
                log.WriteLine();
                log.Write(getContenido() + " = ");
                match(Tipos.Asignacion);
                Expresion(EMU);
                match(";");
                float resultado = stack.Pop();
                if (EMU)
                {
                    asm.WriteLine("POP AX");
                }
                //asm.WriteLine("POP AX");
                log.Write("= " + resultado);
                log.WriteLine();
                if (Dominante < evaluaNumero(resultado))
                {
                    Dominante = evaluaNumero(resultado);
                }
                if (Dominante <= getTipo(nombreVariable))
                {
                    if (evaluacion)
                    {
                        modVariable(nombreVariable, resultado);
                        //MODIFICACION EN ENSAMBLADOR
                        //asm.WriteLine("MOV " + nombreVariable + " , AX");
                    }
                }
                else
                {
                    throw new Error("\nError de semantica no podemos asignar un  < " + Dominante + " > a un " + getTipo(nombreVariable) + " en la linea " + linea, log);
                }
                /*if (getTipo(nombreVariable) == Variable.TipoDato.Char)
                {
                    if(EMU)
                    {
                        asm.WriteLine("MOV AH, 0");
                    }
                }*/
                if(EMU)
                {
                    switch (getTipo(nombreVariable))
                    {
                        case Variable.TipoDato.Char:
                            asm.WriteLine("MOV AH, 0");
                            break;
                        case Variable.TipoDato.Int:
                            asm.WriteLine("MOV " + nombreVariable + " , AX");
                            break;
                        case Variable.TipoDato.Float:
                            asm.WriteLine("MOV " + nombreVariable + " , AX");
                            break;
                    }
                    //asm.WriteLine("MOV " + nombreVariable + " , AX");
                }
            }
        }

        //AQUIESTAWHILE
        //While -> while(Condicion) bloque de instrucciones | instruccion
        //SE TIENE QUE VALIDAR A EMU EN ENSAMBLADOR
        private void While(bool evaluacion, bool EMU)
        {
            if(EMU)
            {
                cWhile++;
            }
            match("while");
            match("(");
            string etiquetaInicioWhile = "inicioWhile" + cWhile;
            string etiquetaFinWhile = "finWhile" + cWhile;
            bool validar;
            int guardarPosicion = posicion;
            int guardarLinea = linea;
            int tamano = getContenido().Length;
            do
            {
                if(EMU)
                {
                    asm.WriteLine(etiquetaInicioWhile + ":" );
                }
                validar = Condicion(etiquetaFinWhile,EMU);
                //asm.WriteLine(etiquetaInicioWhile);
                if (!evaluacion)
                {
                    validar = false;
                }
                match(")");
                if (getContenido() == "{")
                {
                    if (validar)
                    {
                        BloqueInstrucciones(evaluacion, EMU);
                    }
                    else
                    {
                        BloqueInstrucciones(false, EMU);
                    }
                    //BloqueInstrucciones(validar);
                }
                else
                {
                    if (validar)
                    {
                        Instruccion(evaluacion, EMU);
                    }
                    else
                    {
                        Instruccion(false, EMU);
                    }
                    //Instruccion(validar);
                }
                if (validar)
                {
                    posicion = guardarPosicion - tamano;
                    linea = guardarLinea;
                    restablecerPosicion(posicion);
                    NextToken();
                }
                if(EMU)
                {
                    asm.WriteLine("JMP " + etiquetaInicioWhile+ ":");
                    asm.WriteLine(etiquetaFinWhile + ":");
                }
                //asm.WriteLine("JMP " + etiquetaInicioWhile);
                //asm.WriteLine(etiquetaFinWhile + ":");
                EMU = false;
            } while (validar);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        // VALIDAR A EMU EN ENSAMBLADOR
        private void Do(bool evaluacion, bool EMU)
        {
            if (EMU)
            {
                cDoWhile++;
            }
            bool validar = evaluacion;
            string etiquetaInicioDoWhile = "inicioDoWhile" + cDoWhile;
            string etiquetaFinDoWhile = "finDoWhile" + cDoWhile;
            
            //asm.WriteLine(etiquetaInicioDoWhile + ":");
            if (evaluacion == false)
            {
                validar = false;
            }
            match("do");
            int guardarPosicion = posicion;
            int guardarLinea = linea;
            do
            {
                if (EMU)
                {
                    asm.WriteLine(etiquetaInicioDoWhile + ":");
                }
                //asm.WriteLine(etiquetaInicioDoWhile + ":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, EMU);
                }
                else
                {
                    Instruccion(evaluacion, EMU);
                }
                match("while");
                match("(");
                //variable = getContenido();
                validar = Condicion(etiquetaFinDoWhile, EMU);
                if (evaluacion == false)
                {
                    validar = false;
                }
                else if (validar)
                {
                    posicion = guardarPosicion - 1;
                    linea = guardarLinea;
                    restablecerPosicion(posicion);
                    NextToken();
                }
                if (EMU)
                {
                    asm.WriteLine("JMP " + etiquetaInicioDoWhile + ":");
                    asm.WriteLine(etiquetaFinDoWhile + ":");
                }
                //asm.WriteLine("JMP " + etiquetaInicioDoWhile);
                //asm.WriteLine(etiquetaFinDoWhile + ":");
                EMU = false;
            } while (validar);

            match(")");
            match(";");
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool EMU)
        {
            if (EMU)
            {
                cFor++;
            }
            string etiquetaIniciarFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor;

            match("for");
            match("(");
            Asignacion(evaluacion, EMU);

            float valor;
            bool validar;
            int guardarPosicion = posicion;
            int guardarLinea = linea;
            int tamano = getContenido().Length;
            string nombreVariable = getContenido();
            do
            {
                if (EMU)
                {
                    asm.WriteLine(etiquetaIniciarFor + ":");
                }
                validar = Condicion(etiquetaFinFor, EMU);
                if (!evaluacion)
                {
                    validar = false;
                }
                match(";");
                match(Tipos.Identificador);

                valor = Incremento(evaluacion, nombreVariable, EMU);

                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validar, EMU);
                }
                else
                {
                    Instruccion(validar, EMU);
                }
                if (validar)
                {
                    posicion = guardarPosicion - tamano;
                    linea = guardarLinea;
                    restablecerPosicion(posicion);
                    NextToken();
                    modVariable(getContenido(), valor);
                }
                if (EMU)
                {
                    asm.WriteLine(incrementoEMU);
                    asm.WriteLine("JMP " + etiquetaIniciarFor);
                    asm.WriteLine(etiquetaFinFor + ":");
                }
                EMU = false;
            } while (validar);
        }

        private void restablecerPosicion(int posicion)
        {
            //https://ajaxhispano.com/ask/volver-streamreader-al-principio-40180/
            //restablecer el búfer interno del objeto StreamReader
            archivo.DiscardBufferedData();
            //se establece la posición dentro de la secuencia actual.
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }

        //Incremento -> Identificador ++ | --
        private float Incremento(bool evaluacion, string variable, bool EMU)
        {
            string nombreVariable = getContenido();
            //match(Tipos.Identificador);
            /*if (!existeVariable(nombreVariable))
            {
                throw new Exception("Error en la linea " + linea + " la variable " + nombreVariable + " no existe");
            }*/
            float valor = getValor(variable);
            Variable.TipoDato tipoDato = getTipo(variable);
            Dominante = Variable.TipoDato.Char;
            float nuevoValor = 0;
            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                string operador = getContenido();
                
                switch (operador)
                {
                    case "++":
                        match("++");
                        if (EMU)
                        {
                            incrementoEMU = "INC " + variable;
                        }
                        //incrementoEMU = "INC " + variable;
                        if (evaluacion)
                        {
                            nuevoValor = valor + 1;
                        }
                        break;
                    case "--":
                        match("--");
                        if (EMU)
                        {
                            incrementoEMU = "DEC " + variable;
                        }
                        //incrementoEMU = "DEC " + variable;
                        if (evaluacion)
                        {
                            nuevoValor = valor - 1;
                        }
                        break;
                    case "+=":
                        match("+=");
                        Expresion(EMU);
                        if (EMU)
                        {
                            incrementoEMU = "POP AX ";
                            incrementoEMU += "\nADD " + variable + ", AX";
                        }
                        //incrementoEMU = "POP AX";
                        //incrementoEMU += "ADD" + variable + ", AX";
                        if (evaluacion)
                        {

                            nuevoValor = valor + stack.Pop();
                        }
                        break;
                    case "-=":
                        match("-=");
                        Expresion(EMU);
                        if (EMU)
                        {
                            incrementoEMU = "POP AX ";
                            incrementoEMU += "\nSUB" + variable + ", AX";
                        }
                        //incrementoEMU = "POP AX";
                        //incrementoEMU += "SUB" + variable + ", AX";
                        if (evaluacion)
                        {
                            nuevoValor = valor - stack.Pop();
                        }
                        break;
                    case "*=":
                        match("*=");
                        Expresion(EMU);
                        if (EMU)
                        {
                            incrementoEMU = "POP AX ";
                            incrementoEMU += "\nMOV BX" + variable;
                            incrementoEMU += "\nMUL BX";
                            incrementoEMU += "\nMOV " + variable + ", AX";
                        }
                        //incrementoEMU = "POP AX";
                        //incrementoEMU += "MOV BX," + variable;
                        //incrementoEMU += "MUL BX";
                        //incrementoEMU += "MOV " + variable + ", AX";
                        if (evaluacion)
                        {

                            nuevoValor = valor * stack.Pop();
                        }
                        break;
                    case "/=":
                        match("/=");
//divide primero el valor de la variable (a la izquierda del operador)
// entre el valor de la expresión (a la derecha del operador) a=12 b=3 a/=b ---
                
                        Expresion(EMU);
                        if (EMU)
                        {
                            incrementoEMU = "POP AX ";
                            incrementoEMU += "\nMOV AX" + variable;
                            incrementoEMU += "\nDIV BX";
                            incrementoEMU += "\nMOV " + variable + ", AX";
                        }
                        //incrementoEMU = "POP AX";
                        //incrementoEMU += "MOV BX," + variable;
                        //incrementoEMU += "DIV BX";
                        //incrementoEMU += "MOV " + variable + ", AX";
                        if (evaluacion)
                        {
                            nuevoValor = valor / stack.Pop();
                        }
                        break;
                    case "%=":
                        match("%=");
                        Expresion(EMU);
                        if (EMU)
                        {
                            incrementoEMU = "POP AX ";
                            incrementoEMU += "\nMOV AX," + variable;
                            incrementoEMU += "\nDIV BX";
                            incrementoEMU += "\nMOV " + variable + ", DX";
                        }
                        //incrementoEMU = "POP AX";
                        //incrementoEMU += "MOV AX," + variable;
                        //incrementoEMU += "DIV BX";
                        //incrementoEMU += "MOV " + variable + ", DX";
                        if (evaluacion)
                        {
                            nuevoValor = valor % stack.Pop();
                        }
                        break;
                }
            }
            if (getTipo(variable) == Variable.TipoDato.Char && nuevoValor > 255)
            {
                throw new Error("\nError de rango en < " + variable + " > se excede el rango en la linea " + linea, log);
            }
            else if (getTipo(variable) == Variable.TipoDato.Int && nuevoValor > 65535)
            {
                throw new Error("\nError de rango en < " + variable + " > se excede el rango en la linea " + linea, log);
            }
            return nuevoValor;
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool EMU)
        {
            match("switch");
            match("(");
            Expresion(EMU);
            stack.Pop();
            if(EMU)
            {
                asm.WriteLine("POP AX");
            }
            //asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion, EMU);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, EMU);
                }
                else
                {
                    Instruccion(evaluacion, EMU);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool EMU)
        {
            match("case");
            Expresion(EMU);
            stack.Pop();
            if (EMU)
            {
                asm.WriteLine("POP AX");
            }
            //asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion, EMU);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion, EMU);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool EMU)
        {
            Expresion(EMU);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(EMU);
            float e2 = stack.Pop();
            float e1 = stack.Pop();

            if (EMU)
            {
                asm.WriteLine("POP BX");
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX, BX");
            }
            switch (operador)
            {
                case "==":
                    if (EMU)
                    {
                        asm.WriteLine("JNE " + etiqueta);
                    }
                    //asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    if (EMU)
                    {
                        asm.WriteLine("JLE " + etiqueta);
                    }
                    //asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case ">=":
                    if (EMU)
                    {
                        asm.WriteLine("JL " + etiqueta);
                    }
                    //asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<":
                    if (EMU)
                    {
                        asm.WriteLine("JGE " + etiqueta);
                    }
                    //asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case "<=":
                    if (EMU)
                    {
                        asm.WriteLine("JG " + etiqueta);
                    }
                    //asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                default:
                    if (EMU)
                    {
                        asm.WriteLine("JE " + etiqueta);
                    }
                    //asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool EMU)
        {
            if (EMU)
            {
                cIf++;
            }
            string etiquetaIf = "if" + cIf;
            string etiquetaElse = "else" + cIf;
            match("if");
            match("(");
            bool validar = Condicion(etiquetaIf, EMU);
            if (!evaluacion)
            {
                validar = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validar, EMU);
            }
            else
            {
                Instruccion(validar, EMU);
            }
            if (EMU)
            {
                asm.WriteLine("JMP " + etiquetaElse);
                asm.WriteLine(etiquetaIf + ":");
            }
            //asm.WriteLine("JMP " + etiquetaElse);
            //asm.WriteLine(etiquetaIf + ":");
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    if (evaluacion == true)
                    {
                        BloqueInstrucciones(!validar, EMU);
                    }
                    else
                    {
                        BloqueInstrucciones(false, EMU);
                    }
                }
                else
                {
                    if (evaluacion == true)
                    {
                        Instruccion(!validar, EMU);
                    }
                    else
                    {
                        Instruccion(false, EMU);
                    }
                }
            }
            if (EMU)
            {
                asm.WriteLine(etiquetaElse + ":");
            }
            //asm.WriteLine(etiquetaElse + ":");
        }

        //Printf -> printf(cadena o expreción);
        private void Printf(bool evaluacion, bool EMU)
        {
            match("printf");
            match("(");
            string aux = getContenido();
            if (getClasificacion() == Tipos.Cadena)
            {
                /*setContenido(getContenido().Replace("\"", ""));
                setContenido(getContenido().Replace("\\n", "\n"));
                setContenido(getContenido().Replace("\\t", "     "));*/
                setContenido(getContenido().Replace("\'", string.Empty));
                setContenido(getContenido().Replace("\"", string.Empty));
                setContenido(getContenido().Replace("\\t", "     "));
                string cadena = getContenido();
                setContenido(getContenido().Replace("\\n", "\n"));
                if (evaluacion)
                {
                    Console.Write(getContenido());
                }
                //asm.WriteLine("PRINTN \"" + getContenido() + "\"");
                match(Tipos.Cadena);
                if(EMU)
                {
                    if(cadena.Contains("\\n"))
                    {
                        string[] subCadena = cadena.Split("\\n");
                        for(int i=0; i<subCadena.Length; i++)
                        {
                            if(i == subCadena.Length -1)
                            {
                                asm.WriteLine("PRINT \'" + subCadena[i] + "\'");
                            }
                            else
                            {
                                asm.WriteLine("PRINTN \'" + subCadena[i] + "\'");
                            }
                        }
                    }
                    else
                    {
                        asm.WriteLine("PRINT \'" + cadena + "\'");
                    }
                }

            }
            else
            {
                Expresion(EMU);
                float resultado = stack.Pop();
                //asm.WriteLine("POP AX");
                if (evaluacion)
                {
                    //REQUERIMIENTO DE PRINTF CODIGO ENSAMBLADOR PARA IMPRIMIR UNA VARIABLE
                    Console.Write(resultado);
                    //asm.WriteLine("CALL PRINT_NUM");
                }
                if (EMU)
                {
                    asm.WriteLine("POP AX");
                    asm.WriteLine("CALL PRINT_NUM");
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena,&identificador);
        private void Scanf(bool evaluacion, bool EMU)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            string nombreVariable = getContenido();
            if (!existeVariable(nombreVariable))
            {
                throw new Error("\nError la variable <" + getContenido() + "> no existe en linea: " + linea, log);
            }
            if (evaluacion)
            {
                string val = "" + Console.ReadLine();
                try
                {
                    float nuevaVal = float.Parse(val);
                    modVariable(nombreVariable, nuevaVal);
                }
                catch (Exception)
                {
                    throw new Error("ERROR no se puede asignar <" + getContenido() + ">  en linea: " + linea, log);
                }
                if (EMU)
                {
                    asm.WriteLine("CALL SCAN_NUM");
                    asm.WriteLine("MOV " + getContenido() + ", CX");
                }
                //asm.WriteLine("CALL SCAN_NUM");
                //asm.WriteLine("MOV " + getContenido() + ", CX");

            }
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true, true);
        }

        //Expresion -> Termino MasTermino
        private void Expresion(bool EMU)
        {
            Termino(EMU);
            MasTermino(EMU);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool EMU)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(EMU);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                //asm.WriteLine("POP BX ");
                float n2 = stack.Pop();
                //asm.WriteLine("POP AX ");
                if (EMU)
                {
                    asm.WriteLine("POP BX ");
                    asm.WriteLine("POP AX ");
                }
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        if (EMU)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        //asm.WriteLine("ADD AX, BX ");
                        //asm.WriteLine("PUSH AX ");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if (EMU)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        //asm.WriteLine("SUB AX, BX");
                        //asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool EMU)
        {
            Factor(EMU);
            PorFactor(EMU);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool EMU)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(EMU);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                //asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                //asm.WriteLine("POP AX");
                if (EMU)
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                }
                //REQUERIMIENTO 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        if (EMU)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        //asm.WriteLine("MUL BX");
                        //asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        //DIVIDE LO QUE HAY EN AX ENTRE BX
                        //EL RESIDUO DE LA DIVISION SE QUEDA EN DX
                        if (EMU)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        //asm.WriteLine("DIV BX");
                        //asm.WriteLine("PUSH AX");
                        break;
                    //*************************** REQUERIMIENTO 1A ******************************** 
                    case "%":
                        stack.Push(n2 % n1);
                        if (EMU)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        //asm.WriteLine("DIV BX");
                        //asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }

        private float Convertir(float valor, Variable.TipoDato casteo)
        {
            if (casteo == Variable.TipoDato.Char)
            {
                valor = valor % 256;
                return valor;
            }
            if (casteo == Variable.TipoDato.Int)
            {
                valor = valor % 65536;
                return valor;
            }
            return valor;
        }

        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool EMU)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (Dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    Dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                if (EMU)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                //asm.WriteLine("MOV AX, " + getContenido());
                //asm.WriteLine("PUSH AX");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                string nombreVariable = getContenido();
                if (existeVariable(nombreVariable) != true)
                {
                    throw new Error("Error de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
                }
                log.Write(getContenido() + " ");
                if (Dominante < getTipo(getContenido()))
                {
                    Dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                //REQUERIMIENTO 3
                if (EMU)
                {
                    switch (getTipo(getContenido()))
                    {
                        case Variable.TipoDato.Char:
                            asm.WriteLine("MOV AL," + getContenido());
                            break;
                        case Variable.TipoDato.Int:
                            asm.WriteLine("MOV AX, " + getContenido());
                            break;
                        case Variable.TipoDato.Float:
                            asm.WriteLine("MOV AX, " + getContenido());     
                            break;
                    }
                    //asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                //asm.WriteLine("PUSH AX");
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion(EMU);
                match(")");
                if (huboCasteo)
                {
                    float valorCasteo = stack.Pop();
                    //asm.WriteLine("POP AX");
                    valorCasteo = Convertir(valorCasteo, casteo);
                    Dominante = casteo;
                    if (EMU)
                    {
                        asm.WriteLine("POP AX");
                        switch (casteo)
                        {
                            case Variable.TipoDato.Char:
                                asm.WriteLine("MOV AH, 0");
                                asm.WriteLine("PUSH AX");
                                break;
                            case Variable.TipoDato.Int:
                            case Variable.TipoDato.Float:
                                asm.WriteLine("PUSH AX");
                                break;
                        }
                        stack.Push(valorCasteo);
                    }
                }
            }
        }
    }
}