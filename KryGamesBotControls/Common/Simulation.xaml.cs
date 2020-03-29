﻿using DevExpress.Xpf.Dialogs;
using DoormatBot.Helpers;
using DoormatBot.Strategies;
using DoormatCore.Sites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KryGamesBotControls.Common
{
    /// <summary>
    /// Interaction logic for Simulation.xaml
    /// </summary>
    public partial class Simulation : BaseControl
    {
        Stopwatch SimTimer = new Stopwatch();

        private BaseSite currentsite;

        public BaseSite CurrentSite
        {
            get { return currentsite; }
            set { currentsite = value; }
        }

        private BaseStrategy strategy;

        public BaseStrategy Strategy
        {
            get { return strategy; }
            set { strategy = value; }
        }

        public InternalBetSettings BetSettings { get; set; }

        private DoormatBot.Helpers.Simulation simulation;

        public DoormatBot.Helpers.Simulation CurrentSimulation
        {
            get { return simulation; }
            set { simulation = value; }
        }

        public decimal Progress { get; set; }
        public TimeSpan TimeRunning { get; set; }
        public TimeSpan ProjectedTime { get; set; }
        public TimeSpan ProjectedRemaining { get; set; }

        public Simulation()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {

            CurrentSimulation = new DoormatBot.Helpers.Simulation(txtBalance.Value, (long)txtBets.Value, CurrentSite, Strategy, BetSettings,"tmp.sim",true);
            btnSaveFile.IsEnabled = false;
            CurrentSimulation.OnSimulationWriting += Tmp_OnSimulationWriting;
            CurrentSimulation.OnSimulationComplete += Tmp_OnSimulationComplete;
            SimTimer.Start();
            CurrentSimulation.Start();
            sesseionStats1.Stats = CurrentSimulation.Stats;
        }
        private void Tmp_OnSimulationComplete(object sender, EventArgs e)
        {
            
            SimTimer.Stop();
            DoormatBot.Helpers.Simulation tmp = sender as DoormatBot.Helpers.Simulation;
            sesseionStats1.RefreshStats();
            long ElapsedMilliseconds = SimTimer.ElapsedMilliseconds;
            Progress = (decimal)tmp.TotalBetsPlaced / (decimal)tmp.Bets;
            decimal totaltime = ElapsedMilliseconds / Progress;
            TimeRunning = TimeSpan.FromMilliseconds(ElapsedMilliseconds);
            ProjectedTime = TimeSpan.FromMilliseconds((double)totaltime);
            ProjectedRemaining = TimeSpan.FromMilliseconds((double)totaltime - ElapsedMilliseconds);

            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(TimeRunning));
            OnPropertyChanged(nameof(ProjectedTime));
            OnPropertyChanged(nameof(ProjectedRemaining));
            OnPropertyChanged(nameof(CurrentSimulation));
            SimTimer.Reset();
            btnSaveFile.IsEnabled = true;
        }

        private void Tmp_OnSimulationWriting(object sender, EventArgs e)
        {
            DoormatBot.Helpers.Simulation tmp = sender as DoormatBot.Helpers.Simulation;
            //Console.WriteLine("Simulation Progress: " + tmp.TotalBetsPlaced + " bets of " + tmp.Bets);

            if (tmp.TotalBetsPlaced > 0)
            {
                long ElapsedMilliseconds = SimTimer.ElapsedMilliseconds;
                Progress = (decimal)tmp.TotalBetsPlaced / (decimal)tmp.Bets;

                decimal totaltime = ElapsedMilliseconds / Progress;
                TimeRunning = TimeSpan.FromMilliseconds(ElapsedMilliseconds);
                ProjectedTime = TimeSpan.FromMilliseconds((double)totaltime);
                ProjectedRemaining = TimeSpan.FromMilliseconds((double)totaltime - ElapsedMilliseconds);

                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(TimeRunning));
                OnPropertyChanged(nameof(ProjectedTime));
                OnPropertyChanged(nameof(ProjectedRemaining));
                OnPropertyChanged(nameof(CurrentSimulation));
                sesseionStats1.RefreshStats();
            }
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            DXSaveFileDialog dg = new DXSaveFileDialog();
            if (dg.ShowDialog()??false)
            {
                CurrentSimulation.MoveLog(dg.FileName);
            }
        }
    }
}