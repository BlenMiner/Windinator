using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public struct BabyEntry
{
    public int Year;

    public string Name;

    public float Percentage;

    public bool IsBoy;

    public bool IsGirl => !IsBoy;
}

public class BabyDataset : MonoBehaviour
{
    [SerializeField] TextAsset m_data;
    
    List<BabyEntry> m_babies = new List<BabyEntry>();

    public ReadOnlyCollection<BabyEntry> Babies {get; private set;}   

    void Awake()
    {
        var csv = m_data.text;
        var lines = csv.Split('\n');

        foreach(var line in lines)
        {
            var values = line.Split(',');

            m_babies.Add(new BabyEntry{
                Year = int.Parse(values[0]),
                Name = values[1],
                Percentage = float.Parse(values[2]),
                IsBoy = values[3] == "boy"
            });
        }

        Babies = m_babies.AsReadOnly();
    }
}
