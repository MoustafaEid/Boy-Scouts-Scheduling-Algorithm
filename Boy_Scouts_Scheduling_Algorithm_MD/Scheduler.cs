using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduling_Algorithm_MD
{
    /*
     * MDD 2/02/2012
     * I am getting rid of the day field for constraints, because it adds an extra layer of
     * complexity when keeping track of how many times a given cub scout group must
     * visit a station. I'm not sure if there's a real use case for the days constraint either.
     * For example, I don't think Michelle would need to have group 1 visit the pool
     * at least twice on Tuesday and once on Thursday. Instead, she would say group 1
     * visits the pool at least three times throughout the week
     */
    
    public class Constraint
    {
        private Group group;
        private uint? groupRank;
        private IList<Station> stations;
        //private IList<StationDay> days;
        private uint? minVisits;
        private uint? maxVisits;
        private uint? priority;

        public Group Group { get; set; }
        public uint? GroupRank { get; set; }
        public IList<Station> Stations { get; set; }
        //public IList<StationDay> Days { get; set; }
        public uint? MinVisits { get; set; }
        public uint? MaxVisits { get; set; }
        public uint? Priority { get; set; }

        public Constraint(Group group, uint? groupRank, IList<Station> stations,
            uint? minVisits, uint? maxVisits, uint? priority)
        {
            this.group = group;
            this.groupRank = groupRank;
            this.stations = stations;
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
        private IList<Station> topStationPicks;
        //private IList<Constraint> constraints;

        public string Name { get; set; }
        public uint Rank { get; set; }
        public IList<Station> TopStationPicks { get; set; }
        //public IList<Constraint> Constraints { get; set; }

        public Group(string groupName, uint groupRank, IList<Station> groupStationPicks)
        {
            name = groupName;
            rank = groupRank;
            topStationPicks = groupStationPicks;
            //constraints = groupConstraints;
        }
    }

    public static class Scheduler
    {
        //each group must visit a station between minVisit and maxVisit times
        struct StationAssignmentRange
        {
            public uint? minVisits;
            public uint? maxVisits;

            public StationAssignmentRange(uint? minVisits, uint? maxVisits)
            {
                this.minVisits = minVisits;
                this.maxVisits = maxVisits;
            }
        }

        public static IList<IList<IDictionary<string, string>>> Schedule
            (IList<Group> groups, IList<Station> stations, IList<Constraint> constraints)
        {

            //compute the numbers of days in camp and the number of stations on each day
            uint numCampDays = findNumCampDays(stations);

            //number of times that each group should be assigned to each station for the week
            Dictionary<Group, Dictionary<Station, StationAssignmentRange>> groupStationVisitRange =
                new Dictionary<Group, Dictionary<Station, StationAssignmentRange>>();

            //populate the newly created groupStationvisitRange with the appropriate values
            populateGroupStationVisitEntries(constraints, groups, stations, ref groupStationVisitRange);

            //assign as many groups as possible to their stations for a given time slot
            IDictionary<string, string> assignments = new Dictionary<string, string>();
            
            //for each station at a given time slot, find the groups that have the station listed as their top picks
            IList<Group> bestGroupsForStation = new List<Group>();

            // [GroupNumber, StationNumber] = Assignment Counter
            int[,] groupStationAssignments = new int[groups.Count, stations.Count];

            //keep track of how many times each group needs to visit a station at a minimum
            uint greatestMinVisits;

            IList<uint> slotsPerDay = new List<uint>();
            findNumSlotsPerDay(stations, numCampDays, ref slotsPerDay);

            for (uint dayNum = 0; dayNum < numCampDays; dayNum++)
            {
                for (uint slotNum = 0; slotNum < slotsPerDay[(int)dayNum]; slotNum++)
                {
                    assignments.Clear();
                    foreach (Station station in stations)
                    {
                        greatestMinVisits = 0;
                        bestGroupsForStation.Clear();

                        if (!isStationAvailableAtSlot(station, dayNum, slotNum))
                            continue;

                        /*
                             *Loop through each group to see who is the best match for
                             *the current station at the current time slot.
                             *The following criteria is used to determine which group
                             *is the best match for the current station:
                             *1. The group has to visit the station more times (min visits)
                             *than all of the other groups
                             *2. The group has one of their top choices to visit this
                             *station, and the other groups have this station listed
                             *as a lower choice
                             *3. Groups with a higher rank have priority over groups
                             *with a lower rank
                             *If there is a tie after these criteria are applied, a
                             *random group will be chosen from the list of best matches
                        */

                        //find best groups for the station based on minVisits attribute
                        foreach (Group group in groups)
                        {
                            uint? groupMinVisits = groupStationVisitRange[group][station].minVisits;
                            if (groupMinVisits == greatestMinVisits ||
                                (!groupMinVisits.HasValue && greatestMinVisits == 0))
                            {
                                bestGroupsForStation.Add(group);
                            }
                            else if (groupMinVisits > greatestMinVisits)
                            {
                                greatestMinVisits = (uint)groupMinVisits;
                                bestGroupsForStation.Clear();
                                bestGroupsForStation.Add(group);
                            }
                        }

                        //filter best groups based on top station picks
                        foreach (Group group in bestGroupsForStation)
                        {

                        }
                    }
                }
            }
            return null;
        }

        private static void populateGroupStationVisitEntries(
            IList<Constraint> constraints, IList<Group> groups, IList<Station> stations,
            ref Dictionary<Group, Dictionary<Station, StationAssignmentRange>> groupStationVisitRange)
        {
            Dictionary<Station, StationAssignmentRange> groupAssignment = 
                new Dictionary<Station, StationAssignmentRange>();

            //first, initialize the groups so there are no constraints for all days of camp
            foreach (Group group in groups)
            {
                groupAssignment.Clear();
                foreach (Station station in stations)
                {
                    groupAssignment.Add(station, new StationAssignmentRange(null, null));
                }
                groupStationVisitRange.Add(group, groupAssignment);
            }

            //now, go through each constraint and update the groupStationVisitRange
            foreach (Constraint constraint in constraints)
            {
                foreach (Group group in groups)
                {
                    groupAssignment.Clear();

                    //group names match or there's no group name specified in the constraints
                    //but the ranks match, or there's no group name or rank specified
                    if (group.Name == constraint.Group.Name || 
                        (constraint.Group == null && group.Rank == constraint.GroupRank) ||
                        (constraint.Group == null && constraint.GroupRank ==  null))
                    {
                        //if the constraint applies to the group, update the
                        //stationAssignmentRanges for each station
                        foreach (Station station in constraint.Stations)
                        {
                            groupAssignment.Add(station, 
                                new StationAssignmentRange(constraint.MinVisits, constraint.MaxVisits));
                        }
                        groupStationVisitRange.Add(group, groupAssignment);
                    }
                }
            }
        }

        private static bool isStationAvailableAtSlot
            (Station station, uint dayNumber, uint slotNumber)
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
