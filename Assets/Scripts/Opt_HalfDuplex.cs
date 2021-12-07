using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using UnityEngine;

public class Opt_HalfDuplex : MonoBehaviour
{
    SerialPort arduino;

    [SerializeField]
    TextMeshProUGUI estado;
    [SerializeField]
    TextMeshProUGUI txt_boton;

    [SerializeField]
    bool estadoLed = false;


    [SerializeField]
    public int valSensor1;
    [SerializeField]
    public int valSensor2;
    [SerializeField]
    public int valSensor3;

    [SerializeField]
    [TextArea(5, 10)]
    public string cadena = "";

    public void conectar(string ncom) {         

        if (arduino == null)  //conectar
        {
            arduino = new SerialPort("COM" + ncom, 9600);  //Ej: COM2
            arduino.ReadTimeout = 100; //100ms
            arduino.Open();
            estado.text = "CONECTADO";
            txt_boton.text = "DESCONECTAR";
        }
        else if (!arduino.IsOpen)  //reconectar
        {
            arduino.Open();
            estado.text = "RECONECTADO";
            txt_boton.text = "DESCONECTAR";
        }
        else {  //desconectar
            arduino.Close();
            estado.text = "DESCONECTADO";
            txt_boton.text = "RECONECTAR";
        }

    }



    public void escribir_datos() {

        if (arduino != null)
        {
            if (arduino.IsOpen)
            {
                if (estadoLed)
                {
                    arduino.WriteLine("1"); //envia 3 caracteres ... 1\n\r 
                    Debug.Log("Se envio un 1");
                }
                else {
                    arduino.WriteLine("0"); //   0\n\r
                }
                estadoLed = !estadoLed;
            }
        }
    }


    public void leer_datos() {
        StopAllCoroutines();
        StartCoroutine("leer_datos_arduino");
    }

    IEnumerator leer_datos_arduino() {
        while (true)
        {
            if (arduino !=null)
            {
                if (arduino.IsOpen)
                {
                    cadena += arduino.ReadExisting();
                    //Debug.Log(valor);
                    //H768R266R809T

                    //CUIDADO CUANDO UNA TRAMA SE LEE DESDE EL COMIENZO TRUNCADA! 
                    int index_begin=-1;
                    int index_end = -1;
                    if (cadena.Equals(""))//cuando no haya nada que leer
                    {
                        //No se hace nada
                    }
                    else 
                    //Posibilidad 1 de falla, llega solo esto en la lectura 1: 556R40T  //arreglada

                    //Posibilidad 2 de falla, llega solo esto en la lectura 1: 556R40TH27R30R50T
                    // ESTO SE PUEDE ARREGLAR COMPROBANDO QUE EL INDICE DE "H" SEA MENOR QUE EL "T"

                    //Posibilidad 3 de falla, llega solo esto en la lectura 1: 556R40TH30R50T
                    // ESTO SE PUEDE ARREGLAR CON UNA VALIDACIÓN CORROBORANDO QUE LA INFORMACIÓN 
                    // HAYA PODIDO SER LEIDA O VAYA A PODER SER LEIDA COMPLETAMENTE (CONTANDO LOS SEPARADORES)
                    {
                        if ((index_begin = cadena.IndexOf('H')) != -1) // AQUI SE SOLUCIONARÍA LO DE CUIDADO
                        {
                            //Debug.Log("la Trama inicia correctamente");
                            if ((index_end = cadena.IndexOf('T')) != -1)
                            {
                                Debug.Log("La Trama esta completa! Indices->");
                                Debug.Log("Inicio: " + index_begin.ToString() + " Fin:" + index_end.ToString());

                                //1.- quitar una trama de la cola

                                //TAREA INDIVIDUAL PARA EL 9 DE NOVIEMBRE  
                                // :  IDEAR Y PROBAR UN ALGORITMO QUE PERMITA QUITAR UNA TRAMA DE
                                //          UNA CONJUNTO(COLA) DE TRAMAS

                                string valor = cadena.Substring(index_begin, index_end + 1);
                                Debug.Log("Cadena Obtenida: " + valor);

                                //Quita la cadena que acaba de recuperar en la variable VALOR
                                cadena = cadena.Substring(index_end + 1);

                                //2.-procesarla

                                //PROCESAMIENTO:

                                string temp = valor.Substring(1, valor.IndexOf('R') - 1);
                                valSensor1 = Convert.ToInt32(temp);
                                Debug.Log("Valor Sensor 1: " + temp);

                                //Trama sin el valor del primer sensor
                                temp = valor.Substring(valor.IndexOf('R') + 1);
                                Debug.Log(temp);

                                /////////////////////////////////

                                //obtención del valor del segundo sendor
                                string aux;   //                         
                                aux = temp.Substring(0, temp.IndexOf('R'));
                                valSensor2 = Convert.ToInt32(aux);
                                Debug.Log("Valor Sensor 2: " + aux);

                                //trama sin el valor del segundo sensor
                                temp = temp.Substring(temp.IndexOf('R') + 1);
                                Debug.Log(temp);

                                //obtencion del valor del tercer sensor
                                temp = temp.Substring(0, temp.Length - 1);
                                Debug.Log("Valor Sensor 3: " + temp);
                                //valSensor3 = Convert.ToInt32(temp);

                                /////////////////////////////
                                ///  



                            }
                        }///
                        else
                        { //si no contiene a la H
                            cadena = ""; //para eliminar fragmentos de tramas que no tienen un comienzo
                                         //**PODRÍA MEJORARSE...(¿Qué pasa con las intermedias incompletas?)
                            Debug.Log("Trama Incompleta de Inicio Eliminada");
                        }
                    }
                }
            }
            yield return new WaitForSeconds(.01f);
        }
    }

}
