using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataList
{

    [System.Serializable]
    public class LevelTypeData
    {
        public string levelType;
        public List<int> levelParametres;
    }

    [System.Serializable]
    public class ColorData
    {
        public string colorName;
        public int amount;
    }

    [System.Serializable]
    public class GeneralDataStructure
    {
        public LevelTypeData level;
        public int winIndex;
        public List<ColorData> colorsInLevel;
    }

    public GeneralDataStructure[] levelsArray;
}