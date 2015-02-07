namespace DD.CiBuildLight.Netduino.Model
{
    public class BuildStatus
    {
        public bool Status { get; set; }
        public string BuildDef { get; set; }
        public bool CommunicationError { get; set; }
    }
}