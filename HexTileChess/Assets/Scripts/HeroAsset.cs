using System.Collections.Generic;
using UnityEngine;


// 英雄表
[CreateAssetMenu(fileName = "heroAsset", menuName = "XXX Game/Hero")]
public class HeroAsset : ScriptableObject
{
    public List<HeroTemplate> heros = new List<HeroTemplate>();

    public HeroTemplate GetData(int heroID)
    {
        foreach (var hero in heros)
        {
            if (hero.heroID == heroID)
            {
                return hero;
            }
        }
        return null;
    }

    public void RandomHeros(ref List<HeroTemplate> ret, int count)
    {
        ret.Clear();
        for (int i = 0; i < count; i++)
        {
            ret.Add(heros[Random.Range(0, heros.Count)]);
        }
    }
}
