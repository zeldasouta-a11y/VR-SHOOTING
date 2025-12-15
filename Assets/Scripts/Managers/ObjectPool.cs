using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    Dictionary<Class, Queue<GameObject>> queue = new ();
}
