//BRIONES ALMAGUER CINTHYA CRISTINA
using System;
using System.Collections.Generic;
/*Requerimiento 1.- ACTUALIZACION:
X                   a) Agregar el residuo en la division en el porfactor 
X                   b) Agregar en Instruccion los incrementos de termino y factor 
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
                    a)  Programar el else en ensamblador
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
        int cIf, cFor;
        public Lenguaje()
        {
            cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = 0;
        }

        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();
        }

        //PASAMOS EL NOMBRE Y TIPO DE DATO Y QUE LO AGREGUE A LA LISTA
        private void addVariable(string nombre, Variable.TipoDato tipo)
        {
            //AGREGAMOS A LA LISTA UNA NUEVA VARIABLE
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables( )
        {
            log.WriteLine();
            log.WriteLine("Variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipoDato() + " " + v.getValor());
            }
        }

        private void Variablesasm( )
        {
            asm.WriteLine();
            asm.WriteLine(";Variables: ");
            foreach (Variable v in variables)
            {
                asm.WriteLine("\t"+v.getNombre() + " DW ?" ); //+ v.getTipoDato() + " " + v.getValor());
            }
        }

        private bool existeVariable (string nombre) //SABER SI EXISTE LA VARIABLE
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                    return true;
            }
            return false;
        }

        private void modVariable (string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }

        private float getValor (string nombreVariable)
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

        private Variable.TipoDato getTipo (string nombreVariable)
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
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() +"> en linea: "+linea, log);
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
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
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
            else if(getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion);
            }
            else if(getContenido() == "switch")
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
                throw new Error("\n2Error de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
            }
            log.WriteLine();
            //log.Write(getContenido()+" = ");
            match(Tipos.Identificador);  
            Dominante = Variable.TipoDato.Char;
            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                /*REQUERIMITNO 1.b)
                b) Agregar en Instruccion los incrementos de termino y factor 
                       a++,a--,a+=1,a-=1,a*=1,a/=1;a%=1 [a+=(5+8)] 
                       en donde el uno puede ser cualquier numero o una expresion*/
                string operador = getContenido();
                float valor;
                switch (operador)
                {
                    case "++": 
                        Incremento(evaluacion, nombreVariable);
                        break;
                    case "--": 
                        Incremento(evaluacion, nombreVariable); 
                        break;
                    case "+=":
                        match(Tipos.IncrementoTermino);
                        Expresion();
                        valor = getValor(nombreVariable) + stack.Pop();
                        asm.WriteLine("POP, AX");
                        if (Dominante < evaluaNumero(valor))
                        {
                            Dominante = evaluaNumero(valor);
                        }
                        if (Dominante <= getTipo(nombreVariable))
                        {
                            if (evaluacion)
                            {
                                modVariable(nombreVariable, valor);
                            }
                        }
                        else
                        {
                            throw new Error("\nError de semantica no podemos asignar un  < " + Dominante + " > a un " + getTipo(nombreVariable) + " en la linea " + linea, log);
                        }
                        modVariable(nombreVariable, valor);
                        break;
                    case "-=": 
                        match(Tipos.IncrementoTermino);
                        Expresion();
                        valor = getValor(nombreVariable) - stack.Pop();
                        asm.WriteLine("POP, AX");
                        modVariable(nombreVariable, valor);
                        break;
                    case "*=": 
                        match(Tipos.IncrementoFactor);
                        Expresion();
                        valor = getValor(nombreVariable) * stack.Pop();
                        asm.WriteLine("POP, AX");
                        if (Dominante < evaluaNumero(valor))
                        {
                            Dominante = evaluaNumero(valor);
                        }
                        if (Dominante <= getTipo(nombreVariable))
                        {
                            if (evaluacion)
                            {
                                modVariable(nombreVariable, valor);
                            }
                        }
                        else
                        {
                            throw new Error("\nError de semantica no podemos asignar un  < " + Dominante + " > a un " + getTipo(nombreVariable) + " en la linea " + linea, log);
                        }
                        modVariable(nombreVariable, valor);
                        break;
                    case "/=": 
                        match(Tipos.IncrementoFactor);
                        Expresion();
                        valor = getValor(nombreVariable) / stack.Pop();
                        asm.WriteLine("POP, AX");
                        modVariable(nombreVariable, valor);
                        break;
                    case "%=": 
                        match(Tipos.IncrementoTermino);
                        Expresion();
                        valor = getValor(nombreVariable) % stack.Pop();
                        asm.WriteLine("POP, AX");
                        modVariable(nombreVariable, valor);
                        break;
                }
                match(Tipos.FinSentencia);       
                //REQUERIMITNO 1.c)
            }           
            else
            {
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
                        //asm.WriteLine("MOV "+nombreVariable+" , AX");
                    }
                }
                else
                {
                    throw new Error("\nError de semantica no podemos asignar un  < " + Dominante + " > a un " + getTipo(nombreVariable) + " en la linea " + linea, log);
                }
                asm.WriteLine("MOV"+nombreVariable+"AX");
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            bool validar = Condicion("");
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
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validar = true;
            if (evaluacion == false)
            {
                validar = false;
            }
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validar);
            }
            else
            {
                Instruccion(validar);
            } 
            match("while");
            match("(");
            validar = Condicion("");
            match(")");
            match(";");
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            string nombreVariable = getContenido();
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + ++cFor;
            asm.WriteLine(etiquetaInicioFor);
            match("for");
            match("(");
            Asignacion(evaluacion);
            bool validar = true;
            int guardarPosicion = posicion;
            int guardarLinea = linea;
            int tamano = getContenido().Length;
            while(validar)
            {
                validar = Condicion("");
                if (evaluacion == false)
                {
                    validar = false;
                }   
                match(";");
                string nombreIncremento = getContenido();
                float valorIncremento = incrementarFor(validar);
                //REQUERIMIENTO 1.d
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validar);
                }
                else
                {
                    Instruccion(validar);
                }
                if(validar)
                {
                    posicion = guardarPosicion - tamano;
                    linea = guardarLinea;
                    restablecerPosicion(posicion);
                    NextToken();
                    modVariable(nombreIncremento, valorIncremento);
                }
            }
            asm.WriteLine(etiquetaFinFor);
        }

        private void restablecerPosicion(int posicion)
        {
            //https://ajaxhispano.com/ask/volver-streamreader-al-principio-40180/
            //restablecer el búfer interno del objeto StreamReader
            archivo.DiscardBufferedData();
            //se establece la posición dentro de la secuencia actual.
            archivo.BaseStream.Position = posicion;
            //archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }

        private float incrementarFor(bool evaluacion)
        {
            string variable = getContenido();
            if (!existeVariable(variable))
            {
                throw new Error("\nError de sintaxis for la variable " + variable + " no existe en la linea " + linea, log);
            }
            match(Tipos.Identificador);
            Dominante = Variable.TipoDato.Char;
            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                string operador = getContenido();
                float valor;
                switch (operador)
                {
                    case "++":
                        Incremento(evaluacion, variable);
                        break;
                    case "--": 
                        Incremento(evaluacion, variable);
                        break;
                    case "+=":
                        match(Tipos.IncrementoTermino);
                        Expresion();
                        valor = getValor(variable) + stack.Pop();
                        if (Dominante < evaluaNumero(valor))
                        {
                            Dominante = evaluaNumero(valor);
                        }
                        if (Dominante <= getTipo(variable))
                        {
                            if (evaluacion)
                            {
                                modVariable(variable, valor);
                            }
                        }
                        else
                        {
                            throw new Error("\nError de semantica no podemos asignar un  < " + Dominante + " > a un " + getTipo(variable) + " en la linea " + linea, log);
                        }
                        modVariable(variable, valor);
                        break;
                    case "-=": 
                        match(Tipos.IncrementoTermino);
                        Expresion();
                        valor = getValor(variable) - stack.Pop();
                        modVariable(variable, valor);
                        break;
                    case "*=": 
                        match(Tipos.IncrementoFactor);
                        Expresion();
                        valor = getValor(variable) * stack.Pop();
                        if (Dominante < evaluaNumero(valor))
                        {
                            Dominante = evaluaNumero(valor);
                        }
                        if (Dominante <= getTipo(variable))
                        {
                            if (evaluacion)
                            {
                                modVariable(variable, valor);
                            }
                        }
                        else
                        {
                            throw new Error("\nError de semantica no podemos asignar un  < " + Dominante + " > a un " + getTipo(variable) + " en la linea " + linea, log);
                        }
                        modVariable(variable, valor);
                        break;
                    case "/=": 
                        match(Tipos.IncrementoFactor);
                        Expresion();
                        valor = getValor(variable) / stack.Pop();
                        modVariable(variable, valor);
                        break;
                    case "%=": 
                        match(Tipos.IncrementoTermino);
                        Expresion();
                        valor = getValor(variable) % stack.Pop();
                        modVariable(variable, valor);
                        break;
                }
            }
            /*if (getContenido() == "++")
            {
                match("++");
                if (evaluacion)
                {
                    return getValor(variable) + 1;
                }
            }
            else if (getContenido() == "--")
            {
                match("--");
                if (evaluacion)
                {
                    return getValor(variable) - 1;
                }
            }*/
            
            return 0;            
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion, string variable)
        {
            if (!existeVariable(variable))
            {
                throw new Error("\nError la variable <" + getContenido() + "> no existe en linea: "+linea, log);
            }
            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
//************************************************REQUERIMIENTO 2A ********************************************                
                if (evaluacion)
                {
                    modVariable(variable, getValor(variable)+1);
                    if(Dominante < evaluaNumero(getValor(variable) + 1))
                    {
                        Dominante = evaluaNumero(getValor(variable) + 1);
                    }
                    if(Dominante <= getTipo(variable))
                    {
                        modVariable(variable, getValor(variable) + 1);
                    }
                    else
                    {
                        throw new Error("\nError de semantica no podemos asignar un  < " + Dominante + " > a un " + getTipo(variable) + " en la linea " + linea, log);
                    }
                }
                match("++");
            }
            else
            {
                if(evaluacion)
                {
                    modVariable(variable, getValor(variable)-1);
                }
                match("--");
            }
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
            if(getContenido() == "default")
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
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
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
                    asm.WriteLine("JNE "+ etiqueta);
                    return e1 == e2;
                case ">":
                    asm.WriteLine("JLE "+ etiqueta);
                    return e1 > e2;
                case ">=":
                    asm.WriteLine("JL "+ etiqueta);
                    return e1 >= e2;
                case "<":
                    asm.WriteLine("JGE "+ etiqueta);
                    return e1 < e2;
                case "<=":
                    asm.WriteLine("JG "+ etiqueta);
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE "+ etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if" + ++cIf;
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
            asm.WriteLine(etiquetaIf + ":");
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
                if (evaluacion){
                    Console.Write(getContenido());
                }
                asm.WriteLine("PRINTN \""+getContenido()+"\"");
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
                throw new Error("\nError la variable <" + getContenido() +"> no existe en linea: "+linea, log);
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
                    throw new Error("ERROR no se puede asignar <"+ getContenido() +">  en linea: "+linea, log);
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
                asm.WriteLine("MOV AX, "+getContenido());
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
                if (Dominante  < getTipo(getContenido()))
                {
                    Dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                //REQUERIMIENTO 3
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