import { Button } from "@/components/ui/button";

const TimeSlotSelector = ({ selectedTime, setSelectedTime, allTimeSlots, timeSlots, isSlotsLoading }) => {
    if (isSlotsLoading) {
      return <div>Loading available times...</div>;
    }
  
    return (
      <div className="flex-1">
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Select a Time
        </label>
        <div className="grid grid-cols-3 gap-2">
          {allTimeSlots.map((time) => {
            const isAvailable = timeSlots.includes(time);
            return (
              <Button
                key={time}
                variant={selectedTime === time ? "default" : "outline"}
                className={`w-full ${!isAvailable ? 'opacity-50' : ''}`}
                onClick={() => setSelectedTime(time)}
                disabled={!isAvailable}
              >
                {time}
              </Button>
            );
          })}
        </div>
      </div>
    );
};

export default TimeSlotSelector;