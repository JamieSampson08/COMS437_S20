using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    public GameObject boardPiece;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject temp = GameObject.Instantiate<GameObject>(boardPiece);
            temp.transform.position = new Vector3(2, 2, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
