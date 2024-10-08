using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectSecureCoding1.Models;

namespace ProjectSecureCoding1.Data
{
    public interface IUser
    {
        Users Registration (Users users);
        Users Login (Users users);
    }
}