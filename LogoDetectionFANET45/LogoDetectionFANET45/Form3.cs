﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Json;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.GPU;
using Emgu.CV.UI;

using SharpMap.Base;
using SharpMap.Converters;
using SharpMap.Data;
using SharpMap.Forms;
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Styles;
using SharpMap.Utilities;
using SharpMap.Web;

namespace LogoDetectionFANET45
{
    public partial class Form3 : Form
    {
        SharpMap.Layers.VectorLayer vlay = new SharpMap.Layers.VectorLayer("Indonesia");

        public Form3()
        {
            InitializeComponent();

            fetch_GeoData(dateTimePicker1.Value);
            fetch_ChartDate("Harian");
            comboBox1.Text = "Harian";
        }

        private void fetch_GeoData(DateTime date) {
            List<String> res = new List<String>();
            for (int i = 0; i < db.dates.Count; i++) {
                if (date.Year == db.dates[i].Year && date.Month == db.dates[i].Month && date.Day == db.dates[i].Day)
                {
                    res.Add(db.location[i]);
                }
            }

            int[] colorMap = new int[95];
            foreach (String s in res) {
                for (int i = 0; i < 95; i++) {
                    if (s == i.ToString()) {
                        colorMap[i]++;
                    }
                }
            }

            Dictionary<string, SharpMap.Styles.IStyle> styles = new Dictionary<string, IStyle>();
            VectorStyle detectedArea = new VectorStyle();
            VectorStyle def = new VectorStyle();
            def.Fill = new SolidBrush(Color.FromArgb(255,255,255));
            for(int i = 0; i < colorMap.Length; i++){
                detectedArea.Fill = new SolidBrush(Color.FromArgb(255, colorMap[i] * 30, colorMap[i] * 30));
                if (colorMap[i] > 0){
                    styles.Add(i.ToString(), detectedArea);
                }
                else 
                    styles.Add(i.ToString(), def);
            }

            vlay.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\Users\Onit\Desktop\map.shp", true);
            vlay.Theme = new SharpMap.Rendering.Thematics.UniqueValuesTheme<string>("kode", styles, def);
            mapBox1.Map.Layers.Add(vlay);
            mapBox1.Map.ZoomToExtents();
            mapBox1.Refresh();
        }

        private void fetch_ChartDate(string range) {
            switch (range){
                case "Harian":
                    groupByDays(db.dummyChartStats);
                    break;
                case "Mingguan":
                    groupByWeeks(db.dummyChartStats);
                    break;
                case "Bulanan":
                    groupByMonths(db.dummyChartStats);
                    break;
            }
        }

        private void groupByMonths(List<KeyValuePair<DateTime, int>> _stats) { 

        }

        private void groupByWeeks(List<KeyValuePair<DateTime, int>> _stats) {
            String[] ranges = new String[4];
            int[] counts = new int[4];
            int index = _stats.Count - 1;
            for (int i = ranges.Length - 1; i >= 0; i--)
            {
                ranges[i] = "";
                counts[i] = 0;
                int limit = 7;
                do
                {
                    if (index == 0)
                    {
                        ranges[i] += " s/d " + _stats[index].Key + "\n";
                        goto draw;
                    }
                    if (limit == 7)
                        ranges[i] += _stats[index].Key;
                    if (limit == 1)
                        ranges[i] += " s/d " + _stats[index].Key + "\n";

                    counts[i] = counts[i] + _stats[index].Value;
                    index--;
                    limit--;
                } while (limit > 0);
            }
            draw:
            this.chart1.Series.Clear();
            this.chart1.Palette = ChartColorPalette.Fire;
            for (int i = 0; i < ranges.Length; i++)
            {
                Series series = this.chart1.Series.Add(ranges[i]);
                series.Points.Add((double)counts[i]);
            }
        }

        private void groupByDays(List<KeyValuePair<DateTime, int>> _stats)
        {
            DateTime[] days = new DateTime[7];
            int[] counts = new int[7];
            int index = _stats.Count - 1;
            for (int i = days.Length - 1; i >= 0; i--) {
                days[i] = _stats[index].Key;
                counts[i] = _stats[index].Value;
                index--;
            }
            this.chart1.Series.Clear();
            this.chart1.Palette = ChartColorPalette.Fire;
            for (int i = 0; i < days.Length; i++) {
                Series series = this.chart1.Series.Add(days[i].ToString());
                series.Points.Add((double)counts[i]);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            fetch_GeoData(dateTimePicker1.Value);
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void mapBox1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fetch_ChartDate(comboBox1.Text);
        }
    }
}