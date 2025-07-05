namespace AMS.Web.Models
{
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<string> UserRoles { get; set; } = new List<string>();
        public IList<string> AllRoles { get; set; } = new List<string>();
        public IList<string> SelectedRoles { get; set; } = new List<string>(); // For checkboxes in the view
    }
}
