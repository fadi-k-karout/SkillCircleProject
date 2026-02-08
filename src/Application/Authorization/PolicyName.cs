namespace Application.Authorization;
public static class PolicyName
{
    public const string CanManageUser = "CanManageUser";
    public const string CanManageReviews = "CanManageReviews";
    public const string CanManageSkills = "CanManageSkills";
    public const string CanManageCourses = "CanManageCourses";
    public const string CanManageVideos = "CanManageVideos";
    public const string CanManageUsersRoles =  "CanManageUsersRolesPolicy";
    public const string CanSeeUserPrivateInformation =  "CanSeeUserPrivateInformation";
    public const string CanSeeUserPayments =  "CanSeeUserPayments";
    public const string CanCreateNewPaymentsForUser =  "CanCreateNewPaymentsForUser";
}