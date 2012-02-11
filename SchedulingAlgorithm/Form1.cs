using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// IDEA: loop for each group and throw them in the schedule, repeat till full.

namespace SchedulingAlgorithm
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			List<Group> G = new List<Group>();

			G.Add(new Group("Knights 1", 3));
			G.Add(new Group("Knights 2", 3));
			G.Add(new Group("Knights 3", 2));
			G.Add(new Group("Knights 4", 1));
			G.Add(new Group("Knights 5", 3));
			G.Add(new Group("Knights 6", 4));
			G.Add(new Group("Knights 7", 2));
			G.Add(new Group("Knights 8", 1));
			G.Add(new Group("Knights 9", 4));
			G.Add(new Group("Knights 10", 3));
			G.Add(new Group("Knights 11", 4));
			G.Add(new Group("Knights 12", 3));
			G.Add(new Group("Knights 13", 2));
			G.Add(new Group("Knights 14", 1));
			G.Add(new Group("Knights 15", 3));
			G.Add(new Group("Knights 16", 4));
			G.Add(new Group("Knights 17", 2));
			G.Add(new Group("Knights 18", 4));
			G.Add(new Group("Knights 19", 4));
			G.Add(new Group("Knights 20", 1));

			List<Station> S = new List<Station>();


			List<Availability> K = new List<Availability>( new Availability[] 
							{ 
								new Availability( 1, new List<int>( new int [] { 1, 3 ,4 } ) ),
								new Availability( 2, new List<int>( new int [] { 3, 4 } ) ),
								new Availability( 4, new List<int>( new int [] { 1 } ) ),
								new Availability( 5, new List<int>( new int [] { 5 } ) ),
							} );

			S.Add(new Station("Pool", 2, K));

			K = new List<Availability>(new Availability[] 
							{ 
								new Availability( 1, new List<int>( new int [] { 1, 3 ,4 } ) ),
								new Availability( 2, new List<int>( new int [] { 1, 2, 3 ,4 } ) ),
								new Availability( 3, new List<int>( new int [] { 3, 4,5 } ) ),
								new Availability( 4, new List<int>( new int [] { 1,4,5 } ) ),
								new Availability( 5, new List<int>( new int [] { 1,2,3,4,5 } ) ),
							});

			S.Add(new Station("Archery", 1, K));

			K = new List<Availability>(new Availability[] 
							{ 
								new Availability( 1, new List<int>( new int [] { 1, 2, 3 ,4, 5 } ) ),
								new Availability( 2, new List<int>( new int [] { 1,2,3,4,5 } ) ),
								new Availability( 3, new List<int>( new int [] { 1,2,3,4,5 } ) ),
							});

			S.Add(new Station("Shooting", 1, K));

			K = new List<Availability>(new Availability[] 
							{ 
								new Availability( 2, new List<int>( new int [] { 5 } ) ),
								new Availability( 4, new List<int>( new int [] { 1 } ) ),
							});

			S.Add(new Station("Skits", 2, K));

			K = new List<Availability>(new Availability[] 
							{ 
								new Availability( 1, new List<int>( new int [] { 1, 3 ,4 } ) ),
								new Availability( 2, new List<int>( new int [] { 3, 4 } ) ),
								new Availability( 4, new List<int>( new int [] { 1 } ) ),
								new Availability( 5, new List<int>( new int [] { 5 } ) ),
							});

			S.Add(new Station("Karate", 2, K));

			K = new List<Availability>(new Availability[] 
							{ 
								new Availability( 1, new List<int>( new int [] { 1, 2, 3, 4, 5 } ) ),
								new Availability( 2, new List<int>( new int [] { 1, 2, 3, 4, 5 } ) ),
								new Availability( 3, new List<int>( new int [] { 1, 2, 3, 4, 5 } ) ),
								new Availability( 4, new List<int>( new int [] { 1, 2, 3, 4, 5 } ) ),
								new Availability( 5, new List<int>( new int [] { 1, 2, 3, 4, 5 } ) ),
							});

			S.Add(new Station("Tennis", 2, K));

			K = new List<Availability>(new Availability[] 
							{ 
								new Availability( 1, new List<int>( new int [] { 1, 3 ,4 } ) ),
								new Availability( 2, new List<int>( new int [] { 3, 4, 5 } ) ),
								new Availability( 3, new List<int>( new int [] { 1, 4, 5 } ) ),
							});

			S.Add(new Station("Hunting", 2, K));

			K = new List<Availability>(new Availability[] 
							{ 
								new Availability( 1, new List<int>( new int [] { 1, 3 ,4 } ) ),
								new Availability( 2, new List<int>( new int [] { 3, 4 } ) ),
								new Availability( 4, new List<int>( new int [] { 1 } ) ),
								new Availability( 5, new List<int>( new int [] { 5 } ) ),
							});

			S.Add(new Station("Baseball", 2, K));

			Dictionary<int, int>[,] A = Scheduler.Schedule(G, S, 5);

			printSchedule(G,S,A);
		}

		private void printSchedule( List<Group> G, List<Station> S, Dictionary<int, int>[,] masterSchedule)
		{
			int i, j, k;
			richTextBox1.Text = "";

			label1.Text = label2.Text = "";

			foreach (Group g in G)
				label1.Text += "[" + g.Name + ": " + g.Rank + "]    ";
			
			foreach (Station s in S)
				label2.Text += "[" + s.Name + "-" + s.Capacity + "]    ";

			List<RichTextBox> R = new List<RichTextBox>();
			R.Add(richTextBox1);
			R.Add(richTextBox2);
			R.Add(richTextBox3);
			R.Add(richTextBox4);
			R.Add(richTextBox5);

			Dictionary<string, List<string>> GS = new Dictionary<string, List<string>>();
			Dictionary<string, List<string>> SG = new Dictionary<string, List<string>>();

			for (i = 1; i <= 5; i++)
			{
				RichTextBox curR = R[i - 1];

				for (j = 1; j <= 5; j++)
				{
					List<Station> avail = getAvailableStationsInSlot(i, j, S);

					String x = "";

					foreach (Station s in avail)
					{
						if (x.Length > 0)
							x += ", ";

						x += s.Name;
					}

					curR.Text += "Slot #" + j + " ( " + x + " )" + "\n--------------------------\n";

					Dictionary<int, int> D = masterSchedule[i, j];

					foreach (KeyValuePair<int, int> P in D)
					{
						curR.Text += "\t" + G[P.Key].Name + " Assigned To " + S[P.Value].Name + "\n";

						if (!GS.ContainsKey(G[P.Key].Name))
							GS[G[P.Key].Name] = new List<string>();

						if (!SG.ContainsKey(S[P.Value].Name))
							SG[S[P.Value].Name] = new List<string>();

						GS[G[P.Key].Name].Add(S[P.Value].Name);
						SG[S[P.Value].Name].Add(G[P.Key].Name);
					}

					curR.Text += "_________________________________\n\n";
				}
			}

			foreach (KeyValuePair<string, List<string>> q in GS)
			{
				richTextBox6.Text += q.Key + ": ";

				foreach (string x in q.Value)
					richTextBox6.Text += x + ", ";

				richTextBox6.Text += "\n";
			}

			richTextBox6.Text += "\n-------------------------------------------------\n";
			foreach (KeyValuePair<string, List<string>> q in SG)
			{
				richTextBox6.Text += q.Key + ": ";

				foreach (string x in q.Value)
					richTextBox6.Text += x + ", ";

				richTextBox6.Text += "\n";
			}
		}

		private List<Station> getAvailableStationsInSlot(int Day, int Slot, List<Station> S)
		{
			List<Station> ret = new List<Station>();

			foreach (Station s in S)
			{
				foreach (Availability a in s.Avail)
				{
					if (a.DayNumber == Day && a.Slots.IndexOf(Slot) != -1)
						ret.Add(s);
				}
			}
			return ret;
		}
	}
}
