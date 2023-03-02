using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Vector3 defaultPos;    
    [SerializeField] Battle battle;
    [SerializeField] GameObject camera;
    Vector3 cameraPos;
    Vector3 targetPos;
    float speed = 5f;
    float x, y, lastY;
    bool isMoving = false;
    bool hitWall = false;
    int trigger = 0;
   
    void Start()
    {
        cameraPos = camera.transform.position;
        transform.position = defaultPos;
        hitWall = false;
    }
    void Update()
    {
        if (!(isMoving))
        {
            if (!(battle.inBattle))
            {
                x = Input.GetAxisRaw("Horizontal"); // Raw:「1」または「-1」の入力を受け取る →　キッチリ一マスずつ移動
                y = Input.GetAxisRaw("Vertical");
            }
            else { x = 0; y = 0; }
            if(x != 0) { y = 0; }       
            StartCoroutine(Move(new Vector2(x, y)));
        }
        if(x == 0 && y == 0) { trigger = 0; }
        if(x != lastY) { trigger = 0; }
        camera.transform.position = cameraPos;
        lastY = y;
    }

    IEnumerator Move(Vector3 direction)
    {
        isMoving = true;
        targetPos = transform.position + direction;       
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            if (hitWall) { targetPos.x -= x; targetPos.y -= y; hitWall = false; }　//壁に当たったら元の位置に戻る
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;               //1フレーム待つ　→　While文で移動を一瞬で終えるのではなく1フレームずつ進む
        }
        transform.position = targetPos;
        isMoving = false;
        hitWall=false;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            hitWall = true;
        }
       
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("CameraTrigger"))
        {
            
            if (isMoving && trigger == 0)
            {
                if (x > 0) { cameraPos.x = col.gameObject.GetComponent<CameraTrigger>().rightX; }
                else if (x < 0) { cameraPos.x = col.gameObject.GetComponent<CameraTrigger>().leftX; }
                
                if (y > 0) { cameraPos.y = col.gameObject.GetComponent<CameraTrigger>().upperY; }
                else if (y < 0) { cameraPos.y = col.gameObject.GetComponent<CameraTrigger>().lowerY; }
                trigger = 1;
            }
        }
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.SetActive(false);
            battle.inBattle = true;
            battle.enemySprite = col.gameObject.GetComponent<SpriteRenderer>().sprite;
            transform.position = targetPos;
        }
    }

}
