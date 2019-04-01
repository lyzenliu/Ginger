#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common;
using Ginger;
using Ginger.BusinessFlowsLibNew.AddActionMenu;
using Ginger.Run;
using GingerCore;
using GingerCore.Environments;
using GingerCore.GeneralLib;
using GingerCoreNET.RunLib;
using GingerWPF.GeneralLib;
using GingerWPF.RunLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace GingerWPF.BusinessFlowsLib
{
    /// <summary>
    /// Interaction logic for BusinessFlowPage.xaml
    /// </summary>
    public partial class NewAutomatePage : Page
    {
        BusinessFlow mBusinessFlow;
        GingerRunner mRunner;
        Context mContext = new Context();

        GridLength mLastAddActionsColumnWidth = new GridLength(270);

        public NewAutomatePage(BusinessFlow businessFlow)
        {
            InitializeComponent();

            mRunner = new GingerRunner(eExecutedFrom.Automation);
            mRunner.CurrentBusinessFlow = businessFlow;
            mContext.Runner = mRunner;
            mBusinessFlow = businessFlow;
            mContext.BusinessFlow = mBusinessFlow;

            //Binding
            BusinessFlowNameLabel.BindControl(mBusinessFlow, nameof(BusinessFlow.Name));
            // TODO: break it down to each folder and show parts with hyperlink
            FlowPathLabel.Content = mBusinessFlow.ContainingFolder;
            EnvironmentComboBox.ItemsSource = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<ProjEnvironment>();
            EnvironmentComboBox.DisplayMemberPath = nameof(ProjEnvironment.Name);

            if (mBusinessFlow.CurrentActivity == null && mBusinessFlow.Activities.Count > 0)
            {
                mBusinessFlow.CurrentActivity = mBusinessFlow.Activities[0];
            }

            
            ActivitiesList.ItemsSource = mBusinessFlow.Activities;


            App.PropertyChanged += App_PropertyChanged;

            xCurrentActivityFrame.Content = new ActivityPage((Activity)mBusinessFlow.Activities[0], mContext);  // TODO: use binding? or keep each activity page

            InitGingerRunnerControls();

            xAddActionMenuFrame.Content = new MainAddActionsNavigationPage(mContext);
        }

        internal void GoToBusFlowsListHandler(object v)
        {
            throw new NotImplementedException();
        }

        private void App_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        static GingerRunnerControlsPage mGingerRunnerControlsPage;
        private void InitGingerRunnerControls()
        {
            // TODO: if this page is going to be used as standalone pass the controls page as input
            if (mGingerRunnerControlsPage == null)
            {
                mGingerRunnerControlsPage = new GingerRunnerControlsPage(mRunner);
            }
            //GingerRunnerControlsFrame.Content = mGingerRunnerControlsPage;
        }

        private void GingerRunner_GingerRunnerEvent(GingerRunnerEventArgs EventArgs)
        {
            switch (EventArgs.EventType)
            {
                case GingerRunnerEventArgs.eEventType.ActivityStart:
                    Activity a = (Activity)EventArgs.Object;
                    // Just to show we can display progress
                    this.Dispatcher.Invoke(() =>
                    {
                        //StatusLabel.Content = "Running " + a.ActivityName;
                    });

                    break;
                case GingerRunnerEventArgs.eEventType.ActionEnd:
                    this.Dispatcher.Invoke(() =>
                    {
                        // just quick code to show activity progress..
                        int c = (from x in mBusinessFlow.Activities where x.Status != Amdocs.Ginger.CoreNET.Execution.eRunStatus.Pending select x).Count();
                        //ProgressBar.Maximum = mBusinessFlow.Activities.Count;
                        //ProgressBar.Value = c;
                    });
                    break;
            }
        }

        private void CurrentBusinessFlow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BusinessFlow.CurrentActivity))
            {
                ActivitiesList.Dispatcher.Invoke(() =>
                {
                    ActivitiesList.SelectedItem = mBusinessFlow.CurrentActivity;
                });

            }
        }

        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
            List<ActionSelectorItem> actions = new List<ActionSelectorItem>();
            actions.Add(new ActionSelectorItem() { Name = "Add Activity using recording", Action = AddActivity });
            actions.Add(new ActionSelectorItem() { Name = "Add Empty Activity", Action = AddActivity });
            actions.Add(new ActionSelectorItem() { Name = "Add Activity from shared repository", Action = AddActivity });

            ActionSelectorWindow w = new ActionSelectorWindow("What would you like to add?", actions);
            w.Show();
        }

        private void AddActivity()
        {
            Activity activity = new Activity();
            bool b = InputBoxWindow.OpenDialog("Add new Activity", "Activity Name", activity, nameof(Activity.ActivityName));
            if (b)
            {
                mBusinessFlow.Activities.Add(activity);
            }
        }

        private void ActivitiesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Activity SelectedActivity = (Activity)ActivitiesList.SelectedItem;
            
            mRunner.CurrentBusinessFlow.CurrentActivity = SelectedActivity;
            mRunner.CurrentBusinessFlow.CurrentActivity = SelectedActivity;
            if (SelectedActivity.Acts.CurrentItem == null && SelectedActivity.Acts.Count > 0)
            {
                SelectedActivity.Acts.CurrentItem = SelectedActivity.Acts[0];
            }
            xCurrentActivityFrame.Content = new ActivityPage(SelectedActivity, mContext);
        }

        private void BusinessFlowsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            WorkSpace.Instance.EventHandler.ShowBusinessFlows();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            WorkSpace.Instance.SolutionRepository.SaveRepositoryItem(mBusinessFlow);
            //TODO: show message item save or Ginger Helper
        }

        public void GoToBusFlowsListHandler(RoutedEventHandler clickHandler)
        {
            xStepBack.Click += clickHandler;
        }

        private void XAddActionsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (xAddActionsBtn.ButtonImageType == Amdocs.Ginger.Common.Enums.eImageType.Add)
            {
                //Expand
                xAddActionsColumn.Width = mLastAddActionsColumnWidth;
                xAddActionsBtn.ButtonImageType = Amdocs.Ginger.Common.Enums.eImageType.ArrowRight;
                xAddActionsBtn.ToolTip = "Collapse Add Actions Section";
                xAddActionsBtn.ButtonStyle = (Style)FindResource("$AddActionsMenuBtnStyle");
                xAddActionSectionSpliter.IsEnabled = true;
            }
            else
            {
                //Collapse
                mLastAddActionsColumnWidth = xAddActionsColumn.Width;
                xAddActionsColumn.Width = new GridLength(10);
                xAddActionsBtn.ButtonImageType = Amdocs.Ginger.Common.Enums.eImageType.Add;
                xAddActionsBtn.ToolTip = "Add Actions";
                xAddActionsBtn.ButtonStyle = (Style)FindResource("$AddActionsMenuBtnStyle");
                xAddActionSectionSpliter.IsEnabled = false;
            }
        }
    }
}
