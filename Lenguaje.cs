using System;
using System.Collections.Generic;
/*Requerimiento 1.- Actualizar el dominante para variables en la expreción.  Si el dominante
                    Ejemplo: float x;char y; y=x; Debe marcar error.
  Requerimiento 2.- Actualizar el dominante para el casteo y el valor de la subexpresion.
                    char x; x=(char)(255+1); esto es lo que tiene que salir x=0;
  Requerimiento 3.- Programar un metodo de converción de una valor a un tipo de dato
                    private float convert(valor float, string TipoDato){}
                    Deberan usar el reciduo de la division %255, %65535
  Requerimiento 4.- Evaluar nuevamente la condicion del If - else, while, For, Do while con 
                    respecto al parametro que reciba
  Requerimiento 5.- Levantar la excepcion cunado la captura no sea un numero
  Requerimiento 6.- Ejecutar el For();
*/
namespace Semantica
{

    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato Dominante;
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

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
            Libreria();
            Variables();
            Main();
            displayVariables();
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
                throw new Error("\nError la variable <" + getContenido() +"> no existe en linea: "+linea, log);
            }
            log.WriteLine();
            log.Write(getContenido()+" = ");
            match(Tipos.Identificador);             
            match(Tipos.Asignacion);
            Dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");
            float resultado = stack.Pop();
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
                }
            }
            else
            {
                throw new Error("\nError de semantica no podemos asignar un  <"+ Dominante+
                                    "> a un "+ getTipo(nombreVariable)+" en la linea "+linea, log);
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            //REQUERIMIENTO 4 -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
            //Condicion();
            bool validarEvaluacion = Condicion();
            if (evaluacion == false)
            {
                validarEvaluacion = false;
            }
            match(")");
            if (getContenido() == "{") 
            {
                //BloqueInstrucciones(evaluacion);
                BloqueInstrucciones(validarEvaluacion);
            }
            else
            {
                //Instruccion(evaluacion);
                Instruccion(validarEvaluacion);
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validarEvaluacion = true;
            if (evaluacion == false)
            {
                validarEvaluacion = false;
            }
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarEvaluacion);
            }
            else
            {
                Instruccion(validarEvaluacion);
            } 
            match("while");
            match("(");
            validarEvaluacion=Condicion();
            match(")");
            match(";");
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            //REQUERIMIENTO 4
            bool validarFor = Condicion();
            if (evaluacion == false){
                validarFor = false;
            }
            //REQUERIMIENTO 6
            //a) Guardar la posicion del archivo de texto
            //bool validarFor=Condicion();
            //b) Agregar un ciclo while
            //while()
            //{
            match(";");
            Incremento(validarFor);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarFor);
            }
            else
            {
                Instruccion(validarFor);
            }
                // c) Regresar a la posicion de lectura del archivo
                // d) Sacar otro token
            //}
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string variable = getContenido();
            if (!existeVariable(variable))
            {
                throw new Error("\nError la variable <" + getContenido() + 
                                    "> no existe en linea: "+linea, log);
            }
            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
                if (evaluacion)
                {
                    modVariable(variable, getValor(variable)+1);
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
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            float e1 = stack.Pop();
            switch (operador)
            {
                case "==":
                    return e1 == e2;
                case ">":
                    return e1 > e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 < e2;
                case "<=":
                    return e1 <= e2;
                default:
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            //REQUERIMIENTO 4
            bool validarIF = Condicion();
            if (evaluacion == false)
            {
                validarIF = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIF);  
            }
            else
            {
                Instruccion(validarIF);
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    if (evaluacion == true)
                    {
                        BloqueInstrucciones(!validarIF);
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
                        Instruccion(!validarIF);
                    }
                    else
                    {
                        Instruccion(false);
                    }
                }
            }
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
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                if (evaluacion)
                {
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
                //Hacemos el parseo de val, de string a float, para poder utilizarlo en el metodo modVariable
                //Requerimiento 5
                float nuevaVal = float.Parse(val);
                modVariable(nombreVariable, nuevaVal);
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
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        break;
                    case "-":
                        stack.Push(n2 - n1);
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
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }

        //REQUERIMIENTO 3
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
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                string nombreVariable = getContenido();
                if (existeVariable(nombreVariable) != true)
                {
                    throw new Error("\nError la variable <" + getContenido() +
                                    "> no existe en linea: "+linea, log);
                }
                log.Write(getContenido() + " ");
                //REQUERIMIENTO 1_OBTENER EL TIPO DE DATO
                if (Dominante  < getTipo(getContenido()))
                {
                    Dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
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
                    //REQUERIMIENTO 2 ->: TIENE QUE ACTUALIZAR EL DOMINATE
                    //SI HUBO CASTEO SACO UN ELEMENTO DEL STACK
                    //CONVIERTO ESE VALOR AL EQUIVALENTE EN CASTEO
                    //Dominante = casteo;
                    float valorGuardado = stack.Pop();
                    valorGuardado = Convertir(valorGuardado, casteo);
                    stack.Push(valorGuardado);
                    Dominante = casteo;
                    //Requerimiento 3 -> 
                    //EJEMPLO: SI EL CASTEO ES char Y EL POP REGRESA UN 256 EL VALOR
                    //EQUIVALENTE EL CASTEO ES 0 
                    //Y METO ESE VALOR AL STACK
                    
                }
            }
        }
    }
}