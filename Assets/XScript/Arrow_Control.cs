using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Control : MonoBehaviour {

    public float att = 30.0f; //攻擊力
    public float speed = 15.0f; //移動速度
    public GameObject Arrow_sfx;//光箭火花特效
    

    void Start () {
		
	}
	
	
	void Update () {

        transform.Translate(Vector3.forward * Time.deltaTime * speed);

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "tag_wall")
        {
            Destroy(gameObject);
        }
        //判斷碰撞對象的標籤
        if (other.GetComponent<Collider>().tag == "tag_npc")

        {
            //調NPC跳用NPC_Hurt事件
            other.gameObject.BroadcastMessage("NPC_Hurt", att);
            //移除遊戲物件
            Destroy(gameObject);
            Vector3 Pos;//宣告爆破位置
                        //計算爆破位置
            Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            Instantiate(Arrow_sfx, Pos, Quaternion.identity);//複製靈魂物件
        }

    }
}
