﻿using System.Collections;
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
    public float multiplier = 1;
    public float increaseMultiplierCost = 5;

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
    public GameObject[] provinceObjects;
    public Text provinceDisplay;
    public Text moneyDisplay;
    public Dropdown dropdown;
    public Button buyButton;
    public Image panel;
    public Button BuildButton;
    public Text buildingInfo;
    public GameObject factoryPrefab;
    public GameObject planet;
    public Canvas canvas;
    public Canvas menu;

    public int currentProvince;
    public float money;
    public float globalMultiplier;
    float moneyOnClick;

    public float increaseGlobalMultiplierCost;
    float increaseMoneyOnClickCost;
    float buyProvinceCostMultiplier;

    public bool paused = false;
    float time=0;
    // Use this for initialization
    void Start () {
        provinceData = new List<Province>();
        buildings = new List<Building>();
        provinceObjects = new GameObject[55];
        ReadBuildingData();
        ReadProvinceData();
        BuildProvince();

        dropdown.options.Clear();
        foreach (var building in buildings)
        {
            dropdown.options.Add(new Dropdown.OptionData(building.name));
        }
        

        money = 200;
        globalMultiplier = 1;
        increaseGlobalMultiplierCost = 20;
        moneyOnClick = 1;
        increaseGlobalMultiplierCost = 15;
        increaseMoneyOnClickCost = 20;
        buyProvinceCostMultiplier = 1;

        //Save();
        //if(File.Exists("Assets//save.txt")) ReadSave();
    }

    // Update is called once per frame
    void Update () {
        if(!paused)time += Time.deltaTime;
        ProvinceIncome();
        calculateMoney();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Pause();
                paused = true;
            }
            else if (paused)
            {
                Unpause();
                paused = false;
            }
        }
        //provinces[currentProvince].transform.TransformDirection(Vector3.down);
        //Debug.Log(provinces[currentProvince].transform.TransformPoint(Vector3.down));
        //Debug.Log(provinces[currentProvince].transform - Camera.main.transform);
        //Debug.Log( Vector3.Angle(provinces[currentProvince].transform.rotation.eulerAngles, Camera.main.transform.rotation.eulerAngles));
        //Debug.Log(provinces[currentProvince].transform.position);

        if (!provinceData[currentProvince].unlocked)
        {
            panel.color = new Color(50,50,50);
            buyButton.interactable = true;
            BuildButton.interactable = false;
            provinceDisplay.text = "name:" + provinceData[currentProvince].name + '\n' + "cost:" +
                ShortenNumber( provinceData[currentProvince].cost * buyProvinceCostMultiplier) +
                '\n' + "production:" + provinceData[currentProvince].baseProduction;
        }
        if (provinceData[currentProvince].unlocked)
        {
            panel.color = new Color(255, 255, 255);
            buyButton.interactable = false;
            BuildButton.interactable = true;
            provinceDisplay.text = "name:" + provinceData[currentProvince].name + '\n' + "income:" +
                ShortenNumber(provinceData[currentProvince].income);
             
        }
        buildingInfo.text = "cost:" + ShortenNumber( buildings[dropdown.value].cost *
            Mathf.Pow(1.2f, provinceData[currentProvince].buildings[dropdown.value].level))+ '\n'+"production:" +
            ShortenNumber(buildings[dropdown.value].production) + '\n' + "level:" + 
            provinceData[currentProvince].buildings[dropdown.value].level;
        moneyDisplay.text = ShortenNumber(money);// "money:"+ money;
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
        if (!provinceData[currentProvince].unlocked && money > provinceData[currentProvince].cost * buyProvinceCostMultiplier)
        {
            provinceData[currentProvince].unlocked = true;
            money -= provinceData[currentProvince].cost * buyProvinceCostMultiplier;
            //buyProvinceCostMultiplier = Mathf.Pow( buyProvinceCostMultiplier+1, 2);
            buyProvinceCostMultiplier *= 5;
        }
    }
    public void Build()
    {
        int id = dropdown.value;
        float cost = provinceData[currentProvince].buildings[id].building.cost *
            Mathf.Pow(1.2f, provinceData[currentProvince].buildings[id].level);
        if (money > cost)
        {
            provinceData[currentProvince].buildings[id].level++;
            money -= cost;
            //provinceData[currentProvince].buildings[id].building.cost*= 1.1f;
            /*if (provinceObjects[currentProvince] == null)
            {
                GameObject temp = Instantiate(factoryPrefab, new Vector3(), new Quaternion());
                temp.transform.parent = provinces[currentProvince].transform;
                temp.transform.position = provinces[currentProvince].transform.TransformPoint(Vector3.up);
                //temp.transform.LookAt(planet.transform);
                //temp.transform.rotation = Quaternion.Euler(temp.transform.rotation.eulerAngles * 1);
                provinceObjects[currentProvince] = temp;
                
            }*/
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
            provinceData[currentProvince].increaseMultiplierCost *= 1.2f;
        }
    }
    public void IncreaseGlobalMultiplier()
    {
        if (money > increaseGlobalMultiplierCost)
        {
            globalMultiplier *= 1.05f;
            money -= increaseGlobalMultiplierCost;
            increaseGlobalMultiplierCost *= 1.2f;
        }
    }
    public void Click()
    {
        money += moneyOnClick;
    }
    public void UpgradeMoneyOnClick()
    {
        if (money > increaseMoneyOnClickCost)
        {
            moneyOnClick += 1;
            money -= increaseMoneyOnClickCost;
            increaseMoneyOnClickCost *= 3;
        }
    }
    public string ShortenNumber(float n)
    {
        string output = "";
        double l = (double)n;
        if (l > 1000000) output = System.Math.Round(l / 1000000, 3) + "M";
        else if (l > 1000) output = System.Math.Round(l / 1000, 3) + "K";
        else output = System.Math.Round(l) + "";
        return output;
    }
    public void Save()
    {
        using (var fr = File.CreateText("Assets//save.txt"))
        {

            fr.WriteLine("{0} {1} {2} {3} {4} {5}",money, globalMultiplier,moneyOnClick, increaseGlobalMultiplierCost,
                increaseMoneyOnClickCost,buyProvinceCostMultiplier);
            for (int i = 0; i < 55; i++)
            {
                fr.Write("{0} {1} {2} ", provinceData[i].unlocked, provinceData[i].multiplier,
                    provinceData[i].increaseMultiplierCost);
                //Debug.Log(provinceData[i].multiplier);
                foreach (var building in provinceData[i].buildings)
                {
                    fr.Write("{0} ",building.level);
                }
                fr.WriteLine();
            }
        }
    }
    public void ReadSave()
    {
        if (!File.Exists("Assets//save.txt")) return;
        string[] lines = File.ReadAllLines("Assets//save.txt");
        string[] parts = lines[0].Split(' ');

        money = float.Parse( parts[0]);
        globalMultiplier = float.Parse(parts[1]);
        moneyOnClick = float.Parse(parts[2]);
        increaseGlobalMultiplierCost = float.Parse(parts[3]);
        increaseMoneyOnClickCost = float.Parse(parts[4]);
        buyProvinceCostMultiplier = float.Parse(parts[5]);

        for (int i=0;i<54;i++)
        {
            string[] parts1 = lines[i+1].Split(' ');
            //Debug.Log( parts1.Length);
            provinceData[i].unlocked = bool.Parse(parts1[0]);
            provinceData[i].multiplier = float.Parse( parts1[1]);
            provinceData[i].increaseMultiplierCost = float.Parse( parts1[2]);
            for (int d = 0; d < buildings.Count; d++)
            {
                provinceData[i].buildings[d].level = int.Parse(parts1[d+3]);
            }
        }
    }
    public void Pause()
    {
        canvas.enabled = false;
        menu.enabled = true;
    }
    public void Unpause()
    {
        canvas.enabled = true;
        menu.enabled = false;
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void ResetGame()
    {
        provinceData.Clear();
        ReadProvinceData();
        BuildProvince();

        time = 0;
        money = 200;
        globalMultiplier = 1;
        increaseGlobalMultiplierCost = 20;
        moneyOnClick = 1;
        increaseGlobalMultiplierCost = 15;
        increaseMoneyOnClickCost = 20;
        buyProvinceCostMultiplier = 1;
    }
}
