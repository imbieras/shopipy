using Shopipy.UserManagement.Dtos;

namespace Shopipy.Web.ViewModels;

public abstract class BaseViewModel
{
    public UserResponseDto? CurrentUser { get; set; }
}