using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player picks up coin
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            GameManager.Instance.CoinPickUp();
        }
    }
}