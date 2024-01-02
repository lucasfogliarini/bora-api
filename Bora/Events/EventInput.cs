namespace Bora.Events
{
    public class EventInput
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? Location { get; set; }
        public bool? Public { get; set; }
        public bool AddConference { get; set; } = true;
        public GoogleCalendarColor? Color { get; set; }
		public string CalendarId { get; set; } = "primary";
		public bool CreateReminderTask { get; set; }
	}

	public enum GoogleCalendarColor
	{
		Cinza = 1,
		Vermelho = 2,
		Laranja = 3,
		Amarelo = 4,
		Verde = 5,
		Turquesa = 6,
		Azul = 7,
		Roxo = 8,
		Rosa = 9,
		Borgonha = 10,
		VerdeEscuro = 11
	}
}
