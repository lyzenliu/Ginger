﻿
using Amdocs.Ginger.Common;
using Amdocs.Ginger.Common.InterfacesLib;
using Amdocs.Ginger.Repository;
using Ginger.Run;
using GingerCore;
using GingerCore.Actions;
using GingerCore.Activities;
using GingerCore.Environments;
using GingerCore.Platforms;
using GingerCore.Variables;
using GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib.PlatformsLib;
using GingerTestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GingerTest.Variable_Dependancies
{
    [TestClass]
    [Level1]
    public class VariableDependancyTest
    {
        static BusinessFlow BF1;
        static GingerRunner mGR;
        static ProjEnvironment environment;
        static Context context1;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            BF1 = new BusinessFlow();
            BF1.Name = "VariableDependancyTest";
            BF1.Active = true;

            ActivitiesGroup activityGroup = new ActivitiesGroup();

            Activity activity1 = new Activity();
            activity1.Active = true;

            ActDummy actDummy1 = new ActDummy();
            actDummy1.Active = true;
            ActDummy actDummy2 = new ActDummy();
            actDummy2.Active = true;
            ActDummy actDummy7 = new ActDummy();
            actDummy7.Active = true;
            ActDummy actDummy8 = new ActDummy();
            actDummy8.Active = true;

            activity1.Acts.Add(actDummy1);
            activity1.Acts.Add(actDummy2);
            activity1.Acts.Add(actDummy7);
            activity1.Acts.Add(actDummy8);

            Activity activity2 = new Activity();
            activity2.Active = true;

            ActDummy actDummy3 = new ActDummy();
            actDummy3.Active = true;
            ActDummy actDummy4 = new ActDummy();
            actDummy4.Active = true;

            activity2.Acts.Add(actDummy3);
            activity2.Acts.Add(actDummy4);

            Activity activity3 = new Activity();
            activity3.Active = true;

            ActDummy actDummy5 = new ActDummy();
            actDummy5.Active = true;
            ActDummy actDummy6 = new ActDummy();
            actDummy6.Active = true;

            activity3.Acts.Add(actDummy5);
            activity3.Acts.Add(actDummy6);

            activityGroup.AddActivityToGroup(activity1);
            activityGroup.AddActivityToGroup(activity2);
            activityGroup.AddActivityToGroup(activity3);

            BF1.Activities.Add(activity1);
            BF1.Activities.Add(activity2);
            BF1.Activities.Add(activity3);

            BF1.ActivitiesGroups.Add(activityGroup);

            Platform p = new Platform();
            p.PlatformType = ePlatformType.Web;
            BF1.TargetApplications.Add(new TargetApplication() { AppName = "SCM" });

            mGR = new GingerRunner();
            mGR.Name = "Test Runner";
            mGR.CurrentSolution = new Ginger.SolutionGeneral.Solution();

            environment = new ProjEnvironment();
            environment.Name = "Default";
            BF1.Environment = environment.Name;

            Agent a = new Agent();
            a.DriverType = Agent.eDriverType.SeleniumChrome;

            mGR.SolutionAgents = new ObservableList<Agent>();
            mGR.SolutionAgents.Add(a);

            mGR.ApplicationAgents.Add(new ApplicationAgent() { AppName = "SCM", Agent = a });
            mGR.SolutionApplications = new ObservableList<ApplicationPlatform>();

            mGR.SolutionApplications.Add(new ApplicationPlatform() { AppName = "SCM", Platform = ePlatformType.Web, Description = "New application" });
            mGR.BusinessFlows.Add(BF1);

            context1 = new Context();
            context1.BusinessFlow = BF1;
            context1.Activity = BF1.Activities[0];

            mGR.CurrentBusinessFlow = BF1;
            mGR.CurrentBusinessFlow.CurrentActivity = BF1.Activities[0];
            mGR.Context = context1;
        }

        [TestMethod]
        //[Timeout(60000)]
        public void ActivityVariablesDependancyTest()
        {
            //Arrange
            ObservableList<Activity> activityList = BF1.Activities;

            BF1.EnableActivitiesVariablesDependenciesControl = true;

            //Added selection list variable in BF
            VariableSelectionList selectionList = new VariableSelectionList();
            selectionList.OptionalValuesList.Add(new OptionalValue("a"));
            selectionList.OptionalValuesList.Add(new OptionalValue("b"));
            selectionList.SelectedValue = selectionList.OptionalValuesList[0].Value;
            selectionList.ItemName = "MyVar";
            BF1.Variables.Add(selectionList);

            //Added dependancies in activities
            VariableDependency actiVD0 = new VariableDependency(selectionList.Guid, selectionList.ItemName, new string[] { "a", "b" });
            activityList[0].VariablesDependencies.Add(actiVD0);

            VariableDependency actiVD1 = new VariableDependency(selectionList.Guid, selectionList.ItemName, new string[] { "a", "b" });
            activityList[1].VariablesDependencies.Add(actiVD1);

            VariableDependency actiVD2 = new VariableDependency(selectionList.Guid, selectionList.ItemName, new string[] { "b" });
            activityList[2].VariablesDependencies.Add(actiVD2);
            
            //Act
            mGR.RunBusinessFlow(BF1);

            //Assert
            Assert.AreEqual(BF1.Activities[0].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[1].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[2].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Skipped);

            //Changed the selected value of selection list 
            ((VariableSelectionList)BF1.Variables[0]).SelectedValue = selectionList.OptionalValuesList[1].Value;

            //Act
            mGR.RunBusinessFlow(BF1);

            //Assert
            Assert.AreEqual(BF1.Activities[0].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[1].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[2].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
        }

        [TestMethod]
        [Timeout(60000)]
        public void ActionVariablesDependancyTest()
        {
            //Arrange
            ObservableList<Activity> activityList = BF1.Activities;

            Activity activity = activityList[0];
            ObservableList<IAct> actionList = activity.Acts;

            activity.EnableActionsVariablesDependenciesControl = true;

            //Added selection list variable in activity
            VariableSelectionList selectionList = new VariableSelectionList();
            selectionList.OptionalValuesList.Add(new OptionalValue("abc"));
            selectionList.OptionalValuesList.Add(new OptionalValue("xyz"));
            selectionList.SelectedValue = selectionList.OptionalValuesList[1].Value;
            selectionList.ItemName = "MyActVar";
            activity.Variables.Add(selectionList);

            //added action level dependancies
            VariableDependency actiVD0 = new VariableDependency(selectionList.Guid, selectionList.ItemName, new string[] { "abc" });
            actionList[0].VariablesDependencies.Add(actiVD0);

            VariableDependency actiVD1 = new VariableDependency(selectionList.Guid, selectionList.ItemName, new string[] { "abc", "xyz" });
            actionList[1].VariablesDependencies.Add(actiVD1);

            VariableDependency actiVD3 = new VariableDependency(selectionList.Guid, selectionList.ItemName, new string[] { "abc", "xyz" });
            actionList[2].VariablesDependencies.Add(actiVD3);

            VariableDependency actiVD4 = new VariableDependency(selectionList.Guid, selectionList.ItemName, new string[] { "abc" });
            actionList[3].VariablesDependencies.Add(actiVD4);

            //Act
            mGR.RunActivity(activity);

            //Assert
            Assert.AreEqual(BF1.Activities[0].Acts[0].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Skipped);
            Assert.AreEqual(BF1.Activities[0].Acts[1].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[0].Acts[2].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[0].Acts[3].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Skipped);
            
            //Changed the selected value of selection list
            ((VariableSelectionList)activity.Variables[0]).SelectedValue = selectionList.OptionalValuesList[0].Value;

            //Act
            mGR.RunActivity(activity);

            //Assert
            Assert.AreEqual(BF1.Activities[0].Acts[0].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[0].Acts[1].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[0].Acts[2].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
            Assert.AreEqual(BF1.Activities[0].Acts[3].Status, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed);
        }
    }
}