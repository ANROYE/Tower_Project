using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Controller : MonoBehaviour {

    public GameObject NPC_player;//NPC物件
    public float Speed = 5;//移動速度
    private float HP = 100;//生命值
    public float MaxHP = 100;//最大生命值
    private bool bHuet = false;
    private bool Attack = false;//攻擊狀態
    public AnimationClip[] Ani = new AnimationClip[3];
    public GameObject NPC_Soul;//靈魂的粒子特效

    public AudioClip hitVoice;
    public AudioClip hitWall;
    private int NPC_ATT;
    public int NPC_ATT_Max = 0;
    public int NPC_ATT_Min = 0;
    private GameObject Player;//玩家腳色

    [Header("生命值")]
    public Image HP_Bar;
    private float val_hp;
    public float max_HP_Bar;

    void Start () {
        Player = GameObject.Find("ApplicationGameMaker");
        HP = MaxHP;//設定生命值
        NPC_player.GetComponent<Animation>().clip = Ani[0];//指定目前動作為走路
        NPC_player.GetComponent<Animation>().Play();
        max_HP_Bar = HP_Bar.GetComponent<RectTransform>().sizeDelta.x;
	}
	
	void Update () {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed, Space.Self);

        //取得目前NPC攻擊動作的進度
        if (NPC_player.GetComponent<Animation>()[Ani[1].name.ToString()].normalizedTime >= 0.3)
        {
            if (Attack == false)
            {
                //未攻擊狀態
                GetComponent<AudioSource>().clip = hitWall;
                GetComponent<AudioSource>().Play();
                Attack = true;//切換為攻擊模式
                NPC_ATT = Random.Range(NPC_ATT_Max, NPC_ATT_Min);
                Player.BroadcastMessage("CalculateHP", NPC_ATT);//計算玩家生命值
                Invoke("Att", NPC_player.GetComponent<Animation>()[Ani[1].name.ToString()].length);
            }
        }
    }
    void Att()
    {
        Attack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //碰撞對象的標籤是player_tag
        if (other.GetComponent<Collider>().tag == "Defence_wall")
        {
            Speed = 0f;//速度為0,不在前進
            NPC_player.GetComponent<Animation>().Stop();//動作停止
            NPC_player.GetComponent<Animation>().clip = Ani[1];//切換為攻擊動作
            NPC_player.GetComponent<Animation>().Play();//播放動作
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //持續碰撞對象的標籤是player_tag
        if (other.GetComponent<Collider>().tag == "Defence_wall")
        {
            //假使動作停止
            if(!NPC_player.GetComponent<Animation>().isPlaying)
            {
                NPC_player.GetComponent<Animation>().Stop();//動作停止
                NPC_player.GetComponent<Animation>().clip = Ani[1];//切換為攻擊動作
                NPC_player.GetComponent<Animation>().Play();//播放動作
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //碰撞對象的標籤是player_tag
        if (other.GetComponent<Collider>().tag == "Defence_wall")
        {         
                Speed = 5f;//恢復移動
                NPC_player.GetComponent<Animation>().Stop();//動作停止
                NPC_player.GetComponent<Animation>().clip = Ani[0];//切換為攻擊動作
                NPC_player.GetComponent<Animation>().Play();//播放動作           
        }
    }
    void NPC_Hurt(float att)
    {
        if (bHuet == false)
        {
            bHuet = true;
            GetComponent<AudioSource>().clip = hitVoice;
            GetComponent<AudioSource>().Play();
            //向後位移40單位
            transform.Translate(Vector3.back * Time.deltaTime * 40f);
            HP = HP - att;

            val_hp = max_HP_Bar / MaxHP;
            HP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(HP_Bar.GetComponent<RectTransform>().sizeDelta.x - (val_hp * att), 0)
                                                                 , HP_Bar.GetComponent<RectTransform>().sizeDelta.y);
            //print(HP);
            if (HP <= 0)
            {
                HP_Bar.transform.parent.gameObject.SetActive(false);
                GetComponent<Collider>().enabled = false;//取消物體碰撞
                Speed = 0;

                NPC_player.GetComponent<Animation>().Stop();//動作停止
                NPC_player.GetComponent<Animation>().clip = Ani[2];//切換為死亡動作
                NPC_player.GetComponent<Animation>().Play();//播放動作
                Destroy(gameObject, 2);//2秒後移除此遊戲物件
                Vector3 Soul_Pos;//宣告靈魂位置
                                 //計算靈魂位置
                Soul_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y + 5, transform.localPosition.z - 5);
                Instantiate(NPC_Soul, Soul_Pos, Quaternion.identity);//複製靈魂物件
            }
        }
        bHuet = false;
    }
}
