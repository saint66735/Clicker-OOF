using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

class Building
{
    string name;
    double production;
    double cost;
    public Building(string name ,double production, double cost)
    {
        this.name = name;
        this.production = production;
        this.cost = cost;
    }
}
class BuildingInstance
{
    Building building;
    int level;
    public BuildingInstance(Building building)
    {
        this.building = building;
        level = 0;
    }
}
class Province
{
    public string name;
    public double cost;
    public double baseProduction;
    public List<BuildingInstance> buildings;
    public bool unlocked;

    public Province(string name, double cost, double baseProduction)
    {
        this.name = name;
        this.cost = cost;
        this.baseProduction = baseProduction;
        unlocked = false;
    }
}
public class GameLogic : MonoBehaviour {

    List<Province> provinceData;
    List<Building> buildings;
    public List<GameObject> provinces;
    public Text provinceDisplay;
    public int currentProvince;
    // Use this for initialization
    void Start () {
        provinceData = new List<Province>();
        buildings = new List<Building>();
        ReadBuildingData();
        ReadProvinceData();
        BuildProvince();
	}
	
	// Update is called once per frame
	void Update () {
        provinceDisplay.text ="" + currentProvince;
	}

    void ReadProvinceData()
    {
        string[] lines = File.ReadAllLines("Assets//ProvinceData.txt");
        foreach (var line in lines)
        {
            string[] parts = line.Split(' ');
            provinceData.Add(new Province(parts[0], double.Parse(parts[1]), double.Parse(parts[2])));
        }
    }
    void BuildProvince()
    {
        foreach (var province in provinceData)
        {
            province.buildings = new List<BuildingInstance>();
            foreach (var building in buildings)
            {
                BuildingInstance temp = new BuildingInstance(building);
                province.buildings.Add(temp);
            }
        }
    }
    void ReadBuildingData()
    {
        string[] lines = File.ReadAllLines("Assets//BuildingData.txt");
        foreach (var line in lines)
        {
            string[] parts = line.Split(' ');
            buildings.Add(new Building(parts[0], double.Parse(parts[1]), double.Parse(parts[2])));
        }
    }
}
