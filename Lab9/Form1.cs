using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab9
{
    public partial class Form1 : Form
    {
        public Random rnd = new Random();
        public List<double> stat;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            stat = new List<double>() { 0, 0, 0, 0, 0};
            chart1.Series[0].Points.Clear();
            double probability = (double) prob.Value;
            List<double> probs = new List<double>() { 0, 0, 0, 0, 0 };
            int days = (int)daysNumeric.Value;
            if (probability > 1)
            {
                MessageBox.Show("Probability should be less than 1");
            }
            else
            {
                double e0 = 0, d0 = 0, n = probability;
                probs[4] = 1;
                for(var i = 0; i < probs.Count - 1; i++)
                {
                    probs[i] = n;
                    probs[4] -= n;
                    e0 += (i + 1) * n;
                    d0 += Math.Pow((i + 1), 2) * n;
                    n *= - probability + 1;
                }
                e0 += 4 * probs[4];
                d0 += 16 * probs[4];
                d0 -= Math.Pow(e0, 2);

                var ans = Count(days, probability);
                var mat = average(ans);
                var disp = variance(ans, mat);
                var xiRes = xi(probs, days);
                var matError = countError(mat, e0);
                var dispError = countError(disp, d0);
                Show(ans, mat, disp, xiRes, matError, dispError);
            }
            
        }

        private void Show (List<double> ans, double mat, double disp, double xiRes, double matError, double dispError)
        {
            for (var i = 0; i < ans.Count; i++)
            {
                chart1.Series[0].Points.AddXY(i + 1, ans[i]);
            }
            averageLabel.Text = mat.ToString();
            varietyLabel.Text = disp.ToString();
            xiLabel.Text = xiRes.ToString();
            averageError.Text = matError.ToString();
            varienceError.Text = dispError.ToString();
            chart1.Series[0].IsValueShownAsLabel = true;
        }

        private List<double> Count(int d, double prob)
        {
            List<double> result = new List<double>() { 0, 0, 0, 0, 0};
            for (var i = 0; i < d; i++)
            {
                var current = (int)Math.Truncate(Math.Log(rnd.NextDouble()) / Math.Log((double)(1 - prob)));
                if (current < 5)
                {
                    stat[current]++;
                }
                else
                {
                    stat[4]++;
                }
            }             
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = (double) stat[i] / d;
            }
            return result;
        }
        private double average (List<double> freq)
        {
            double ans = 0;
            for (int i = 0; i < freq.Count; i++)
            {
                ans += (i + 1) * freq[i];
            }
            return ans;
        }

        private double variance (List<double> freq, double e)
        {
            double ans = 0;
            for (int i = 0; i < freq.Count; i++)
                ans += (i + 1) * (i + 1) * freq[i];
            ans -= e * e;

            return ans;
        }

        private double xi (List<double> probs, int days)
        {
            double ans = 0;
            for (int i = 0; i < probs.Count; i++) { 
                ans += Math.Pow(stat[i], 2) / (days * probs[i]);
            }
            ans -= days;

            return ans;
        }

        private double countError (double i, double i0)
        {
            return Math.Abs(i - i0) * 100 / Math.Abs(i0);
        }
    }
}
