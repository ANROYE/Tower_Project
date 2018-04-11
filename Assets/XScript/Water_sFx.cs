using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_sFx : MonoBehaviour {

    void PlayWaterParticle()
    {
        //播放粒子效果
        GetComponent<ParticleSystem>().Play();
    }
}
