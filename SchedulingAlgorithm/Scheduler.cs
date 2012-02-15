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
		public int StationPick1;
		public int StationPick2;

		public Group(string N, int R, int S1 = -1, int S2 = -1)
		{
			Name = N;
			Rank = R;
			StationPick1 = S1;
			StationPick2 = S2;
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
		public int totalAvailabltSlots;

		public Station(string N, int C, List<Availability> A)
		{
			Name = N;
			Capacity = C;
			Avail = A;
			totalAvailabltSlots = 0;

			foreach (Availability x in A)
				totalAvailabltSlots += x.Slots.Count;
		}
	}

	public class Assignment
	{
		public Group G;
		public Station S;

		public Assignment(Group g, Station s)
		{
			G = g;
			S = s;
		}
	}

	public class Constraint
	{
		public Group G;
		public Station S;
		public int nTimes;

		public Constraint(Group g, Station s, int n)
		{
			G = g;
			S = s;
			nTimes = n;
		}
	}

	public class TimeSlot
	{
		public List<Assignment> Assignments;

		public TimeSlot()
		{
			Assignments = new List<Assignment>();
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
		private const int MAXN = 500;
		private const int CONSTRAINT_PENALTY = 10;
		private const int NOT_GETTING_FIRST_PICK_PENALTY = 20;
		private const int NOT_GETTING_ANY_PICK_PENALTY = 100;

		private static int totalSlotsPerDay;
		private static List<Group> AllGroups;
		private static List<Station> AllStations;
		private static List<Constraint> AllConstraints;

		private static int[] GroupAssignments = new int[MAXN];
		// [GroupNumber, StationNumber] = Assignment Counter
		private static int[,] GroupStationAssignments = new int[MAXN, MAXN];
		private static int[,] GroupRankStationAssignments = new int[MAXN, MAXN];

		private static bool[] ConstraintMet = new bool[MAXN];

		// stations assignment counts
		private static int[, ,] StationSlotAssignmentsCounts = new int[MAXN, MAXN, MAXN];
		private static int[] StationAssignmentsCounts = new int[MAXN];
		
		public static Dictionary<int, int>[,] Schedule(List<Group> groups, List<Station> stations, List<Constraint> Constraints, int slotsPerDay)
		{
			// start monday end Friday
			int dayStart = 1, dayEnd = 5;
			int Day, Slot, i,j, k;

			totalSlotsPerDay = slotsPerDay;
			AllStations = stations;
			AllGroups = groups;
			AllConstraints = Constraints;

			// Schedule
			Dictionary<int, int>[,] masterSchedule = new Dictionary<int, int>[10, 20];

			for (i = 0; i < GroupStationAssignments.GetLength(0); i++)
				for (j = 0; j < GroupStationAssignments.GetLength(1); j++)
				{
					GroupStationAssignments[i, j] = GroupRankStationAssignments[i, j] = GroupAssignments[i] = StationAssignmentsCounts[i] = 0;

					for (k = 0; k < MAXN; k++)
						StationSlotAssignmentsCounts[i, j, k] = 0;

					ConstraintMet[i] = false;
				}

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

		private static int getStationIndex(Station s)
		{
			int i;
			for (i = 0; i < AllStations.Count; i++)
			{
				if (AllStations[i].Name == s.Name)
					return i;
			}

			return -1;
		}

		private static int score(Dictionary<int, int>[,] masterSchedule, Assignment A, int Day, int Slot)
		{
			int ret = 0;

			int i, j;

			// check if other constraints will be violated if this assignment happens.

			// station cap - 1 for the current assigmment
			int stationIndex = getStationIndex(A.S);
			int stationTotalAvailableSlotsLeft = A.S.totalAvailabltSlots - StationAssignmentsCounts[stationIndex] - 1;

			int otherGroupsNeedThisStation = 0;

			if (stationTotalAvailableSlotsLeft < 0)
				throw new Exception("Error generating schedule. SC-1");

			// look for anyone else who wants this station.
			for (i = 0; i < AllConstraints.Count; i++)
			{
				if (ConstraintMet[i] || AllConstraints[i].S != A.S || AllConstraints[i].G == A.G)
					continue;

				otherGroupsNeedThisStation++;
			}

			if (otherGroupsNeedThisStation > stationTotalAvailableSlotsLeft)
			{
				ret += CONSTRAINT_PENALTY * (otherGroupsNeedThisStation - stationTotalAvailableSlotsLeft);
			}

			int nSecondPicks = 0;
			int nNoPicks = 0;

			// Check how many groups get their second pick instead of first, and how many groups aren't getting any picks
			
			// Copy the total assignment count for stations.
			int[] StationAssignmentsCountsTemp = new int[MAXN];

			for (i = 0; i < AllStations.Count; i++)
				StationAssignmentsCountsTemp[i] = StationAssignmentsCounts[i];

			for (i = 0; i < AllGroups.Count; i++)
			{
				if( AllGroups[i].StationPick1 == -1 )
					continue;

				int stationPick1AvailableSlots = AllStations[AllGroups[i].StationPick1].totalAvailabltSlots - StationAssignmentsCountsTemp[AllGroups[i].StationPick1];
				int stationPick2AvailableSlots = AllStations[AllGroups[i].StationPick2].totalAvailabltSlots - StationAssignmentsCountsTemp[AllGroups[i].StationPick2];

				if (stationPick1AvailableSlots > 0)
				{
					StationAssignmentsCountsTemp[AllGroups[i].StationPick1]++;
				}
				else if (stationPick2AvailableSlots > 0)
				{
					StationAssignmentsCountsTemp[AllGroups[i].StationPick2]++;
					nSecondPicks++;
				}
				else
				{
					nNoPicks++;
				}
			}

			ret += NOT_GETTING_FIRST_PICK_PENALTY * nSecondPicks;
			ret += NOT_GETTING_ANY_PICK_PENALTY * nNoPicks;

			return ret;
		}

		private static int getStationTotalCapacityDuringWeek(Station s, int Day, int Slot)
		{
			int ret = 0;

			int i, j;

			for (i = 0; i < s.Avail.Count; i++)
			{
				if (s.Avail[i].DayNumber == Day)
				{
					for (j = 0; j < s.Avail[i].Slots.Count; j++)
						if (s.Avail[i].Slots[j] > Slot)
							ret++;
				}
				else if (s.Avail[i].DayNumber > Day)
				{
					ret += s.Avail[i].Slots.Count;
				}
			}

			ret *= s.Capacity;

			return ret;
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
