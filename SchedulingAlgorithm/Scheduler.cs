using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SchedulingAlgorithm
{
	public class Group
	{
		public string Name;
		public int Rank;

		public Group(string N, int R)
		{
			Name = N;
			Rank = R;
		}
	}

	public class Availability
	{
		public int DayNumber;
		public List<int> Slots;

		public Availability(int D, List<int> S)
		{
			DayNumber = D;
			Slots = S;
		}
	}

	public class Station
	{
		public string Name;
		public int Capacity;
		public List<Availability> Avail;

		public Station(string N, int C, List<Availability> A)
		{
			Name = N;
			Capacity = C;
			Avail = A;
		}
	}

	public static class Scheduler
	{
		public static Dictionary<string, string>[,] Schedule(List<Group> groups, List<Station> stations, int slotsPerDay)
		{
			// start monday end Friday
			int dayStart = 1, dayEnd = 5;
			int Day, Slot, i,j, k;

			// [GroupNumber, StationNumber] = Assignment Counter
			int[,] GroupStationAssignments = new int[100, 100];

			// Schedule
			Dictionary<string, string>[,] masterSchedule = new Dictionary<string, string>[10, 20];

			for (i = 0; i < GroupStationAssignments.GetLength(0); i++)
				for (j = 0; j < GroupStationAssignments.GetLength(1); j++)
					GroupStationAssignments[i, j] = 0;

			for (Day = dayStart; Day <= dayEnd; Day++)
			{
				for (Slot = 1; Slot <= slotsPerDay; Slot++)
				{
					masterSchedule[Day, Slot] = new Dictionary<string, string>();

					// Group busy
					bool[] isGroupBusy = new bool[100];

					for (i = 0; i < stations.Count; i++)
					{
						Station curStation = stations[i];

						if (!isStationAvailableAtSlot(stations[i], Day, Slot))
							continue;

						int counter = 1;

						for (j = 0; j < groups.Count; j++)
						{
							if (counter > curStation.Capacity)
								break;

							Group curGroup = groups[j];

							if (GroupStationAssignments[j, i] >= 2 || isGroupBusy[j])
								continue;

							// Group is busy in this slot
							isGroupBusy[j] = true;
							GroupStationAssignments[j, i]++;

							counter++;

							masterSchedule[Day, Slot].Add(curGroup.Name, curStation.Name);
						}
					}
				}
			}

			return masterSchedule;
		}

		private static bool isStationAvailableAtSlot(Station station, int Day, int Slot)
		{
			int i, j;

			if (station.Capacity <= 0)
				return false;

			for (i = 0; i < station.Avail.Count; i++)
			{
				if (station.Avail[i].DayNumber == Day)
				{
					if (station.Avail[i].Slots.IndexOf(Slot) != -1)
						return true;
				}
			}

			return false;
		}
	}
}
