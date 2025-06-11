namespace HUBT_Social_Core.Models.Requests.Temp
{
    public class UpdateTimetableRequest
    {

        public string Id { get; set; } = string.Empty;
        private DateTime _starttime;
        private DateTime? _endtime;
        public DateTime NewStartTime
        {
            get => _starttime;
            set
            {
                _starttime = value;
            }
        }

        public DateTime? NewEndTime
        {
            get => _endtime;
            set
            {
                if (value <= _starttime)
                    throw new ArgumentException("Endtime must be later than Starttime.");
                _endtime = value;
            }
        }

    }
}
