using System;
using System.Collections.Generic;

namespace DataAccess.Models;

    public partial  class SystemConfig
    {
    public int SessionTimeout { get; set; }
    public bool EnableLogging { get; set; }
    public bool EnableTwoFactorAuth { get; set; }
    public bool EnableAutoLogout { get; set; }
}
