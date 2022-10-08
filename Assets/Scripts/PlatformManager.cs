using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace extOSC.Examples
{
    public class PlatformManager : MonoBehaviour
    {
        private int _width = 120, _height = 50;
        public string Address = "/test/1";
        public GameObject planeGO, planeLeftGO, planeRightGO;

        //int[,] data = new int[60, 50];

        private Dictionary<(int, int), int> m_platformData;

        [Header("OSC Settings")]
        public OSCReceiver Receiver;
        

        protected virtual void Start()
        {
            m_platformData = new Dictionary<(int, int), int>();

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    m_platformData[(x, y)] = 0;
                }
            }

            Receiver.Bind(Address, ReceivedMessage);

        }

        private void ReceivedMessage(OSCMessage message)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var id = _height * x + y;
                    var val = message.Values[0].BlobValue;
                    //y = 49 - y;
                    m_platformData[(x, y)] = val[id];
                }
            }


            //int x = message.Values[0].IntValue;
            //int y = message.Values[1].IntValue;
            //int value = message.Values[2].IntValue;
            ////data[x, y] = value;

            //if(value>50) m_platformData[(x, y)] = value;
            //else m_platformData[(x, y)] = 0;
        }

        public Dictionary<(int, int), int> GetPlatformData()
        {
            return m_platformData;
        }

        //void Update()
        //{
        //    Texture2D texture = new Texture2D(60, 50);
        //    planeGO.GetComponent<Renderer>().material.mainTexture = texture;
        //    for (var x = 0; x < 60; x++)
        //    {
        //        for (var y = 0; y < 50; y++)
        //        {
        //            texture.SetPixel(x, y, new Color(data[x, y], 0, 0));
        //        }
        //    }
        //    texture.Apply();
        //    Array.Clear(data, 0, data.Length);

        //Texture2D textureLeft = new Texture2D(60, 50);
        //Texture2D textureRight = new Texture2D(60, 50);
        //planeLeftGO.GetComponent<Renderer>().material.mainTexture = textureLeft;
        //planeRightGO.GetComponent<Renderer>().material.mainTexture = textureRight;
        //for (var x = 0; x < 120; x++)
        //{
        //    for (var y = 0; y < 50; y++)
        //    {
        //        if (x < 60) textureLeft.SetPixel(x, y, new Color(data[x, y], 0, 0));
        //        else textureRight.SetPixel(x, y, new Color(data[x, y], 0, 0));
        //    }
        //}
        //textureLeft.Apply();
        //textureRight.Apply();
        //Array.Clear(data, 0, data.Length);

        //}
    }
}
