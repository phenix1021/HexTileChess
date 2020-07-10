using UnityEngine;
using Phenix.Unity.Collection;
using System.Collections.Generic;

public class LZY
{
    public static bool operator ==(LZY l1, LZY l2) { return l1.a == l2.a; }
    public static bool operator !=(LZY l1, LZY l2) { return l1.a != l2.a; }

    public int a;

    public override bool Equals(object obj)
    {
        if (obj is LZY == false)
        {
            return false;
        }
        return a == (obj as LZY).a;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class test : MonoBehaviour
{
    MinHeap<int> _minHeap = new MinHeap<int>((x, y) => { return x.CompareTo(y); });

    public StageConfigAsset asset1;   // 关卡配置表    
    public ScriptableObject asset2;   // 关卡配置表    

    // Use this for initialization
    void Start()
    {/*
        LZY m = new LZY();
        LZY n = new LZY();
        m.a = 1;
        n.a = 1;
        HashSet<LZY> h = new HashSet<LZY>();
        h.Add(m);
        h.Add(n);
        Debug.Log(m == n);
        Debug.Log(m.Equals(n));*/
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            CommandMarch march = new CommandMarch(GetComponent<Hero>(),
                new List<Vector3>() { new Vector3(-5.5f, 0.6f, 5.3f),
                new Vector3(-4.6f, 0.6f, 3.8f), new Vector3(-3.7f, 0.6f, 2.3f)
                , new Vector3(-2.0f, 0.6f, 2.3f)});
            GetComponent<Hero>().AddCommand(march);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            CommandFly fly = new CommandFly(GetComponent<Hero>(), new Vector3(-2.0f, 0.6f, 2.3f));
            GetComponent<Hero>().AddCommand(fly);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            CommandAttack attack = new CommandAttack(GetComponent<Hero>(), new Vector3(-3.7f, 0.6f, 2.3f), 1, false);
            GetComponent<Hero>().AddCommand(attack);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            GetComponent<Hero>().IsAI = !GetComponent<Hero>().IsAI;
        }
        
    }
}
