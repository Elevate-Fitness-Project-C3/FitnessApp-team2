# FitnessApp-team2

## IdentityService database

Apply pending migrations from Visual Studio's Package Manager Console before starting the service. The role migration adds any missing `User` and `Admin` roles.

```powershell
Update-Database -Project IdentityService -StartupProject IdentityService
```
