using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Opt_HalfDuplex_Int : MonoBehaviour
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
    public int valSensor4;

    [SerializeField]
    [TextArea(5, 10)]
    public string cadena = "";

    [SerializeField]
    TextMeshProUGUI avisotxt;

    [SerializeField]
    Image sens1;
    [SerializeField]
    Image sens2;
    [SerializeField]
    Image sens3;
    [SerializeField]
    Image sens4;

    bool mov1 = false;
    bool mov2 = false;
    bool mov3 = false;
    bool mov4 = false;

    string strMov1 = "";
    string strMov2 = "";
    string strMov3 = "";
    string strMov4 = "";

    [SerializeField]
    GameObject sonidoAlarma;

    private void Update()
    {
        Debug.Log("Vals 1 " + valSensor1);
        if (valSensor1 < 10 && !mov1)
        {
            sens1.color = Color.red;
            StartCoroutine("reinicioSens1");
            mov1 = true;
        }
        if (valSensor2 < 10)
        {
            sens2.color = Color.red;
            StartCoroutine("reinicioSens2");
            mov2 = true;
        }
        if (valSensor3 < 10)
        {
            sens3.color = Color.red;
            StartCoroutine("reinicioSens3");
            mov3 = true;
        }
        if (valSensor4 < 10)
        {
            sens4.color = Color.red;
            StartCoroutine("reinicioSens4");
            mov4 = true;
        }

        if (mov1) { strMov1 = "en el cuarto de huéspedes "; }
        if (mov2) { strMov2 = ",en el cuarto de niños "; }
        if (mov3) { strMov3 = ",en el dormitorio principal "; }
        if (mov4) { strMov4 = ",en la puerta principal "; }

        if (!mov1 && !mov2 && !mov3 && !mov4)
        {
            avisotxt.text = "No se detecto movimiento.";
            strMov1 = "";
            strMov2 = "";
            strMov3 = "";
            strMov4 = "";
            sonidoAlarma.SetActive(false);
        }

        //Se detecto Movimiento en los sensores: 1 2 3 4
        if (mov1 || mov2 || mov3 || mov4)
        {
            avisotxt.text = "Se detecto Movimiento " + strMov1 + strMov2 + strMov3 + strMov4;
            sonidoAlarma.SetActive(true);
        }
    }

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
                    if (cadena.Equals(""))
                    {

                    }
                    else
                    {
                        if ((index_begin = cadena.IndexOf('H')) != -1)
                        {
                            //Debug.Log("la Trama inicia correctamente");
                            if ((index_end = cadena.IndexOf('T')) != -1)
                            {
                                //Debug.Log("La Trama esta completa! Indices->");
                                //Debug.Log("Inicio: " + index_begin.ToString() + " Fin:" + index_end.ToString());

                                string valor = cadena.Substring(index_begin, index_end + 1);
                                Debug.Log("Cadena Obtenida: " + valor);

                                //Quita la cadena que acaba de recuperar en la variable VALOR
                                cadena = cadena.Substring(index_end + 1);

                                string temp = valor.Substring(1, valor.IndexOf('R') - 1); //saca sens 1
                                valSensor1 = Convert.ToInt32(temp);
                                Debug.Log("Valor Sensor 1: " + temp);

                                temp = valor.Substring(valor.IndexOf('R') + 1);
                                //Debug.Log("temporal despues de sens 1 " + temp);

                                string aux;
                                aux = temp.Substring(0, temp.IndexOf('R'));
                                valSensor2 = Convert.ToInt32(aux);
                                Debug.Log("Valor Sensor 2: " + aux);

                                temp = temp.Substring(temp.IndexOf('R') + 1);
                                //Debug.Log("temporal despues de sens 2 " + temp);

                                aux = temp.Substring(0, temp.IndexOf('R'));
                                valSensor3 = Convert.ToInt32(aux);
                                Debug.Log("Valor Sensor 3: " + aux);

                                temp = temp.Substring(temp.IndexOf('R') + 1);
                                //Debug.Log("temporal despues de sens 3 " + temp);

                                temp = temp.Substring(0, temp.Length - 1);
                                temp = temp.Replace("T","");
                                Debug.Log("Valor Sensor 4: " + temp);
                                valSensor4 = Convert.ToInt32(temp);
                                
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

    IEnumerator reinicioSens1()
    {
        int time1 = 0;
        while (true)
        {
            time1++;
            if (time1 == 5)
            {
                sens1.color = Color.green;
                mov1 = false;
                StopCoroutine("reinicioSens1");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator reinicioSens2()
    {
        int time1 = 0;
        while (true)
        {
            time1++;
            if (time1 == 5)
            {
                sens2.color = Color.green;
                mov2 = false;
                StopCoroutine("reinicioSens2");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator reinicioSens3()
    {
        int time1 = 0;
        while (true)
        {
            time1++;
            if (time1 == 5)
            {
                sens3.color = Color.green;
                mov3 = false;
                StopCoroutine("reinicioSens3");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator reinicioSens4()
    {
        int time1 = 0;
        while (true)
        {
            time1++;
            if (time1 == 5)
            {
                sens4.color = Color.green;
                mov4 = false;
                StopCoroutine("reinicioSens4");
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
