using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameHubApi.Repository.Contracts
{
    [Flags]
    public enum Preference
    {
        Like,
        Dislike,
        Owned,
        WishList
    }
}
