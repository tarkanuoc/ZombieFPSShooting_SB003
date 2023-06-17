using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T instanceInScene = FindObjectOfType<T>();
                RegisterInstance(instanceInScene);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            RegisterInstance((T)(MonoBehaviour)this);
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    private static void RegisterInstance(T newInstance)
    {
        if (newInstance == null) return;
        _instance = newInstance;
        DontDestroyOnLoad(_instance.transform.root);
    }

}