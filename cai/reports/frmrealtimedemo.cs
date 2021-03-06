﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ChartDirector;

namespace CSharpChartExplorer
{
    public class FrmRealTimeDemo : System.Windows.Forms.Form
    {
        //
        // Data to draw the chart. In this demo, the data buffer will be filled by a data generator, 
        // triggered to run by a timer every 250ms. We plot the last 240 samples.
        //
        private const int sampleSize = 240;
        private DateTime[] timeStamps = new DateTime[sampleSize];
        private double[] dataSeriesA = new double[sampleSize];
        private double[] dataSeriesB = new double[sampleSize];
        private double[] dataSeriesC = new double[sampleSize];

        // Internal variable to record when the data generator is expected to be triggered again
        private DateTime nextDataTime = DateTime.Now;

        //
        // Controls in the form
        // *** Automatically generated by Windows Form Designer ***
        //
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.RadioButton runPB;
        private System.Windows.Forms.RadioButton freezePB;
        private System.Windows.Forms.Label updatePeriodLabel;
        private System.Windows.Forms.NumericUpDown samplePeriod;
        private System.Windows.Forms.Label alarmThresholdLabel;
        private System.Windows.Forms.NumericUpDown alarmThreshold;
        private System.Windows.Forms.Label simulatedMachineLabel;
        private System.Windows.Forms.Label valueALabel;
        private System.Windows.Forms.Label valueA;
        private System.Windows.Forms.Label valueBLabel;
        private System.Windows.Forms.Label valueB;
        private System.Windows.Forms.Label valueCLabel;
        private System.Windows.Forms.Label valueC;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Label separator;
        private ChartDirector.WinChartViewer winChartViewer1;
        private System.Windows.Forms.Timer dataRateTimer;
        private System.Windows.Forms.Timer chartUpdateTimer;
        private System.Windows.Forms.ContextMenu rightClickContextMenu;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private System.ComponentModel.IContainer components;

        public FrmRealTimeDemo()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources used.
        /// *** Automatically generated by Windows Form Designer ***
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Form Load event handler - initialize the form
        /// </summary>
        private void FrmRealTimeDemo_Load(object sender, System.EventArgs e)
        {
            // Data generation rate
            dataRateTimer.Interval = 250;

            // Chart update rate
            chartUpdateTimer.Interval = (int)samplePeriod.Value;

            // Initialize data buffer. In this demo, we just set the initial state to no data.
            for (int i = 0; i < timeStamps.Length; ++i)
                timeStamps[i] = DateTime.MinValue;

            // Enable RunPB button
            runPB.Checked = true;

            // Now can start the timers for data collection and chart update
            dataRateTimer.Start();
            chartUpdateTimer.Start();
        }

        /// <summary>
        /// Enable/disable chart updates when user press the Run/Freeze buttons
        /// </summary>
        private void runPB_CheckedChanged(object sender, System.EventArgs e)
        {
            // Update the colors of the buttons depends on their states
            runPB.BackColor = runPB.Checked ? Color.LightGreen : runPB.Parent.BackColor;
            freezePB.BackColor = freezePB.Checked ? Color.LightGreen : freezePB.Parent.BackColor;

            // Enable chart update if the Run button is active.
            chartUpdateTimer.Enabled = runPB.Checked;
        }

        /// <summary>
        /// Updates the chartUpdateTimer interval if the user selects another interval.
        /// </summary>
        private void samplePeriod_ValueChanged(object sender, System.EventArgs e)
        {
            chartUpdateTimer.Interval = (int)samplePeriod.Value;
        }

        /// <summary>
        /// Updates the threshold value if the user modifies the threshold.
        /// </summary>
        private void alarmThreshold_ValueChanged(object sender, System.EventArgs e)
        {
            winChartViewer1.updateViewPort(true, false);
        }

        /// <summary>
        /// Handler for Right Click/Copy
        /// </summary>
        private void copyMenuItem_Click(object sender, System.EventArgs e)
        {
            // Just copy existing image to the clipboard
            Clipboard.SetDataObject(winChartViewer1.Image, true);
        }

        /// <summary>
        /// The data generator - invoke once every 250ms
        /// </summary>
        private void dataRateTimer_Tick(object sender, System.EventArgs e)
        {
            do
            {
                // This is our formula for the data generator
                double p = nextDataTime.Ticks / 10000000.0 * 4;
                double dataA = 20 + Math.Cos(p * 129241) * 10 + 1 / (Math.Cos(p) * Math.Cos(p) + 0.01);
                double dataB = 150 + 100 * Math.Sin(p / 27.7) * Math.Sin(p / 10.1);
                double dataC = 150 + 100 * Math.Cos(p / 6.7) * Math.Cos(p / 11.9);

                // Now we shift the data into the array
                shiftData(dataSeriesA, dataA);
                shiftData(dataSeriesB, dataB);
                shiftData(dataSeriesC, dataC);
                shiftData(timeStamps, nextDataTime);

                // Update nextDataTime
                nextDataTime = nextDataTime.AddMilliseconds(dataRateTimer.Interval);
            }
            while (nextDataTime < DateTime.Now);

            // We provide some visual feedback to the numbers generated, so you can see the
            // data being updated.
            valueA.Text = dataSeriesA[dataSeriesA.Length - 1].ToString(".##");
            valueB.Text = dataSeriesB[dataSeriesB.Length - 1].ToString(".##");
            valueC.Text = dataSeriesC[dataSeriesC.Length - 1].ToString(".##");
        }

        /// <summary>
        /// Utility to shift a double value into an array
        /// </summary>
        private void shiftData(double[] data, double newValue)
        {
            for (int i = 1; i < data.Length; ++i)
                data[i - 1] = data[i];
            data[data.Length - 1] = newValue;
        }

        /// <summary>
        /// Utility to shift a DataTime value into an array
        /// </summary>
        private void shiftData(DateTime[] data, DateTime newValue)
        {
            for (int i = 1; i < data.Length; ++i)
                data[i - 1] = data[i];
            data[data.Length - 1] = newValue;
        }

        /// <summary>
        /// The chartUpdateTimer Tick event - this updates the chart periodicially.
        /// </summary>
        private void chartUpdateTimer_Tick(object sender, System.EventArgs e)
        {
            winChartViewer1.updateViewPort(true, false);
        }

        /// <summary>
        /// The viewPortChanged event handler.
        /// </summary>
        private void winChartViewer1_ViewPortChanged(object sender, ChartDirector.WinViewPortEventArgs e)
        {
            drawChart(winChartViewer1);
        }

        /// <summary>
        /// Draw the chart and display it in the given viewer.
        /// </summary>
        private void drawChart(WinChartViewer viewer)
        {
            // Create an XYChart object 600 x 270 pixels in size, with light grey (f4f4f4) 
            // background, black (000000) border, 1 pixel raised effect, and with a rounded frame.
            XYChart c = new XYChart(600, 270, 0xf4f4f4, 0x000000, 1);
            c.setRoundedFrame(Chart.CColor(BackColor));

            // Set the plotarea at (55, 62) and of size 520 x 175 pixels. Use white (ffffff) 
            // background. Enable both horizontal and vertical grids by setting their colors to 
            // grey (cccccc). Set clipping mode to clip the data lines to the plot area.
            c.setPlotArea(55, 62, 520, 175, 0xffffff, -1, -1, 0xcccccc, 0xcccccc);
            c.setClipping();

            // Add a title to the chart using 15 pts Times New Roman Bold Italic font, with a light
            // grey (dddddd) background, black (000000) border, and a glass like raised effect.
            c.addTitle("Realtime Chart Demonstration", "Times New Roman Bold Italic", 15
                ).setBackground(0xdddddd, 0x000000, Chart.glassEffect());

            // Add a legend box at the top of the plot area with 9pts Arial Bold font. We set the 
            // legend box to the same width as the plot area and use grid layout (as opposed to 
            // flow or top/down layout). This distributes the 3 legend icons evenly on top of the 
            // plot area.
            LegendBox b = c.addLegend2(55, 33, 3, "Arial Bold", 9);
            b.setBackground(Chart.Transparent, Chart.Transparent);
            b.setWidth(520);

            // Configure the y-axis with a 10pts Arial Bold axis title
            c.yAxis().setTitle("Price (USD)", "Arial Bold", 10);

            // Configure the x-axis to auto-scale with at least 75 pixels between major tick and 15 
            // pixels between minor ticks. This shows more minor grid lines on the chart.
            c.xAxis().setTickDensity(75, 15);

            // Set the axes width to 2 pixels
            c.xAxis().setWidth(2);
            c.yAxis().setWidth(2);

            // Now we add the data to the chart. Access to the data buffer should be synchronized 
            // because the data buffer may be updated by another thread at real time
            DateTime lastTime = timeStamps[timeStamps.Length - 1];
            if (lastTime != DateTime.MinValue)
            {
                // Set up the x-axis to show the time range in the data buffer
                c.xAxis().setDateScale(lastTime.AddSeconds(
                    -dataRateTimer.Interval * timeStamps.Length / 1000), lastTime);

                // Set the x-axis label format
                c.xAxis().setLabelFormat("{value|hh:nn:ss}");

                // Create a line layer to plot the lines
                LineLayer layer = c.addLineLayer2();

                // The x-coordinates are the timeStamps.
                layer.setXData(timeStamps);

                // The 3 data series are used to draw 3 lines. Here we put the latest data
                // values as part of the data set name, so you can see them updated in the
                // legend box.
                layer.addDataSet(dataSeriesA, 0xff0000, "Software: <*bgColor=FFCCCC*>" +
                    c.formatValue(dataSeriesA[dataSeriesA.Length - 1], " {value|2} "));
                layer.addDataSet(dataSeriesB, 0x00cc00, "Hardware: <*bgColor=CCFFCC*>" +
                    c.formatValue(dataSeriesB[dataSeriesB.Length - 1], " {value|2} "));
                layer.addDataSet(dataSeriesC, 0x0000ff, "Services: <*bgColor=CCCCFF*>" +
                    c.formatValue(dataSeriesC[dataSeriesC.Length - 1], " {value|2} "));

                //
                // To show the capabilities of ChartDirector, we are add a movable threshold 
                // line to the chart and dynamically print a warning message on the chart if
                // a data value exceeds the threshold
                //

                // Add a red mark line to the chart, with the mark label shown at the left of
                // the mark line.
                double threshold = (double)alarmThreshold.Value;
                Mark m = c.yAxis().addMark(threshold, 0xff0000, "Alarm = " + threshold);
                m.setAlignment(Chart.Left);
                m.setBackground(0xffcccc);

                if ((dataSeriesC[dataSeriesC.Length - 1] > threshold) ||
                    (dataSeriesB[dataSeriesB.Length - 1] > threshold))
                {
                    // Add an alarm message as a custom text box on top-right corner of the 
                    // plot area if the latest data value exceeds threshold.
                    c.addText(575, 62, "Alarm - Latest Value Exceeded Threshold",
                        "Arial Bold Italic", 10, 0xffffff, Chart.TopRight).setBackground(0xdd0000);
                }

                // Fill the region above the threshold as semi-transparent red (80ff8888)
                c.addInterLineLayer(layer.getLine(1), m.getLine(), unchecked((int)0x80ff8888), Chart.Transparent);
                c.addInterLineLayer(layer.getLine(2), m.getLine(), unchecked((int)0x80ff8888), Chart.Transparent);
            }

            // Set the chart image to the WinChartViewer
            winChartViewer1.Image = c.makeImage();
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRealTimeDemo));
            this.rightClickContextMenu = new System.Windows.Forms.ContextMenu();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.dataRateTimer = new System.Windows.Forms.Timer(this.components);
            this.leftPanel = new System.Windows.Forms.Panel();
            this.alarmThreshold = new System.Windows.Forms.NumericUpDown();
            this.samplePeriod = new System.Windows.Forms.NumericUpDown();
            this.valueC = new System.Windows.Forms.Label();
            this.valueB = new System.Windows.Forms.Label();
            this.valueA = new System.Windows.Forms.Label();
            this.valueCLabel = new System.Windows.Forms.Label();
            this.valueBLabel = new System.Windows.Forms.Label();
            this.valueALabel = new System.Windows.Forms.Label();
            this.simulatedMachineLabel = new System.Windows.Forms.Label();
            this.freezePB = new System.Windows.Forms.RadioButton();
            this.runPB = new System.Windows.Forms.RadioButton();
            this.alarmThresholdLabel = new System.Windows.Forms.Label();
            this.separator = new System.Windows.Forms.Label();
            this.updatePeriodLabel = new System.Windows.Forms.Label();
            this.topLabel = new System.Windows.Forms.Label();
            this.chartUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.winChartViewer1 = new ChartDirector.WinChartViewer();
            this.leftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplePeriod)).BeginInit();
            this.SuspendLayout();
            // 
            // rightClickContextMenu
            // 
            this.rightClickContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								  this.copyMenuItem});
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Index = 0;
            this.copyMenuItem.Text = "Copy";
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            // 
            // dataRateTimer
            // 
            this.dataRateTimer.Tick += new System.EventHandler(this.dataRateTimer_Tick);
            // 
            // leftPanel
            // 
            this.leftPanel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(224)), ((System.Byte)(224)), ((System.Byte)(224)));
            this.leftPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.alarmThreshold,
																					this.samplePeriod,
																					this.valueC,
																					this.valueB,
																					this.valueA,
																					this.valueCLabel,
																					this.valueBLabel,
																					this.valueALabel,
																					this.simulatedMachineLabel,
																					this.freezePB,
																					this.runPB,
																					this.alarmThresholdLabel,
																					this.separator,
																					this.updatePeriodLabel});
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 24);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(120, 286);
            this.leftPanel.TabIndex = 19;
            // 
            // alarmThreshold
            // 
            this.alarmThreshold.Location = new System.Drawing.Point(4, 124);
            this.alarmThreshold.Maximum = new System.Decimal(new int[] {
																		   400,
																		   0,
																		   0,
																		   0});
            this.alarmThreshold.Name = "alarmThreshold";
            this.alarmThreshold.Size = new System.Drawing.Size(112, 20);
            this.alarmThreshold.TabIndex = 4;
            this.alarmThreshold.Value = new System.Decimal(new int[] {
																		 210,
																		 0,
																		 0,
																		 0});
            this.alarmThreshold.ValueChanged += new System.EventHandler(this.alarmThreshold_ValueChanged);
            // 
            // samplePeriod
            // 
            this.samplePeriod.Increment = new System.Decimal(new int[] {
																		   250,
																		   0,
																		   0,
																		   0});
            this.samplePeriod.Location = new System.Drawing.Point(4, 76);
            this.samplePeriod.Maximum = new System.Decimal(new int[] {
																		 2000,
																		 0,
																		 0,
																		 0});
            this.samplePeriod.Minimum = new System.Decimal(new int[] {
																		 250,
																		 0,
																		 0,
																		 0});
            this.samplePeriod.Name = "samplePeriod";
            this.samplePeriod.Size = new System.Drawing.Size(112, 20);
            this.samplePeriod.TabIndex = 3;
            this.samplePeriod.Value = new System.Decimal(new int[] {
																	   1000,
																	   0,
																	   0,
																	   0});
            this.samplePeriod.ValueChanged += new System.EventHandler(this.samplePeriod_ValueChanged);
            // 
            // valueC
            // 
            this.valueC.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.valueC.Location = new System.Drawing.Point(56, 244);
            this.valueC.Name = "valueC";
            this.valueC.Size = new System.Drawing.Size(60, 20);
            this.valueC.TabIndex = 45;
            this.valueC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueB
            // 
            this.valueB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.valueB.Location = new System.Drawing.Point(56, 224);
            this.valueB.Name = "valueB";
            this.valueB.Size = new System.Drawing.Size(60, 20);
            this.valueB.TabIndex = 44;
            this.valueB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueA
            // 
            this.valueA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.valueA.Location = new System.Drawing.Point(56, 204);
            this.valueA.Name = "valueA";
            this.valueA.Size = new System.Drawing.Size(60, 20);
            this.valueA.TabIndex = 43;
            this.valueA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueCLabel
            // 
            this.valueCLabel.Location = new System.Drawing.Point(4, 244);
            this.valueCLabel.Name = "valueCLabel";
            this.valueCLabel.Size = new System.Drawing.Size(48, 20);
            this.valueCLabel.TabIndex = 42;
            this.valueCLabel.Text = "Gamma";
            this.valueCLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueBLabel
            // 
            this.valueBLabel.Location = new System.Drawing.Point(4, 224);
            this.valueBLabel.Name = "valueBLabel";
            this.valueBLabel.Size = new System.Drawing.Size(48, 20);
            this.valueBLabel.TabIndex = 41;
            this.valueBLabel.Text = "Beta";
            this.valueBLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueALabel
            // 
            this.valueALabel.Location = new System.Drawing.Point(4, 204);
            this.valueALabel.Name = "valueALabel";
            this.valueALabel.Size = new System.Drawing.Size(48, 20);
            this.valueALabel.TabIndex = 40;
            this.valueALabel.Text = "Alpha";
            this.valueALabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // simulatedMachineLabel
            // 
            this.simulatedMachineLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.simulatedMachineLabel.Location = new System.Drawing.Point(4, 188);
            this.simulatedMachineLabel.Name = "simulatedMachineLabel";
            this.simulatedMachineLabel.Size = new System.Drawing.Size(112, 16);
            this.simulatedMachineLabel.TabIndex = 38;
            this.simulatedMachineLabel.Text = "Simulated Machine";
            // 
            // freezePB
            // 
            this.freezePB.Appearance = System.Windows.Forms.Appearance.Button;
            this.freezePB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.freezePB.Image = ((System.Drawing.Bitmap)(resources.GetObject("freezePB.Image")));
            this.freezePB.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.freezePB.Location = new System.Drawing.Point(0, 24);
            this.freezePB.Name = "freezePB";
            this.freezePB.Size = new System.Drawing.Size(120, 25);
            this.freezePB.TabIndex = 1;
            this.freezePB.Text = "       Freeze Chart";
            // 
            // runPB
            // 
            this.runPB.Appearance = System.Windows.Forms.Appearance.Button;
            this.runPB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.runPB.Image = ((System.Drawing.Bitmap)(resources.GetObject("runPB.Image")));
            this.runPB.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.runPB.Name = "runPB";
            this.runPB.Size = new System.Drawing.Size(120, 25);
            this.runPB.TabIndex = 0;
            this.runPB.Text = "       Run Chart";
            this.runPB.CheckedChanged += new System.EventHandler(this.runPB_CheckedChanged);
            // 
            // alarmThresholdLabel
            // 
            this.alarmThresholdLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.alarmThresholdLabel.Location = new System.Drawing.Point(4, 108);
            this.alarmThresholdLabel.Name = "alarmThresholdLabel";
            this.alarmThresholdLabel.Size = new System.Drawing.Size(112, 16);
            this.alarmThresholdLabel.TabIndex = 34;
            this.alarmThresholdLabel.Text = "Alarm Threshold";
            // 
            // separator
            // 
            this.separator.BackColor = System.Drawing.Color.Black;
            this.separator.Dock = System.Windows.Forms.DockStyle.Right;
            this.separator.Location = new System.Drawing.Point(119, 0);
            this.separator.Name = "separator";
            this.separator.Size = new System.Drawing.Size(1, 286);
            this.separator.TabIndex = 31;
            // 
            // updatePeriodLabel
            // 
            this.updatePeriodLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.updatePeriodLabel.Location = new System.Drawing.Point(4, 60);
            this.updatePeriodLabel.Name = "updatePeriodLabel";
            this.updatePeriodLabel.Size = new System.Drawing.Size(112, 16);
            this.updatePeriodLabel.TabIndex = 1;
            this.updatePeriodLabel.Text = "Update Period (ms)";
            // 
            // topLabel
            // 
            this.topLabel.BackColor = System.Drawing.Color.Navy;
            this.topLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topLabel.Font = new System.Drawing.Font("Arial", 9.75F, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic), System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.topLabel.ForeColor = System.Drawing.Color.Yellow;
            this.topLabel.Name = "topLabel";
            this.topLabel.Size = new System.Drawing.Size(736, 24);
            this.topLabel.TabIndex = 20;
            this.topLabel.Text = "Advanced Software Engineering";
            this.topLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chartUpdateTimer
            // 
            this.chartUpdateTimer.Tick += new System.EventHandler(this.chartUpdateTimer_Tick);
            // 
            // winChartViewer1
            // 
            this.winChartViewer1.ContextMenu = this.rightClickContextMenu;
            this.winChartViewer1.Location = new System.Drawing.Point(128, 32);
            this.winChartViewer1.Name = "winChartViewer1";
            this.winChartViewer1.Size = new System.Drawing.Size(600, 272);
            this.winChartViewer1.TabIndex = 21;
            this.winChartViewer1.TabStop = false;
            this.winChartViewer1.ViewPortChanged += new ChartDirector.WinViewPortEventHandler(this.winChartViewer1_ViewPortChanged);
            // 
            // FrmRealTimeDemo
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(736, 310);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.winChartViewer1,
																		  this.leftPanel,
																		  this.topLabel});
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmRealTimeDemo";
            this.Text = "ChartDirector Realtime Chart Demonstration";
            this.Load += new System.EventHandler(this.FrmRealTimeDemo_Load);
            this.leftPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alarmThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.samplePeriod)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
