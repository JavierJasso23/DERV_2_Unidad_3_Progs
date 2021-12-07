using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using UnityEngine;

public class MovP1 : MonoBehaviour
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
    [TextArea(5, 10)]
    public string cadena = "";

    //Variables Pract1
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float speedForce = 10f;
    [SerializeField]
    TextMeshProUGUI txtValorPot;

    public void conectar(string ncom)
    {
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
        else
        {  //desconectar
            arduino.Close();
            estado.text = "DESCONECTADO";
            txt_boton.text = "RECONECTAR";
        }
    }

    public void leer_datos()
    {
        StopAllCoroutines();
        StartCoroutine("leer_datos_arduino");
    }

    private void Update()
    {
        if (valSensor1 > 0 && valSensor1 <= 250) //w arriba
        {
            rb.AddForce(transform.forward * speedForce, ForceMode.Acceleration);
        }
        if (valSensor1 > 250 && valSensor1 <= 500) //s abajo
        {
            rb.AddForce(transform.forward * -1f * speedForce, ForceMode.Acceleration);
        }
        if (valSensor1 > 500 && valSensor1 <= 750) //a izq
        {
            rb.AddForce(transform.right * -1 * speedForce, ForceMode.Acceleration);
        }
        if (valSensor1 > 750 && valSensor1 <= 1000) //d der
        {
            rb.AddForce(transform.right * speedForce, ForceMode.Acceleration);
        }
    }

    IEnumerator leer_datos_arduino()
    {
        while (true)
        {
            if (arduino != null)
            {
                if (arduino.IsOpen)
                {
                    cadena += arduino.ReadExisting();
                    int index_begin = -1;
                    int index_end = -1;
                    if (cadena.Equals(""))//cuando no haya nada que leer
                    {
                        //No se hace nada
                    }
                    else
                    {
                        if ((index_begin = cadena.IndexOf('H')) != -1)
                        {
                            //Debug.Log("la Trama inicia correctamente");
                            if ((index_end = cadena.IndexOf('T')) != -1)
                            {
                                Debug.Log("La Trama esta completa! Indices->");
                                Debug.Log("Inicio: " + index_begin.ToString() + " Fin:" + index_end.ToString());

                                string valor = cadena.Substring(index_begin, index_end + 1);
                                Debug.Log("Cadena Obtenida: " + valor);

                                cadena = cadena.Substring(index_end + 1);

                                string temp = valor.Substring(1, valor.IndexOf('R') - 1);
                                valSensor1 = Convert.ToInt32(temp);
                                Debug.Log("Valor Sensor 1: " + temp);

                                txtValorPot.text = "V. Potenciometro: "+temp;
                            }
                        }
                        else
                        {
                            cadena = "";
                            Debug.Log("Trama Incompleta de Inicio Eliminada");
                        }
                    }
                }
            }
            yield return new WaitForSeconds(.01f);
        }
    }

}