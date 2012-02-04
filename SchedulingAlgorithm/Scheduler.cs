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

	public class Activity
	{
		public Group G;
		public Station S;

		public Activity(Group g, Station s)
		{
			G = g;
			S = s;
		}
	}

	public class TimeSlot
	{
		public List<Activity> Assignments;

		public TimeSlot()
		{
			Assignments = new List<Activity>();
		}
	}

	public class ScheduleStatus
	{
		public List<Group> FreeGroups;
		public int ConstraintsMet;
	}

	public class Schedule
	{
		public List<TimeSlot> Monday;
		public List<TimeSlot> Wednesday;
		public List<TimeSlot> Tuesday;
		public List<TimeSlot> Thursday;
		public List<TimeSlot> Friday;
	}

	public static class Scheduler
	{
		private static int totalSlotsPerDay;
		private static List<Group> AllGroups;
		private static List<Station> AllStations;

		public static Dictionary<string, string>[,] Schedule(List<Group> groups, List<Station> stations, int slotsPerDay)
		{
			// start monday end Friday
			int dayStart = 1, dayEnd = 5;
			int Day, Slot, i,j, k;

			totalSlotsPerDay = slotsPerDay;
			AllStations = stations;
			AllGroups = groups;

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

		private ScheduleStatus getScheduleStatus(Dictionary<string, string>[,] schedule)
		{
			ScheduleStatus ret = new ScheduleStatus();
			ret.FreeGroups = new List<Group>();

			int Day, Slot;
			int i;

			HashSet<string> groups = new HashSet<string>();

			for (Day = 1; Day <= 5; Day++)
			{
				for (Slot = 1; Slot <= totalSlotsPerDay; Slot++)
				{
					Dictionary<string, string> D = schedule[Day, Slot];

					foreach (KeyValuePair<string, string> P in D)
					{
						groups.Add(P.Key);
					}
				}
			}

			for (i = 0; i < AllGroups.Count; i++)
			{
				if (!groups.Contains(AllGroups[i].Name))
				{
					ret.FreeGroups.Add(AllGroups[i]);
				}
			}

			return ret;
		}

		private Schedule toScheduleClass(Dictionary<string, string>[,] schedule)
		{
			Schedule S = new Schedule();

			int Day, Slot;

			for (Day = 1; Day <= 5; Day++)
			{
				for (Slot = 1; Slot <= 5; Slot++)
				{
					Dictionary<string, string> D = schedule[Day, Slot];

					foreach (KeyValuePair<string, string> P in D)
					{
						
					}
				}
			}

			return S;
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
