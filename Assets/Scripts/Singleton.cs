using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public bool IsDontDestroyOnload;
    private static bool _isDontDestroyOnload;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T instanceInScene = FindObjectOfType<T>();

                RegisterInstance(instanceInScene, _isDontDestroyOnload);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _isDontDestroyOnload = IsDontDestroyOnload;
        if (_instance == null)
        {
            RegisterInstance((T)(MonoBehaviour)this, IsDontDestroyOnload);
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    private static void RegisterInstance(T newInstance, bool isDontDestroyOnload)
    {
        if (newInstance == null) return;
        _instance = newInstance;

        if (isDontDestroyOnload)
        {
            DontDestroyOnLoad(_instance.transform.root);
        }
    }

}