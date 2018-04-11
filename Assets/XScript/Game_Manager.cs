using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    [Header("玩家")]
    public GameObject Player;               //玩家角色
    public AnimationClip Player_ani;        //玩家動作

    private bool bCanCreateArrow = false;   //是否產生光箭
    private RaycastHit hit;                 //射線碰撞的物件
    private Vector3 target_pos;             //目標點位置
    private Vector3 look_pos;               //玩家角色注視的位置

    public GameObject fire_pos;		//發射光箭的位置參考

    public GameObject Arrow ;        //攻擊的光箭物件
    public GameObject RedArrow;//攻擊的紅光箭物件
    public GameObject GreenArrow;//攻擊的綠光箭物件
    public int Green_Arrow_Count = 0;//自訂綠光箭物件生成幾顆
    public int arrowType = 1;

    [Header("敵人")]
    // Use this for initialization
    public GameObject NPC_Start_Pos;        //敵人誕生位置
    public GameObject NPC;      //敵人角色
    public int NPC_Max_Num = 15;        //敵人最大數量
    private float NPC_Cur_Num = 0;      //敵人目前數量
    private float NPC_Real_Time = 0;        //敵人實際時間
    private float NPC_Born_Time = 0;		//敵人誕生時間差

    [Header("大絕招")]
    public GameObject Magic_Obj_Ani;//複製的魔法動畫物件
    private bool bCreateMagic = true;//可否產生魔法動作
    private float Magic_Cur_Time = 0;//產生魔法的當前時間
    private float Magic_Total_Time = 5;//產生魔法的時間差
    private GameObject Magic_Obj;//魔法動畫物件
    private bool bCanDragMagic = true;//可否拖曳魔法物件

    public float PlayerMaxHP = 100;//玩家最大生命值
    public static float PlayerHP = 100;//玩家生命值
    public float PlayerMaxMp = 100;//玩家最大魔力值
    public static float PlayerMP = 100;//玩家魔力值

    [Header("玩家生命值和魔力值")]
    public Image HP_Bar;
    public Image MP_Bar;
    private float val_hp;
    private float val_mp;
    public float max_val_NPC;
    public float max_MP_Bar;

    [Header("敵人數量")]
    public Image NPC_Bar;
    private float val_NPC;
    public float max_NPC_Bar;


    void Start()
    {
        Player.GetComponent<Animation>().clip = Player_ani; //指定動作
        Player.GetComponent<Animation>().Stop();//暫停動作
        PlayerMaxHP = 100;
        PlayerHP = 100;
        PlayerMaxMp = 100;
        PlayerMP = 100;

        PlayerHP = PlayerMaxMp;//玩家起始生命值為最大生命值
        PlayerMP = PlayerMaxMp;//玩家起始魔力為最大魔力值

        max_val_NPC = HP_Bar.GetComponent<RectTransform>().sizeDelta.x;
        max_MP_Bar = MP_Bar.GetComponent<RectTransform>().sizeDelta.x;
        max_NPC_Bar = NPC_Bar.GetComponent<RectTransform>().sizeDelta.x;

    }
    float a = 0;
    // Update is called once per frame
    void Update()
    {
        CreateNPC();
        if (Input.GetMouseButton(0))
        {
            //產生雷射光
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //如果取得的物件是地板

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "tag_floor")
                {
                    //計算目前玩家點擊的位置
                    target_pos = new Vector3(hit.point.x, Player.transform.position.y, hit.point.z);
                    //使用內插法計算玩家角色注視的位置
                    look_pos = Vector3.Lerp(look_pos, target_pos, 1);
                    //命令玩家角色注視此點
                    Player.transform.LookAt(look_pos);
                }
                //假使魔法動畫物件存在
                if (Magic_Obj && bCanDragMagic == true)
                {
                    //假使滑鼠點擊的是地板
                    if (hit.collider.tag == "tag_floor")
                    {
                        //將魔法動畫物件設定在地板上
                        Magic_Obj.transform.position = hit.point;
                    }
                }
            }
            if (!Player.GetComponent<Animation>().isPlaying)
            {
                //播放動作
                Player.GetComponent<Animation>().Stop();
                Player.GetComponent<Animation>().Play();
                bCanCreateArrow = true;
            }
            //假設滑鼠按下魔法按鈕
            if (Button_Manager._press && PlayerMP >= 10)
            {
                //假設可以產生魔法
                if (bCreateMagic == true)
                {
                    bCreateMagic = false;//停止產生魔法動畫物件
                    bCanDragMagic = true;//取用魔法物件拖曳
                    //複製魔法動畫物件
                    Magic_Obj = Instantiate(Magic_Obj_Ani, new Vector3(200, 200, 200), Quaternion.identity) as GameObject;
                }
            }
            if (!Button_Manager._press)
            {
                if (Magic_Obj && bCreateMagic == false)
                {
                    //停止魔法物件拖曳
                    bCanDragMagic = false;
                    //繼續產生魔法動畫物件(暫時)
                    bCreateMagic = true;
                    //播放魔法特效
                    Magic_Obj.BroadcastMessage("PlayMagicAnimation");
                    PlayerMP = Mathf.Max(PlayerMP - 10f, 0);
                    //print("魔力" + PlayerMP);

                    val_mp = max_MP_Bar / PlayerMaxMp;
                    MP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(MP_Bar.GetComponent<RectTransform>().sizeDelta.x - (val_mp * 10),0)
                                                                         ,MP_Bar.GetComponent<RectTransform>().sizeDelta.y);
                }
            }
        }

       



        if (Player.GetComponent<Animation>().isPlaying)
        {
            Player.GetComponent<Animation>()["moon_att"].speed = 10f;
            //判斷攻擊動畫的進度與可否創造光箭
            if (Player.GetComponent<Animation>()["moon_att"].normalizedTime >= 0.5 && bCanCreateArrow == true)
            {
                //創造光箭
                bCanCreateArrow = false;
                if (Input.GetMouseButton(0) && arrowType == 1) 
                    
                    CreateArrow();
                else if (Input.GetMouseButton(0) && arrowType == 2)

                    CreateGreenArrow();
                
                else if (Input.GetMouseButton(0) && arrowType == 3)

                    CreateRedArrow();
                
            }
        }
    }
    void CreateArrow()
    {
        GameObject Arrow_perfab;
        Arrow_perfab = Instantiate(Arrow, fire_pos.transform.position, Player.transform.rotation) as GameObject;
        Arrow_perfab.transform.Rotate(Vector3.up * 360);
    }
    void CreateRedArrow()
    {
        GameObject Arrow_perfab;
        Arrow_perfab = Instantiate(RedArrow, fire_pos.transform.position, Player.transform.rotation) as GameObject;
        Arrow_perfab.transform.Rotate(Vector3.up * 360);
    }
    void CreateGreenArrow()
    {
        for (int i = 1; i < Green_Arrow_Count; i++)
        {
            float a = i * 5f;//15為範圍
            GameObject FierArow_prefab;
            float n = 360 - (15 * Green_Arrow_Count) / 2;
            FierArow_prefab = Instantiate(GreenArrow, fire_pos.transform.position, Player.transform.rotation) as GameObject;
            FierArow_prefab.transform.Rotate(new Vector3(0, n + a, 0));
        }
    }

    public void change_type(int val)
    {
        arrowType = val;
    }

    void CreateNPC()
    {
        //NPC目前數量小於最大數量
        if (NPC_Cur_Num < NPC_Max_Num)
        {
            //假使誕生實際時間大於誕生時間差
            if (NPC_Real_Time >= NPC_Born_Time)
            {
                Vector3 Max_Start = NPC_Start_Pos.GetComponent<Collider>().bounds.max; //計算NPC起始點的最大邊界
                Vector3 Min_Start = NPC_Start_Pos.GetComponent<Collider>().bounds.min; //計算NPC起始點的最小邊界

                //計算NPC的位置
                Vector3 Random_Pos = new Vector3(Random.Range(Min_Start.x, Max_Start.x), 0f, Max_Start.z);
                GameObject NPC_Perfab = null;//定義NPC物件

                //更正角度用
                Vector3 angles = transform.rotation.eulerAngles;
                angles.y = 180;

                NPC_Perfab = Instantiate(NPC, Random_Pos, Quaternion.Euler(angles)) as GameObject; //複製NPC
                NPC_Cur_Num++;//NPC數量累加

                NPC_Born_Time = Random.Range(1.5f, 3f);//重新計算誕生的時間
                NPC_Real_Time = 0; //實際誕生時間歸零

                val_NPC = max_val_NPC / NPC_Max_Num;
                NPC_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(NPC_Bar.GetComponent<RectTransform>().sizeDelta.x - (val_NPC), 0)
                                                                 , NPC_Bar.GetComponent<RectTransform>().sizeDelta.y);
            
        }
            else
            {
                NPC_Real_Time = NPC_Real_Time + Time.deltaTime; //誕生時間累加
            }
        }
    }
    void CalculateHP(float att)
    {
        //目前生命值 = 目前生命值 - 受傷值
        PlayerHP = Mathf.Max(PlayerHP - att, 0);
        //print("生命"+PlayerHP);

        val_hp = max_val_NPC / PlayerMaxHP;
        HP_Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(HP_Bar.GetComponent<RectTransform>().sizeDelta.x - (val_hp * att), 0)
                                                         , HP_Bar.GetComponent<RectTransform>().sizeDelta.y);    
    }
}
