using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.data;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.data = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] data;
    }
}
