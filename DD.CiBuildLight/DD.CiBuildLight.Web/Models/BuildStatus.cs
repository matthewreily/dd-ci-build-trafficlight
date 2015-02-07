namespace DD.CiBuildLight.Web.Models
{
    public class BuildStatus
    {
        public bool status { get; set; }
        public string builddef { get; set; }
        public bool communicationerror { get; set; }
    }
}