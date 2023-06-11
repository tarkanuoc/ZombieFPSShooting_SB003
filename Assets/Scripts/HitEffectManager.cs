using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitSurfaceType
{ 
    Dirt = 0,
    Blood =1
}

[Serializable]
public class HitEffectMapper
{
    public HitSurfaceType surface;
    public GameObject effectPrefab;
}

public class HitEffectManager : MonoBehaviour
{
    public HitEffectMapper[] effectMap;
}
