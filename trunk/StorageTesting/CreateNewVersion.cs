﻿using System;
using System.Collections.Generic;
using OpenDMS.Networking.Http.Methods;
using OpenDMS.Storage.Providers;
using OpenDMS.Storage.Providers.CouchDB;
using OpenDMS.Storage.Data;

namespace StorageTesting
{
    public class CreateNewVersion : TestBase
    {
        private DateTime _start;

        public CreateNewVersion(FrmMain window, IEngine engine, IDatabase db)
            : base(window, engine, db)
        {
        }

        public override void Test()
        {
            FrmGetResource win = new FrmGetResource();
            win.OnGoClick += new FrmGetResource.GoDelegate(win_OnGoClick);
            win.ShowDialog();
        }
        
        void win_OnGoClick(string id)
        {
            OpenDMS.Storage.Providers.EngineRequest request = new OpenDMS.Storage.Providers.EngineRequest();
            request.Engine = _engine;
            request.Database = _db;
            request.OnActionChanged += new EngineBase.ActionDelegate(EngineAction);
            request.OnProgress += new EngineBase.ProgressDelegate(Progress);
            request.OnComplete += new EngineBase.CompletionDelegate(Complete);
            request.OnTimeout += new EngineBase.TimeoutDelegate(Timeout);
            request.OnError += new EngineBase.ErrorDelegate(Error);
            request.AuthToken = _window.Session.AuthToken;
            request.RequestingPartyType = OpenDMS.Storage.Security.RequestingPartyType.User;

            Clear();

            //OpenDMS.Storage.Data.Metadata resourceMetadata = new OpenDMS.Storage.Data.Metadata();

            //resourceMetadata.Add("$creator", "lucas");
            //resourceMetadata.Add("$created", DateTime.Now);

            //versionMetadata.Add("$creator", "lucas");
            //versionMetadata.Add("$created", DateTime.Now);

            System.IO.FileStream fs = new System.IO.FileStream("testdoc.txt", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None, 8192, System.IO.FileOptions.None);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("This is a test content file.");
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            
            OpenDMS.Storage.Data.Metadata versionMetadata = new OpenDMS.Storage.Data.Metadata();
            OpenDMS.Storage.Data.Content versionContent;
            OpenDMS.Storage.Data.Version version;
            
            versionMetadata.Add("$creator", "lucas");
            versionMetadata.Add("$created", DateTime.Now);
            versionContent = new OpenDMS.Storage.Data.Content(bytes.Length, new OpenDMS.Storage.Data.ContentType("text/plain"), "testdoc.txt");

            version = new OpenDMS.Storage.Data.Version(
                new VersionId(new ResourceId(id)), versionMetadata, versionContent);

            WriteLine("Starting CreateNewVersion test...");
            _start = DateTime.Now;

            _engine.CreateNewVersion(request, version);
        }

        private void EngineAction(EngineRequest request, EngineActionType actionType, bool willSendProgress)
        {
            if (willSendProgress)
                WriteLine("CreateNewVersion.EngineAction - Type: " + actionType.ToString() + " Expecting Progress Reports.");
            else
                WriteLine("CreateNewVersion.EngineAction - Type: " + actionType.ToString() + " NOT Expecting Progress Reports.");
        }

        private void Progress(EngineRequest request, OpenDMS.Networking.Http.DirectionType direction, int packetSize, decimal sendPercentComplete, decimal receivePercentComplete)
        {
            WriteLine("CreateNewVersion.Progress - Sent: " + sendPercentComplete.ToString() + " Received: " + receivePercentComplete.ToString());
        }

        private void Complete(EngineRequest request, ICommandReply reply, object result)
        {
            DateTime stop = DateTime.Now;
            TimeSpan duration = stop - _start;
            Tuple<Resource, OpenDMS.Storage.Data.Version> res = (Tuple<Resource, OpenDMS.Storage.Data.Version>)result;

            WriteLine("CreateNewVersion.Complete - Resource: " + res.Item1.ResourceId.ToString() + " and Version: " + res.Item2.VersionId.ToString() + " successfully created in " + duration.TotalMilliseconds.ToString() + "ms.");
        }

        private void Timeout(EngineRequest request)
        {
            WriteLine("CreateNewVersion.Timeout - Timeout.");
        }

        private void Error(EngineRequest request, string message, Exception exception)
        {
            WriteLine("CreateNewVersion.Error - Error.  Message: " + message);
        }
    }
}
