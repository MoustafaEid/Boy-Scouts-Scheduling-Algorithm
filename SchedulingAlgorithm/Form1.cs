using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
			G.Add(new Group("Knights 2", 4));
			G.Add(new Group("Knights 3", 2));
			G.Add(new Group("Knights 66", 1));

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

			Dictionary<string, string>[,] A = Scheduler.Schedule(G, S, 5);

			printSchedule(A);
		}

		private void printSchedule(Dictionary<string, string>[,] masterSchedule)
		{
			int i, j, k;
			richTextBox1.Text = "";

			for (i = 1; i <= 5; i++)
			{
				richTextBox1.Text += "Day " + i + "\n----------------------------\n";

				for (j = 1; j <= 5; j++)
				{
					richTextBox1.Text += "\t Slot #" + j + "\n\t-----------\n";

					Dictionary<string, string> D = masterSchedule[i, j];

					foreach (KeyValuePair<string, string> P in D)
					{
						richTextBox1.Text += "\t\t" + P.Key + " Assigned To " + P.Value + "\n";
					}
				}
			}
		}
	}
}
