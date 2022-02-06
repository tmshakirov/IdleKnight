using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public Road lastSpawned;
    public int spawnCount = 8;
    [SerializeField] private float startingPointZ;

    [Header("Road sections")]
    //Here you need to place all sections of the road in double quantity
    //Здесь нужно разместить все участки дороги в двойном количестве
    public GameObject[] Roads;

    //List of the finished road, you don't need to fill it
    //Список готовой дороги, заполнять не нужно
    private List<GameObject> ReadyRoad = new List<GameObject>();

    [Header("Number of road sections")]
    //Here you need to specify the number of road sections
    //Здесь нужно указать количество участков дорог
    public bool[] numberRoads;

    [Header("Duplication")]
    //Here you need to indicate whether it is possible to repeat the previous section of the road
    //Здесь нужно указать можно ли повторить предыдущий участок дороги
    public bool repeatLastRoad;

    //Current road length
    //Текущая длина дороги
    private int currentCountRoad = 0;

    [Header("Maximum road length")]
    //This variable is the maximum road height
    //Эта переменная отвечает за максимальная длину дороги
    public int maxCountRoad = 8;

    //Current number of the road section
    //Текущий номер участка дороги
    private int currentNumberRoad = -1;

    //The last number of the road section
    //Последний номер участка дороги
    private int lastNumberRoad = -1;

    [Header("Distance between road sections")]
    //Here you need to specify the distance between the road sections, the road sections must be of the same size
    //Здесь нужно указать расстояние между участками дороги, участки дороги должны быть одного размера
    public float distance = 10;

    [Header("Destruction Z-point")]
    //This variable is responsible for the position along the Z axis in which you want to turn off the first section of the road.
    //Эта переменная отвечает за положение по оси Z, в котором нужно выключить первый участок дороги
    public float maxPosZ = -15;

    [Header("Waiting area for road sections")]
    //This zone will contain unused road sections
    //В этой зоне будут неиспользуемые участки дороги
    public Vector3 ZonaWaiting = new Vector3(0, 0, -40);

    [Header("Generator running status")]
    //This variable is responsible for the state of the generator.
    //Эта переменная отвечает за cостояние работы генератора
    public string status = "Generate";

    private void FixedUpdate()
    {
        if(status == "Generate")
        {
            if (currentCountRoad != maxCountRoad)
            {
                currentNumberRoad = Random.Range(0, Roads.Length);
                var curRoad = Roads[currentNumberRoad];
                if (currentNumberRoad < Roads.Length / 2)
                {
                    if (!numberRoads[currentNumberRoad])
                    {
                        if (ReadyRoad.Count > 0)
                        {
                            curRoad.transform.localPosition = ReadyRoad[ReadyRoad.Count - 1].transform.position + new Vector3(0f, 0f, distance);
                            if (spawnCount <= 0)
                            {
                                if (lastSpawned == null)
                                {
                                    lastSpawned = curRoad.GetComponent<Road>();
                                    lastSpawned.LastSpawn();
                                }
                                else
                                {
                                    if (curRoad != lastSpawned.gameObject)
                                    {
                                        curRoad.GetComponent<Road>().Spawn();
                                    }
                                }
                            }
                            else
                            {
                                curRoad.GetComponent<Road>().Spawn();
                            }
                        }
                        else if (ReadyRoad.Count == 0)
                        {
                            curRoad.GetComponent<Road>().cantspawn = true;
                            curRoad.transform.localPosition = new Vector3(0f, curRoad.transform.localPosition.y, startingPointZ);
                            curRoad.GetComponent<Road>().Spawn();
                        }
                        Roads[currentNumberRoad].GetComponent<Road>().number = currentNumberRoad;
                        spawnCount--;
                        numberRoads[currentNumberRoad] = true;
                        lastNumberRoad = currentNumberRoad;
                        currentCountRoad++;
                        ReadyRoad.Add(Roads[currentNumberRoad]);
                    }
                }
                else if (currentNumberRoad >= Roads.Length / 2)
                {
                    if (!numberRoads[currentNumberRoad])
                    {
                        if (ReadyRoad.Count > 0)
                        {
                            curRoad.transform.localPosition = ReadyRoad[ReadyRoad.Count - 1].transform.position + new Vector3(0f, 0f, distance);
                            if (spawnCount <= 0)
                            {
                                if (lastSpawned == null)
                                {
                                    lastSpawned = curRoad.GetComponent<Road>();
                                    lastSpawned.LastSpawn();
                                }
                                else
                                {
                                    if (curRoad != lastSpawned.gameObject)
                                    {
                                        curRoad.GetComponent<Road>().Spawn();
                                    }
                                }
                            }
                            else
                            {
                                curRoad.GetComponent<Road>().Spawn();
                            }
                        }
                        else if (ReadyRoad.Count == 0)
                        {
                            curRoad.GetComponent<Road>().cantspawn = true;
                            curRoad.transform.localPosition = new Vector3(curRoad.transform.localPosition.x, curRoad.transform.localPosition.y, startingPointZ);
                            curRoad.GetComponent<Road>().Spawn();
                        }
                        curRoad.GetComponent<Road>().number = currentNumberRoad;
                        numberRoads[currentNumberRoad] = true;
                        lastNumberRoad = currentNumberRoad;
                        currentCountRoad++;
                        ReadyRoad.Add(curRoad);
                    }
                }
                //curRoad.GetComponent<Road>().Spawn();
            }

            if (lastSpawned == null)
            {
                foreach (GameObject got in ReadyRoad)
                {
                    got.transform.Translate(Vector3.back * GameHandler.Instance.moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                lastSpawned.transform.Translate(Vector3.back * GameHandler.Instance.moveSpeed * Time.deltaTime);
                foreach (GameObject got in ReadyRoad)
                {
                    if (got != lastSpawned.gameObject)
                    {
                        if (got.transform.position.z > lastSpawned.transform.position.z)
                        {
                            //got.transform.position = ZonaWaiting;
                            got.GetComponent<Road>().LastSet(ZonaWaiting);
                        }
                        else
                            got.transform.Translate(Vector3.back * GameHandler.Instance.moveSpeed * Time.deltaTime);
                    }
                }
            }
            if (ReadyRoad[0].transform.position.z < maxPosZ)
            {
                int i;
                ReadyRoad[0].transform.position = ZonaWaiting;
                i = ReadyRoad[0].GetComponent<Road>().number;
                ReadyRoad[0].GetComponent<Road>().Reset();
                ReadyRoad.RemoveAt(0);
                numberRoads[i] = false;
                currentCountRoad--;
                spawnCount--;
            }
        }
    }

    public void ResetGame()
    {
        while (ReadyRoad.Count > 0)
        {
            ReadyRoad.RemoveAt(0);
        }
        for (int i = 0; i < Roads.Length; i++)
        {
            numberRoads[i] = false;
            Roads[i].transform.position = ZonaWaiting;
        }
        currentCountRoad = 0;
        lastNumberRoad = -1;
        currentNumberRoad = -1;
        status = "Generate";
    }
}