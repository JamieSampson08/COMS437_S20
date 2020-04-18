using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardClick : MonoBehaviour
{
    Collider c;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit h;

        Physics.Raycast(castPoint, out h);
        int row = (int)(h.point.z + transform.position.z + 4);
        int col = (int)(h.point.x + transform.position.x + 4);

        print(row + " " + col);

    }
}
