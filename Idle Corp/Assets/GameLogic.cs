using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

class Building
{
    public string name;
    public float cost;
    public float production;
    
    public Building(string name, float cost, float production )
    {
        this.name = name;
        this.cost = cost;
        this.production = production;
    }
}
class BuildingInstance
{
    public Building building;
    public int level;
    public BuildingInstance(Building building)
    {
        this.building = building;
        level = 0;
    }
}
class Province
{
    public string name;
    public float cost;
    public float baseProduction;
    public List<BuildingInstance> buildings;
    public bool unlocked;
    public float income;
    public float multiplier;
    public float increaseMultiplierCost;

    public Province(string name, float cost, float baseProduction)
    {
        this.name = name;
        this.cost = cost;
        this.baseProduction = baseProduction;
        unlocked = false;
        income = 0;
        multiplier = 1;
        increaseMultiplierCost = 5;
    }
}
public class GameLogic : MonoBehaviour {

    List<Province> provinceData;
    List<Building> buildings;
    public List<GameObject> provinces;
    public Text provinceDisplay;
    public Text moneyDisplay;
    public Dropdown dropdown;
    public Button buyButton;
    public Image panel;
    public Button BuildButton;
    public Text buildingInfo;

    public int currentProvince;
    public float money;
    public float globalMultiplier;

    public float increaseGlobalMultiplierCost;

    float time=0;
    // Use this for initialization
    void Start () {
        provinceData = new List<Province>();
        buildings = new List<Building>();
        ReadBuildingData();
        ReadProvinceData();
        BuildProvince();

        dropdown.options.Clear();
        foreach (var building in buildings)
        {
            dropdown.options.Add(new Dropdown.OptionData(building.name));
        }
        

        money = 25;
        globalMultiplier = 1;
        increaseGlobalMultiplierCost = 20;

    }

    // Update is called once per frame
    void Update () {
        time += Time.deltaTime;
        ProvinceIncome();
        calculateMoney();


        if (!provinceData[currentProvince].unlocked)
        {
            panel.color = new Color(50,50,50);
            buyButton.interactable = true;
            BuildButton.interactable = false;
            provinceDisplay.text = "name:" + provinceData[currentProvince].name + '\n' + "cost:" + provinceData[currentProvince].cost +
                '\n' + "production:" + provinceData[currentProvince].baseProduction;
        }
        if (provinceData[currentProvince].unlocked)
        {
            panel.color = new Color(255, 255, 255);
            buyButton.interactable = false;
            BuildButton.interactable = true;
            provinceDisplay.text = "name:" + provinceData[currentProvince].name + '\n' + "income:" +
                provinceData[currentProvince].income;
             
        }
        buildingInfo.text = "cost:" + buildings[dropdown.value].cost *
            Mathf.Pow(1.1f, provinceData[currentProvince].buildings[dropdown.value].level) + '\n'+"production:" +
            buildings[dropdown.value].production + '\n' + "level:" + 
            provinceData[currentProvince].buildings[dropdown.value].level;
        moneyDisplay.text ="money:"+ money;
	}

    void ReadProvinceData()
    {
        string[] lines = File.ReadAllLines("Assets//ProvinceData.txt");
        foreach (var line in lines)
        {
            string[] parts = line.Split(' ');
            provinceData.Add(new Province(parts[0], float.Parse(parts[1]), float.Parse(parts[2])));
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
            buildings.Add(new Building(parts[0], float.Parse(parts[1]), float.Parse(parts[2])));
        }
    }
    public void Buy()
    {
        if (!provinceData[currentProvince].unlocked && money > provinceData[currentProvince].cost)
        {
            provinceData[currentProvince].unlocked = true;
            money -= provinceData[currentProvince].cost;
        }
    }
    public void Build()
    {
        int id = dropdown.value;
        float cost = provinceData[currentProvince].buildings[id].building.cost *
            Mathf.Pow(1.1f, provinceData[currentProvince].buildings[id].level);
        if (money > cost)
        {
            provinceData[currentProvince].buildings[id].level++;
            money -= cost;
            //provinceData[currentProvince].buildings[id].building.cost*= 1.1f;
        }
    }
    void calculateMoney()
    {
        if (time > 1)
        {
            float income = 0;
            foreach (var province in provinceData)
            {
                income += province.income;
            }
            money += income;
            time = 0;
        }
    }
    void ProvinceIncome()
    {
        foreach (var province in provinceData)
        {
            float income = 0;
            foreach (var building in province.buildings)
            {
                income += (building.building.production * building.level) * province.baseProduction;
            }
            province.income = income * province.multiplier * globalMultiplier;
        }
    }
    public void IncreaseProvinceMultiplier()
    {
        if (money > provinceData[currentProvince].increaseMultiplierCost)
        {
            provinceData[currentProvince].multiplier *= 1.05f;
            money -= provinceData[currentProvince].increaseMultiplierCost;
            provinceData[currentProvince].increaseMultiplierCost *= 1.1f;
        }
    }
    public void IncreaseGlobalMultiplier()
    {
        if (money > increaseGlobalMultiplierCost)
        {
            globalMultiplier *= 1.05f;
            money -= increaseGlobalMultiplierCost;
            increaseGlobalMultiplierCost *= 1.1f;
        }
    }
}
