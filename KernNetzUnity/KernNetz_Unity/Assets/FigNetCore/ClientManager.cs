﻿using FigNet.Core;
using UnityEngine;

namespace FigNet
{
    public class ClientManager : MonoBehaviour, IClient
    {
        private void Start()
        {
            SetUp();
        }

        private void OnDestroy()
        {
            TearDown();
        }
        private void Update()
        {
            Process();
        }

        private void OnApplicationQuit()
        {
            foreach (var client in FN.Connections)
            {
                if (client.IsConnected) client.Disconnect();
            }
            FN.Deinitialize();
        }

        public void SetUp()
        {
            var settings = Resources.Load<FigNetConfiguration>("FigNet_Configuration").Config;
            FN.Logger = new DefaultLogger();
            //FN.SetFactories(new SocketFactory(), new PeerFactory());
            
            FN.Initilize(settings);

        }

        public void Process()
        {
            for (int i = 0; i < FN.Connections?.Count; i++)
            {
                FN.Connections[i].Process();
            }
        }

        public void TearDown()
        {
            FN.Deinitialize();
        }
    }

}
