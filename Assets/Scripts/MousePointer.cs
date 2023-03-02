using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    Vector3 mouse;
    public string colName { get; set; }
    [SerializeField] Battle battle;
    void Start()
    {
        Cursor.visible = false;
        colName = null;
    }
    void Update()
    {
      
    }

    void FixedUpdate()
    {
        mouse = Input.mousePosition;
        this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10));
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("UI"))
        {
            colName = col.name;
        }
      
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("UI"))
        {
            colName = null;

        }
    }
}
