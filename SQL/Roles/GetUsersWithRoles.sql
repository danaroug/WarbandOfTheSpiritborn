-- GetUsersWithRoles.sql
-- Purpose: Get a summary of all users and their assigned roles.
-- Output: One row per user, with a comma-separated list of roles in the "Roles" column.
-- Notes: Useful for generating reports or verifying role assignments for all users.
SELECT 
    u.Email,
    STRING_AGG(r.Name, ', ') AS Roles
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
GROUP BY u.Email
ORDER BY u.Email;
