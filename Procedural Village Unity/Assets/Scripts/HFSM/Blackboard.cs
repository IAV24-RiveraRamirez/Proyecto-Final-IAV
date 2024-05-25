using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este código proviene de la biblioteca BehaviourBricks.dll.
// Obtenido a través de los archivos en el proyecto de Unity dados como Plantilla de
// la práctica 3 de la asignatura IAV, del Grado en Desarrollo de Videojuegos en la UCMº

public class Blackboard
{
    private struct Entry
    {
        public System.Type type;

        public object value;
    }

    private Dictionary<string, Entry> _blackboard = new Dictionary<string, Entry>();

    private Blackboard nextInStack;

    public Blackboard()
    {
        nextInStack = null;
    }

    public Blackboard(Blackboard nextInStack)
    {
        this.nextInStack = nextInStack;
    }

    public Blackboard next()
    {
        return nextInStack;
    }

    public object Get(string name, System.Type t)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        Entry value = default(Entry);
        if (!_blackboard.TryGetValue(name, out value))
        {
            if (nextInStack != null)
            {
                return nextInStack.Get(name, t);
            }

            return null;
        }

        if ((object)value.type != t)
        {
            return null;
        }

        return value.value;
    }

    public bool Set(string name, System.Type t, object value)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        Entry value2 = default(Entry);
        if (!_blackboard.TryGetValue(name, out value2))
        {
            value2.type = t;
            value2.value = value;
            _blackboard.Add(name, value2);
            return true;
        }

        if ((object)value2.type != t)
        {
            return false;
        }

        value2.value = value;
        _blackboard[name] = value2;
        return true;
    }
}
