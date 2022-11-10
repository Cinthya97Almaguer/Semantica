//BRIONES ALMAGUER CINTHYA CRISTINA
using System;
using System.Collections.Generic;
/*Requerimiento 1.- ACTUALIZACION:
X                   a) Agregar el residuo en la division en el porfactor 
X                   b) Agregar en Instruccion los incrementos de termino y factor 
                       a++,a--,a+=1,a-=1,a*=1,a/=1;a%=1 [a+=(5+8)] 
                       en donde el uno puede ser cualquier numero o una expresion
X                   c) Programar el destructor para ejecutar el metodo cerrarArchivo 
                       Existe una libreria especial para esto, trabajar en Lexico??
                    
    Requerimiento 2.- Actualizacion parte 2
X                   a) Marcar errores semanticos cuando los incrementos de termino o de factor
                       superen el rango de la variable char c=255, c++; error semantico
X                   b) Considerar el inciso b y c para el For.
X                   c) Funcione el Do y el While.
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
            cIf = cFor = cWhile = cDoWhile = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = cWhile = cDoWhile = 0;
        }

        public void Dispose()
        {
            //https://learn.microsoft.com/es-es/dotnet/visual-basic/programming-guide/language-features/objects-and-classes/object-lifetime-how-objects-are-created-and-destroyed
            // Dispose of unmanaged resources.
            ///Dispose(true);
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
                //asm.WriteLine("\t" + v.getNombre()); // + " DW ?"); + v.getTipoDato() + " " + v.getValor());
                switch (v.getTipoDato())
                {
                    case Variable.TipoDato.Int:
                        asm.WriteLine("\t" + v.getNombre() + " DW "+ v.getValor());
                        break;
                    /*case Variable.TipoDato.Float:
                        asm.WriteLine("DD ?");
                        break;*/
                    case Variable.TipoDato.Char:
                        asm.WriteLine("\t" + v.getNombre() + " DB "+ v.getValor());
                        break;
                    default:
                        asm.WriteLine("\t" + v.getNombre() + " DD "+ v.getValor());
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
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
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
        private void Asignacion(bool evaluacion)
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
                modVariable(nombreVariable, Incremento(evaluacion, nombreVariable));
                asm.WriteLine(incrementoEMU);
                match(";");
                //REQUERIMITNO 1.c)
            }
            else
            {
                log.WriteLine();
                log.Write(getContenido() + " = ");
                match(Tipos.Asignacion);
                Expresion();
                match(";");
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
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
                if (getTipo(nombreVariable) == Variable.TipoDato.Char)
                {
                    asm.WriteLine("MOV AH, 0");
                }
                asm.WriteLine("MOV" + nombreVariable + "AX");
            }
        }

        //AQUIESTAWHILE
        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            string etiquetaInicioWhile = "inicioWhile" + cWhile;
            string etiquetaFinWhile = "finWhile" + cWhile;
            asm.WriteLine(etiquetaInicioWhile + ":");
            bool validar;
            int guardarPosicion = posicion;
            int guardarLinea = linea;
            int tamano = getContenido().Length;
            do
            {
                validar = Condicion("");
                asm.WriteLine(etiquetaInicioWhile);
                if (!evaluacion)
                {
                    validar = false;
                }
                match(")");
                if (getContenido() == "{")
                {
                    if (validar)
                    {
                        BloqueInstrucciones(evaluacion);
                    }
                    else
                    {
                        BloqueInstrucciones(false);
                    }
                    //BloqueInstrucciones(validar);
                }
                else
                {
                    if (validar)
                    {
                        Instruccion(evaluacion);
                    }
                    else
                    {
                        Instruccion(false);
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
                asm.WriteLine("JMP " + etiquetaInicioWhile);
                asm.WriteLine(etiquetaFinWhile + ":");
            } while (validar);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validar = evaluacion;
            string etiquetaInicioDoWhile = "inicioWhile" + cDoWhile;
            string etiquetaFinDoWhile = "finWhile" + cDoWhile;
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
                asm.WriteLine(etiquetaInicioDoWhile + ":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
                match("while");
                match("(");
                //variable = getContenido();
                validar = Condicion("");
                if (evaluacion == false)
                {
                    validar = false;
                }else if(validar)
                {
                    posicion = guardarPosicion - 1;
                    linea = guardarLinea;
                    restablecerPosicion(posicion);
                    NextToken();
                }
                asm.WriteLine("JMP " + etiquetaInicioDoWhile);
                asm.WriteLine(etiquetaFinDoWhile + ":");
            } while (validar);

            match(")");
            match(";");
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor;
            
            match("for");
            match("(");
            Asignacion(evaluacion);

            float valor;
            bool validar;
            int guardarPosicion = posicion;
            int guardarLinea = linea;
            int tamano = getContenido().Length;
            string nombreVariable = getContenido();
            do
            {
                validar = Condicion("");
                asm.WriteLine(etiquetaInicioFor + ":");
                if (!evaluacion)
                {
                    validar = false;
                }
                match(";");
                match(Tipos.Identificador);

                valor = Incremento(evaluacion, nombreVariable);

                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validar);
                }
                else
                {
                    Instruccion(validar);
                }
                if (validar)
                {
                    posicion = guardarPosicion - tamano;
                    linea = guardarLinea;
                    restablecerPosicion(posicion);
                    NextToken();
                    modVariable(getContenido(), valor);
                }
                asm.WriteLine(incrementoEMU);
                asm.WriteLine("JMP " + etiquetaInicioFor);
                asm.WriteLine(etiquetaFinFor + ":");
            } while (validar);
            //asm.WriteLine(etiquetaFinFor +":");
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
        private float Incremento(bool evaluacion, string variable)
        {
            string nombreVariable = getContenido();
            //match(Tipos.Identificador);
            Dominante = Variable.TipoDato.Char;
            float nuevoValor = 0;
            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {

                string operador = getContenido();
                float valor = getValor(variable);
                switch (operador)
                {
                    case "++":
                        match("++");
                        incrementoEMU = "INC " + variable;
                        if (evaluacion)
                        {
                            nuevoValor = valor + 1;
                        }
                        break;
                    case "--":
                        match("--");
                        incrementoEMU = "DEC " + variable;
                        if (evaluacion)
                        {
                            nuevoValor = valor - 1;
                        }
                        break;
                    case "+=":
                        match("+=");
                        Expresion();
                        if (evaluacion)
                        {
                            incrementoEMU = "POP AX";
                            incrementoEMU += "ADD" + variable + ", AX";
                            nuevoValor = valor + stack.Pop();
                        }
                        break;
                    case "-=":
                        match("-=");
                        Expresion();
                        if (evaluacion)
                        {
                            incrementoEMU = "POP AX";
                            incrementoEMU += "SUB" + variable + ", AX";
                            nuevoValor = valor - stack.Pop();
                        }
                        break;
                    case "*=":
                        match("*=");
                        Expresion();
                        if (evaluacion)
                        {
                            incrementoEMU = "POP AX";
                            incrementoEMU += "MOV BX," + variable;
                            incrementoEMU += "MUL BX";
                            incrementoEMU += "MOV " + variable + ", AX";
                            nuevoValor = valor * stack.Pop();
                        }
                        break;
                    case "/=":
                        match("/=");
                        Expresion();
                        if (evaluacion)
                        {
                            incrementoEMU = "POP AX";
                            incrementoEMU += "MOV BX," + variable;
                            incrementoEMU += "DIV BX";
                            incrementoEMU += "MOV " + variable + ", AX";
                            nuevoValor = valor / stack.Pop();
                        }
                        break;
                    case "%=":
                        match("%=");
                        Expresion();
                        if (evaluacion)
                        {
                            incrementoEMU = "POP AX";
                            incrementoEMU += "MOV AX," + variable;
                            incrementoEMU += "DIV BX";
                            incrementoEMU += "MOV " + variable + ", DX";

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
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("POP AX");
            float e1 = stack.Pop();
            asm.WriteLine("POP BX");
            asm.WriteLine("CMP AX; BX");
            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if" + ++cIf;
            string etiquetaElse = "else" + cIf;
            match("if");
            match("(");
            bool validar = Condicion(etiquetaIf);
            if (evaluacion == false)
            {
                validar = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validar);
            }
            else
            {
                Instruccion(validar);
            }
            asm.WriteLine("JMP " + etiquetaElse);
            asm.WriteLine(etiquetaIf + ":");
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    if (evaluacion == true)
                    {
                        BloqueInstrucciones(!validar);
                    }
                    else
                    {
                        BloqueInstrucciones(false);
                    }
                }
                else
                {
                    if (evaluacion == true)
                    {
                        Instruccion(!validar);
                    }
                    else
                    {
                        Instruccion(false);
                    }
                }
            }
            asm.WriteLine(etiquetaElse  + ":");
        }

        //Printf -> printf(cadena o expreción);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                setContenido(getContenido().Replace("\"", ""));
                setContenido(getContenido().Replace("\\n", "\n"));
                setContenido(getContenido().Replace("\\t", "     "));
                if (evaluacion)
                {
                    Console.Write(getContenido());
                }
                asm.WriteLine("PRINTN \"" + getContenido() + "\"");
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                if (evaluacion)
                {
                    //REQUERIMIENTO DE PRINTF CODIGO ENSAMBLADOR PARA IMPRIMIR UNA VARIABLE
                    Console.Write(resultado);
                    asm.WriteLine("CALL PRINT_NUM");
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena,&identificador);
        private void Scanf(bool evaluacion)
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
                asm.WriteLine("CALL scan_num");
                asm.WriteLine("MOV " + getContenido() + ", CX");

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
            BloqueInstrucciones(true);
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                //REQUERIMIENTO 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        //DIVIDE LO QUE HAY EN AX ENTRE BX
                        //EL RESIDUO DE LA DIVISION SE QUEDA EN DX
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    //*************************** REQUERIMIENTO 1A ******************************** 
                    case "%":
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");
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
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (Dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    Dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX, " + getContenido());
                asm.WriteLine("PUSH AX");
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
                asm.WriteLine("PUSH AX");
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
                Expresion();
                match(")");
                if (huboCasteo)
                {
                    float valorCasteo = stack.Pop();
                    asm.WriteLine("POP AX");
                    valorCasteo = Convertir(valorCasteo, casteo);
                    Dominante = casteo;
                    stack.Push(valorCasteo);
                }
            }
        }
    }
}