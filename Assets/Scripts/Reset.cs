using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Net;

public class Reset : MonoBehaviour
{
    public bool reset;
    private GameObject scene;
    // Start is called before the first frame update

    void Start()
    {
        scene = GameObject.FindGameObjectWithTag("_Scene");    
    }

    void Update ()
   {
    
     updatereset();
     if( reset )
     {
            scene.GetComponent<AIInterface>().aiMode = false;
     }
   }
   void updatereset()
    {
        Uri serverUri = new Uri("http://127.0.0.1:5000/reset");
        var request = (HttpWebRequest)WebRequest.Create(serverUri);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string strresponse = reader.ReadToEnd();
        if (strresponse == "True")
        {
            reset = true;
        }
    } 
 }

