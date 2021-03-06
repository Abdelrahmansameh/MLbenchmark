﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}


[Serializable]
public class playerInfo
{
    public float playerx;
    public float playery;
    public bool grounded;
    public bool dead;
    public int id;
    
    public playerInfo(float x, float y, bool g, bool d,int i)
    {
        playerx = x;
        playery = y;
        grounded = g;
        dead = d;
        id = i;
    }
}

[Serializable]
public class platformInfo
{

    public float ULx;
    public float URx;
    public float BRx;
    public float BLx;

    public float ULy;
    public float URy;
    public float BRy;
    public float BLy;

    public platformInfo(float ULx, float URx, float BRx, float BLx, float ULy, float URy, float BRy, float BLy)
    {
        this.ULx = ULx;
        this.URx = URx;
        this.BRx = BRx;
        this.BLx = BLx;

        this.ULy = ULy;
        this.URy = URy;
        this.BRy = BRy;
        this.BLy = BLy;
    }
}

public class AIInterface : MonoBehaviour
{
    private GameObject[] players;
    public GameObject[] platforms;
    public bool aiMode;
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Uri serverUri = new Uri("http://127.0.0.1:5000/hello");
        var request = (HttpWebRequest)WebRequest.Create(serverUri);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        //getPlayerInfo();
        //GetPlatforms();
        if (aiMode){
	    	sendEnv();
	    }
    }
    platformInfo[] GetPlatforms()
    {
        platformInfo[] ret = new platformInfo[10];
        platforms = GameObject.FindGameObjectsWithTag("platform");
        int i = 0;
        {
            foreach (GameObject platform in platforms)
            {
                if (i <10 )
                {
                    GameObject UL;
                    GameObject BL;
                    GameObject UR;
                    GameObject BR;
                    UL = platform.transform.Find("UL").gameObject;
                    BL = platform.transform.Find("BL").gameObject;
                    UR = platform.transform.Find("UR").gameObject;
                    BR = platform.transform.Find("BR").gameObject;
                    platformInfo tmp = new platformInfo(UL.transform.position.x, UR.transform.position.x, BR.transform.position.x, BL.transform.position.x, UL.transform.position.y, UR.transform.position.y, BR.transform.position.y, BL.transform.position.y);
                    ret[i] = tmp;
                    i++;
                }
            }
        }
        return ret;
    }

    playerInfo[] getPlayersInfo()
    {
        playerInfo[] ret = new playerInfo[players.Length];
        int i = 0;
        foreach (GameObject player in players){
            ret[i] = new playerInfo(player.transform.position.x, player.transform.position.y, player.GetComponent<PlayerControl>().Grounded, player.GetComponent<PlayerControl>().Dead, player.GetComponent<PlayerControl>().id);
            i++;
        }
        return ret;
    }

    private string sendEnv()
    {
        playerInfo[] toSend = getPlayersInfo();



        platformInfo[] toSend2 = GetPlatforms();


        string myJson1 = JsonHelper.ToJson(toSend, true);
        string myJson2 = JsonHelper.ToJson(toSend2, true);
        string myJson = "[" + myJson1 + "," + myJson2 + "]";

        var request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:5000/sendenv");
        request.ContentType = "application/json";
        request.Method = "POST";

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(myJson);
        }

        var response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        //print(jsonResponse);
        return jsonResponse;
    }
}
