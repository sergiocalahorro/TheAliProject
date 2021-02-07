using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField]
    private float _rotatingSpeed;
    private GameManager _gameManager;

    // Start is called before the first frame update
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        Rotate();
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player picks up coin
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            _gameManager.CoinPickUp();
        }
    }

    /// <summary>
    /// Rotate coin on the Y axis
    /// </summary>
    private void Rotate()
    {
        transform.Rotate(0f, _rotatingSpeed * Time.deltaTime, 0f);
    }
}