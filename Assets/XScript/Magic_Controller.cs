using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic_Controller : MonoBehaviour {

    public float att = 0;//大絕傷害

    public bool Play_Water_SFX = false;//水花特效
    public AudioClip magic_sound;
    

    void Start () {
        GetComponent<Animation>().Stop();//停止動作
	}

    void PlayMagicAnimation()
    {
        GetComponent<AudioSource>().clip = magic_sound;
        GetComponent<AudioSource>().Play();
        GetComponent<Animation>().Play("dragon_Flying_down");//播放移動動畫
        //將父物件移除
        Destroy(gameObject.transform.parent.gameObject, 2.5f);
    }

	void Update () {
		//假設正在撥放移動動作
        if(GetComponent<Animation>().IsPlaying("dragon_Flying_down"))
        {
            //將動作進度轉為0~1
            float step = GetComponent<Animation>()["dragon_Flying_down"].normalizedTime;
            //假使動作進度大於0.45
            if(step >=0.45)
            {
                //假使尚未撥放水花特效
                if(Play_Water_SFX == false)
                {
                    //可播放水花特效
                    Play_Water_SFX = true;
                    //條用播放水花特效的事件
                    gameObject.transform.parent.gameObject.BroadcastMessage("PlayWaterParticle");
                }
            }
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        //假使大絕跟NPC接觸到
        if(other.tag == "tag_npc")
        {
            other.gameObject.BroadcastMessage("NPC_Hurt", att);
        }
    }
}
