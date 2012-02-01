using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduling_Algorithm_MD
{
    public class Constraint
    {
        //private string groupName;
        //private uint? groupRank;
        private string stationName;
        private IList<StationDay> days;
        private uint? minVisits;
        private uint? maxVisits;
        private uint? priority;

        //public string GroupName { get; set; }
        //public uint? GroupRank { get; set; }
        public string StationName { get; set; }
        public IList<StationDay> Days { get; set; }
        public uint? MinVisits { get; set; }
        public uint? MaxVisits { get; set; }
        public uint? Priority { get; set; }

        public Constraint(string stationName, IList<StationDay> days, uint? minVisits, uint? maxVisits, uint? priority)
        {
            this.stationName = stationName;
            this.days = days;
            this.minVisits = minVisits;
            this.maxVisits = maxVisits;
            this.priority = priority;
        }

    }

    public class StationDay
    {
        private uint dayNumber;
        private IList<uint> openSlotNumbers;

        public uint DayNumber { get; set; }
        public IList<uint> OpenSlotNumbers { get; set; }

        public StationDay(uint stationDayNumber, IList<uint> stationSlotNumbers)
        {
            dayNumber = stationDayNumber;
            openSlotNumbers = stationSlotNumbers;
        }
    }

    public class Station
    {
        private string name;
        private uint capacity;
        private IList<StationDay> availabilities;

        public string Name { get; set; }
        public uint Capacity { get; set; }
        public IList<StationDay> Availabilities { get; set; }

        public Station(string stationName, uint stationCapacity, IList<StationDay> stationAvailabilities)
        {
            name = stationName;
            capacity = stationCapacity;
            availabilities = stationAvailabilities;
        }
    }

    public class Group
    {
        private string name;
        private uint rank;
        private IList<Constraint> constraints;

        public string Name { get; set; }
        public uint Rank { get; set; }
        public IList<Constraint> Constraints { get; set; }

        public Group(string groupName, uint groupRank, IList<Constraint> groupConstraints)
        {
            name = groupName;
            rank = groupRank;
            constraints = groupConstraints;
        }
    }

    public class Scheduler
    {
        public static IList<IList<IDictionary<string, string>>> Schedule
            (IList<Group> groups, IList<Station> stations, IList<Constraint> constraints)
        {
            // [GroupNumber, StationNumber] = Assignment Counter
            int[,] groupStationAssignments = new int[groups.Count, stations.Count];

            //compute the numbers of days in camp and the number of stations on each day
            uint numCampDays = findNumCampDays(stations);
            IList<uint> slotsPerDay = new List<uint>();
            findNumSlotsPerDay(stations, numCampDays, ref slotsPerDay);

            for (uint dayNum = 0; dayNum < numCampDays; dayNum++)
            {
                for (uint slotNum = 0; slotNum < slotsPerDay[(int)dayNum]; slotNum++)
                {
                    IDictionary<string, string> assignments = new Dictionary<string, string>();
                    foreach (Station station in stations)
                    {
                        if (!isStationAvailableAtSlot(station, dayNum, slotNum))
                            continue;

                        foreach (Group group in groups)
                        {
                            //assign the best group to the current station here
                        }
                    }
                }
            }
            return null;
        }

        private static bool isStationAvailableAtSlot(Station station, uint dayNumber, uint slotNumber)
        {
            if (station.Capacity == 0)
                return false;

            foreach (StationDay day in station.Availabilities)
            {
                if (day.DayNumber == dayNumber && day.OpenSlotNumbers.Contains(slotNumber))
                    return true;
            }

            return false;
        }

        private static uint findNumCampDays(IList<Station> stations)
        {
            uint numCampDays = 0;
            foreach (Station currentStation in stations)
            {
                foreach (StationDay currentDay in currentStation.Availabilities)
                {
                    if (currentDay.DayNumber > numCampDays)
                        numCampDays = currentDay.DayNumber;
                }
            }
            return numCampDays;
        }

        private static void findNumSlotsPerDay(IList<Station> stations, uint numCampDays, ref IList<uint> slotsPerDay)
        {
            for (int dayNum = 1; dayNum <= numCampDays; dayNum++)
            {
                uint numSlots = 0;
                foreach (Station currentStation in stations)
                {
                    foreach (StationDay currentDay in currentStation.Availabilities)
                    {
                        if (currentDay.DayNumber == dayNum)
                        {
                            foreach (uint slotNumber in currentDay.OpenSlotNumbers)
                            {
                                if (slotNumber > numSlots)
                                    numSlots = slotNumber;
                            }
                        }
                    }
                }
                slotsPerDay[dayNum - 1] = numSlots;
            }
        }

    }
}
