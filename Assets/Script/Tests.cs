using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tests : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit2D)
            {
                Debug.Log(hit2D.collider.gameObject.name);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(eventData.position);
    }
}
