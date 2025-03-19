using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OpenKeyServer.Data;

public class ApplicationUser
{
    [StringLength(150)]
    public string Id { get; set; } = "";
    public bool AllowOld { get; set; } = false;
    [StringLength(300)]
    public string Key { get; set; } = "";
    public int Type { get; set; } = -1;
}
