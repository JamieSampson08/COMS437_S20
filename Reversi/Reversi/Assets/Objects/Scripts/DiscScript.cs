using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscScript : MonoBehaviour
{

    public enum PieceColor { BLACK, WHITE };

    public PieceColor Color;

    // Start is called before the first frame update
    void Start()
    {
        Color = PieceColor.BLACK;
    }

    public void Flip(PieceColor newColor)
    {
        if (newColor == PieceColor.BLACK) 
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("W2B");
            Color = PieceColor.BLACK;
        }
        else
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("B2W");
            Color = PieceColor.WHITE;
        }
    }
}