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
		private const int MAXN = 100;

		private static int totalSlotsPerDay;
		private static List<Group> AllGroups;
		private static List<Station> AllStations;
		private static int[] GroupAssignments = new int[MAXN];
		// [GroupNumber, StationNumber] = Assignment Counter
		private static int[,] GroupStationAssignments = new int[MAXN, MAXN];
		private static int[,] GroupRankStationAssignments = new int[MAXN, MAXN];

		public static Dictionary<int, int>[,] Schedule(List<Group> groups, List<Station> stations, int slotsPerDay)
		{
			// start monday end Friday
			int dayStart = 1, dayEnd = 5;
			int Day, Slot, i,j, k;

			totalSlotsPerDay = slotsPerDay;
			AllStations = stations;
			AllGroups = groups;

			// Schedule
			Dictionary<int, int>[,] masterSchedule = new Dictionary<int, int>[10, 20];

			for (i = 0; i < GroupStationAssignments.GetLength(0); i++)
				for (j = 0; j < GroupStationAssignments.GetLength(1); j++)
					GroupStationAssignments[i, j] = GroupRankStationAssignments[i, j] = GroupAssignments[i] = 0;

			for (Day = dayStart; Day <= dayEnd; Day++)
			{
				for (Slot = 1; Slot <= slotsPerDay; Slot++)
				{
					masterSchedule[Day, Slot] = new Dictionary<int, int>();

					// Group busy
					bool[] isGroupBusy = new bool[100];

					for (i = 0; i < stations.Count; i++)
					{
						Station curStation = stations[i];

						if (!isStationAvailableAtSlot(curStation, Day, Slot))
							continue;

						int stationCapacityCounter = 1;

						for (j = 0; j < groups.Count; j++)
						{
							if (stationCapacityCounter > curStation.Capacity)
								break;

							int groupNum = getNextLeastAssignedGroup();
							Group curGroup = groups[groupNum];

							if (isGroupBusy[groupNum] || !canHappenGroupStationAssignment(groupNum, i) || !canHappenGroupRankStationAssignment(curGroup.Rank, i))
								continue;

							// Group is busy in this slot
							isGroupBusy[groupNum] = true;
							// Increment group activites
							GroupAssignments[groupNum]++;
							// Assign the Group to the station
							GroupStationAssignments[groupNum, i]++;
							// Assign the Group's rank to the station
							GroupRankStationAssignments[ curGroup.Rank, i]++;

							stationCapacityCounter++;

							masterSchedule[Day, Slot].Add(groupNum, i);
						}
					}
				}
			}

			return masterSchedule;
		}

		private static int getNextLeastAssignedGroup()
		{
			int i;
			int min = 1 << 30;
			int index = -1;

			for (i = 0; i < AllGroups.Count; i++)
			{
				if (GroupAssignments[i] < min)
				{
					min = GroupAssignments[i];
					index = i;
				}
			}

			return index;
		}
		private static bool canHappenGroupStationAssignment(int groupID, int stationID)
		{
			return GroupStationAssignments[ groupID, stationID ] <= 4;
		}

		private static bool canHappenGroupRankStationAssignment(int groupRank, int stationID)
		{
			return GroupRankStationAssignments[groupRank, stationID] <= 2000;
		}
		
		private static ScheduleStatus getScheduleStatus(Dictionary<int, int>[,] schedule)
		{
			ScheduleStatus ret = new ScheduleStatus();
			ret.FreeGroups = new List<Group>();

			int Day, Slot;
			int i;

			HashSet<int> groups = new HashSet<int>();

			for (Day = 1; Day <= 5; Day++)
			{
				for (Slot = 1; Slot <= totalSlotsPerDay; Slot++)
				{
					Dictionary<int, int> D = schedule[Day, Slot];

					foreach (KeyValuePair<int, int> P in D)
					{
						groups.Add(P.Key);
					}
				}
			}

			for (i = 0; i < AllGroups.Count; i++)
			{
				if (!groups.Contains(i))
				{
					ret.FreeGroups.Add(AllGroups[i]);
				}
			}

			return ret;
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
