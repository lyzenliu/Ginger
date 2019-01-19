﻿using Amdocs.Ginger.Common;
using Amdocs.Ginger.CoreNET.Run.RunListenerLib;
using Amdocs.Ginger.Run;
using Ginger.Run;
using GingerCore;
using GingerCore.Actions;
using GingerTestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GingerCoreNETUnitTest.RunTestslib
{
    [Level2]
    [TestClass]
    public class ExecutionDumperListenerTest
    {
        static GingerRunner mGingerRunner;
        static ExecutionDumperListener mExecutionDumperListener;
        static string mDumpFolder = TestResources.GetTempFolder("ExecutionDumperListener");

        [ClassInitialize]
        public static void ClassInitialize(TestContext TestContext)
        {
            mGingerRunner = new GingerRunner();
            mGingerRunner.RunListeners.Clear(); // temp as long as GR auto start with some listener, remove when fixed
            mExecutionDumperListener = new ExecutionDumperListener(mDumpFolder);
            mGingerRunner.RunListeners.Add(mExecutionDumperListener);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        [TestInitialize]
        public void TestInitialize()
        {            

        }

        [TestCleanup]
        public void TestCleanUp()
        {
            
        }


        [TestMethod]
        public void DumperListener()
        {            
            // We lock Ginger runner so we will not run 2 flows at the same time on same GR
            
            //Arrange
            BusinessFlow businessFlow = new BusinessFlow();
            businessFlow.Activities = new ObservableList<Activity>();
            businessFlow.Name = "BF TEST Execution Dumper Listener";
            businessFlow.Active = true;

            Activity activitiy1 = new Activity() { ActivityName = "a1", Active = true };
            activitiy1.Acts.Add(new ActDummy() { Description = "Dummy action 1", Active = true });
            activitiy1.Acts.Add(new ActDummy() { Description = "Dummy action 2", Active = true });
            activitiy1.Acts.Add(new ActDummy() { Description = "Dummy action 3", Active = true });
            businessFlow.Activities.Add(activitiy1);

            Activity activitiy2 = new Activity() { ActivityName = "a2", Active = true };
            activitiy2.Acts.Add(new ActDummy() { Description = "A2 action 1", Active = true });
            businessFlow.Activities.Add(activitiy2);

            RunFlow(businessFlow);
                

            //Act
            RunListenerBase.Start();
            mGingerRunner.RunRunner();                

            //check folder structure and files contents

            // string folder = mExecutionDumperListener.

            string BFDir = Path.Combine(mDumpFolder, "1 " + businessFlow.Name);

            //Assert


            Assert.IsTrue(Directory.Exists(BFDir), "BF directory exist");
            //TODO: all the rest             
        }

        private void RunFlow(BusinessFlow mBF)
        {
            lock (mGingerRunner)
            {
                mGingerRunner.BusinessFlows.Clear();
                mGingerRunner.BusinessFlows.Add(mBF);
                mGingerRunner.RunBusinessFlow(mBF);
            }
        }

        [TestMethod]
        public void DumperListenerBigFlow()
        {
            // TODO: add more data and check speed
            // We lock Ginger runner so we will not run 2 flows at the same time on same GR
            lock (mGingerRunner)
            {
                //Arrange
                BusinessFlow businessFlow = new BusinessFlow();
                businessFlow.Activities = new ObservableList<Activity>();
                businessFlow.Name = "Big Flow";
                businessFlow.Active = true;

                for (int i = 0; i < 10; i++)
                {
                    Activity activitiy = new Activity() { ActivityName = "activity " + i, Active = true };
                    for (int j = 0; j < 10; j++)
                    {
                        activitiy.Acts.Add(new ActDummy() { Description = "Dummy action " + j, Active = true });
                    }
                    businessFlow.Activities.Add(activitiy);
                }

                mGingerRunner.BusinessFlows.Add(businessFlow);

                //Act
                RunFlow(businessFlow);                

                //check folder structure and files contents

                // string folder = mExecutionDumperListener.

                string BFDir = Path.Combine(mDumpFolder, "1 " + businessFlow.Name);

                //Assert


                Assert.IsTrue(Directory.Exists(BFDir), "BF directory exist");
                //TODO: all the rest 
            }
        }

        [TestMethod]
        public void DumperListenerEmptyFlow()
        {            
            lock (mGingerRunner)
            {
                //Arrange
                BusinessFlow businessFlow = new BusinessFlow();
                businessFlow.Activities = new ObservableList<Activity>();
                businessFlow.Name = "Empty Flow";
                businessFlow.Active = true;

                //Act
                RunFlow(businessFlow);

                string BFDir = Path.Combine(mDumpFolder, "1 " + businessFlow.Name);

                //Assert
                Assert.IsTrue(Directory.Exists(BFDir), "BF directory exist");                
            }
        }

        [TestMethod]
        public void MultiFlows()
        {
            // TODO: add several BFs to GR and run all + verify
            //lock (mGingerRunner)
            //{
            //    //Arrange
            //    BusinessFlow mBF = new BusinessFlow();
            //    mBF.Activities = new ObservableList<Activity>();
            //    mBF.Name = "Empty Flow";
            //    mBF.Active = true;

            //    //Act
            //    RunListenerBase.Start();
            //    mGingerRunner.RunBusinessFlow(mBF);

            //    string BFDir = Path.Combine(mDumpFolder, "1 " + mBF.Name);

            //    //Assert
            //    Assert.IsTrue(Directory.Exists(BFDir), "BF directory exist");
            //}
        }


    }

}

