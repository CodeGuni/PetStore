namespace PetStore.Models.ViewModels
{
    public class ProfilePageViewModel
    {
        // Initialize them to avoid NullReferenceExceptions in the view
        public UserProfileViewModel UserProfile { get; set; } = new UserProfileViewModel();
        public ChangePasswordViewModel ChangePassword { get; set; } = new ChangePasswordViewModel();
    }
}