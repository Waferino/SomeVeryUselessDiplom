namespace Starikov

open Microsoft.AspNetCore.Identity.EntityFrameworkCore
open Microsoft.EntityFrameworkCore
open Microsoft.AspNetCore.Identity

type AppIdentityDbContext (options) =
    inherit IdentityDbContext<IdentityUser> (options)
