using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetPoints : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtPuntos;
    
    int pnts = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Coin"))
        {
            pnts = int.Parse(txtPuntos.text);
            pnts++;
            txtPuntos.text = pnts.ToString();
            Destroy(collision.gameObject);
        }
    }
}
