namespace HellMail.Domain {

    public class Mail {

        public int id { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public User UserDestination { get; set; }
        public int UserDestinationId { get; set; }
        public User UserSource { get; set; }
        public int UserSourceId { get; set; }

    }
}
