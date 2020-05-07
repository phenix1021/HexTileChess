using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Phenix.Unity.Collection;

public class Data
{
    public int a;
}
struct ABC
{
    public int a;

    public static bool operator ==(ABC l1, ABC l2) { return l1.a == l2.a; }
    public static bool operator !=(ABC l1, ABC l2) { return l1.a != l2.a; }

    public override bool Equals(object obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class test1 : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            List<HexTileAgent> path = new List<HexTileAgent>();
            HexGridAgent.Instance.FindPath(GetComponent<Hero>().tileComp,
                HexGridAgent.Instance.TileCompFromCoords(new Phenix.Unity.Grid.HexCoordinates(1, -1, 0)), 
                ref path);
            List<Vector3> vPath = new List<Vector3>();
            foreach (var item in path)
            {
                vPath.Add(item.tile.Center);
            }
            CommandMarch march = new CommandMarch(GetComponent<Hero>(), vPath);
            GetComponent<Hero>().AddCommand(march);                   
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            CommandAttack attack = new CommandAttack(GetComponent<Hero>(), 
                new Vector3(-2.0f, 0.6f, 2.3f), 3, false);
            GetComponent<Hero>().AddCommand(attack);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            List<HexTileAgent> path = new List<HexTileAgent>();            
            HexGridAgent.Instance.FindPath(GetComponent<Hero>().tileComp,
                HexGridAgent.Instance.TileCompFromPosition(new Vector3(-5.5f, 0.6f, 5.3f)),
                ref path);
            Debug.Log("================================");
            foreach (var item in path)
            {
                Debug.Log(item.tile.Coords);
            }
            Debug.Log("================================");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha7))
        {            
            Debug.Log(HexGridAgent.Instance.Distance(HexGridAgent.Instance.TileCompFromCoords(new Phenix.Unity.Grid.HexCoordinates(3, -7, 4)),
                HexGridAgent.Instance.TileCompFromCoords(new Phenix.Unity.Grid.HexCoordinates(6, -9, 3))));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            MaxHeap<Data> heap = new MaxHeap<Data>((x, y) => { return x.a.CompareTo(y.a); });
            heap.Add(new Data() { a = 5 });
            heap.Add(new Data() { a = 3 });
            heap.Add(new Data() { a = 9 });
            heap.Add(new Data() { a = 7 });
            heap.Add(new Data() { a = 7 });
            heap.Add(new Data() { a = 9 });
            heap.Add(new Data() { a = 9 });
            heap.Add(new Data() { a = 6 });
            Data dd = new Data() { a = 8 };
            heap.Add(dd);
            Data ret = null;
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            //heap.Pop();
            dd.a = 99;
            heap.OnChanged(dd);
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            /*heap.Pop();
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            heap.Pop();
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            heap.Pop();
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            heap.Pop();
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            heap.Pop();
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            heap.Pop();
            heap.Peek(ref ret);
            Debug.Log(ret.a);
            heap.Pop();
            heap.Peek(ref ret);
            Debug.Log(ret.a);       */     
        }
        else if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            GetComponent<Hero>().IsAI = !GetComponent<Hero>().IsAI;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            int val = 0;
            MinHeap<int> minHeap = new MinHeap<int>((x, y) => x.CompareTo(y));
            minHeap.Add(new List<int> { 7, 1, 8, 2, 6, 5, 3, 4, 9 });
            Debug.Log("======MIN HEAP========");
            minHeap.Replace(6, 11);
            minHeap.Remove(4);
            while (minHeap.Count > 0)
            {
                minHeap.Peek(ref val);
                Debug.Log(val);
                minHeap.Pop();
            }

            MaxHeap<int> maxHeap = new MaxHeap<int>((x, y) => x.CompareTo(y));
            maxHeap.Add(new List<int> { 7, 1, 8, 2, 6, 5, 3, 4, 9 });
            Debug.Log("======MAX HEAP========");
            maxHeap.Replace(6, 11);
            maxHeap.Remove(4);
            while (maxHeap.Count > 0)
            {
                maxHeap.Peek(ref val);
                Debug.Log(val);
                maxHeap.Pop();
            }

            HashSet<Data> h1 = new HashSet<Data>();
            Data d1 = new Data();
            d1.a = 1;            
            Data d2 = new Data();
            d2.a = 1;
            h1.Add(d1);
            h1.Add(d2);
            h1.Add(d1);

            HashSet<ABC> h2 = new HashSet<ABC>();
            ABC abc1, abc2;
            abc1.a = 1;
            abc2.a = 1;
            h2.Add(abc1);
            h2.Add(abc2);
            h2.Add(abc1);

            Debug.Log(d1 == d2);
            Debug.Log(d1.Equals(d2));
            //Debug.Log(abc1 == abc2);
            Debug.Log(abc1.Equals(abc2));
        }
    }
}
