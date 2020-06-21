using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;


public static class ExtensionMethods
{
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        System.Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    public static T FindComponent<T>(this GameObject g, bool in_parent = true, bool in_children = true, int sibling_depth = 0) where T : Component
    {
        if (g.GetComponent<T>() != null)
        {
            return g.GetComponent<T>();
        }
        else if (in_children && g.GetComponentInChildren<T>() != null)
        {
            return g.GetComponentInChildren<T>();
        }
        else if (in_parent)
            if (g.GetComponentInParent<T>() != null)
                return g.GetComponentInParent<T>();

        GameObject current = g;
        while (sibling_depth > 0)
        {
            current = current.transform.parent.gameObject;
            if (!current)
                break;
            if (current.GetComponentInChildren<T>() != null)
            {
                return current.GetComponentInChildren<T>();
            }
            sibling_depth--;
        }

        return g.GetComponent<T>();
    }

    public static TValue GetValueOrDefault<TKey, TValue>
    (this IDictionary<TKey, TValue> dictionary,
     TKey key,
     TValue defaultValue)
    {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    public static Vector3 RandomPointInBounds(this Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}

public static class TransformDeepChildExtension
{
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    public static Transform FindDeepParent(this Transform aChild, string aName)
    {
        Transform node = aChild;
        while (node.parent)
        {
            Transform result = node.parent.Find(aName);
            if (result) return result;
            node = node.parent;
        }
        return null;
    }
}