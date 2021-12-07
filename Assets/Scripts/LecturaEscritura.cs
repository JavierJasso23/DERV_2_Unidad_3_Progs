using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using UnityEngine;

public class LecturaEscritura : MonoBehaviour
{
    SerialPort arduino;

    [SerializeField]
    TextMeshProUGUI estado;
    [SerializeField]
    TextMeshProUGUI txt_boton;

    [SerializeField]
    bool estadoLed = false;

    //programa 1- Agregar control de excepciones cuando se realiza la conexión del SE con el EV

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
                    string valor = arduino.ReadExisting();
                    if (!valor.Equals(""))
                    {
                        Debug.Log(valor);
                    }
                    
                }
            }
            yield return new WaitForSeconds(.50f);
        }
    }

}
